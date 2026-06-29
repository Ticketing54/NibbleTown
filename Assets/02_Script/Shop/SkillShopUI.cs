using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillShopUI : MonoBehaviour
{
    [Header("왼쪽 - 스킬 목록")]
    [SerializeField] private SkillShopSlotUI[] skillSlots = new SkillShopSlotUI[3];

    [Header("오른쪽 - 장착 슬롯")]
    [SerializeField] private SkillShopEquippedSlotUI[] equippedSlots = new SkillShopEquippedSlotUI[3];

    [Header("구매 확인창")]
    [SerializeField] private GameObject      confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button          confirmBtn;
    [SerializeField] private Button          cancelBtn;

    [Header("슬롯 교체창")]
    [SerializeField] private GameObject replaceModePanel;

    [Header("공통")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button          closeButton;
    [SerializeField] private GameObject      panel;

    private ISkillEquipment skillEquipment;
    private IGoldWallet     goldWallet;

    private SkillShopSlotUI selectedSkillSlot;
    private int             pendingSkillId = -1;
    private bool            waitingForReplace;

    private void Awake()
    {
        confirmPanel.SetActive(false);
        replaceModePanel?.SetActive(false);
        panel.SetActive(false);
        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(OnCancelConfirm);
        closeButton?.onClick.AddListener(GameEvents.RaiseNpcConversationCloseRequested);
    }

    private void Start()
    {
        if (PlayerRegistry.All.Count == 0) return;
        var player = PlayerRegistry.All[0].gameObject;
        player.TryGetComponent(out skillEquipment);
        player.TryGetComponent(out goldWallet);
    }

    private void OnEnable()
    {
        GameEvents.OnSkillShopOpen  += Open;
        GameEvents.OnSkillShopClose += Close;
        GameEvents.OnSkillEquipped  += OnSkillEquipped;
        GameEvents.OnGoldChanged    += OnGoldChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnSkillShopOpen  -= Open;
        GameEvents.OnSkillShopClose -= Close;
        GameEvents.OnSkillEquipped  -= OnSkillEquipped;
        GameEvents.OnGoldChanged    -= OnGoldChanged;
    }

    // ── 열기 / 닫기 ──────────────────────────────────────────────

    public void Open(IReadOnlyList<int> skills)
    {
        if (skillEquipment == null && PlayerRegistry.All.Count > 0)
        {
            var player = PlayerRegistry.All[0].gameObject;
            player.TryGetComponent(out skillEquipment);
            player.TryGetComponent(out goldWallet);
        }

        panel.SetActive(true);
        confirmPanel.SetActive(false);
        replaceModePanel?.SetActive(false);
        ResetSelection();
        PopulateSkillList(skills);
        RefreshEquippedSlots();
        GameEvents.OnGoldRefreshed += OnGoldChanged;
        GameEvents.RaiseGoldRefreshRequested();
        GameEvents.OnGoldRefreshed -= OnGoldChanged;
    }

    public void Close()
    {
        if (waitingForReplace) return;
        panel.SetActive(false);
    }

    // ── 스킬 목록 구성 ────────────────────────────────────────────

    private void PopulateSkillList(IReadOnlyList<int> skills)
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] == null) continue;

            var skill = (skills != null && i < skills.Count) ? GameDataManager.Skills.Get(skills[i]) : null;
            skillSlots[i].gameObject.SetActive(skill != null);
            if (skill != null) skillSlots[i].Init(skill, this);
        }
    }

    private void RefreshEquippedSlots()
    {
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i] == null) continue;
            equippedSlots[i].Init(i, this);
            equippedSlots[i].SetSkill(skillEquipment?.GetEquipped(i));
        }
    }

    // ── 클릭 콜백 ─────────────────────────────────────────────────

    public void OnSkillSlotClicked(SkillShopSlotUI slot)
    {
        if (waitingForReplace) return;

        selectedSkillSlot?.SetSelected(false);
        selectedSkillSlot = slot;
        selectedSkillSlot.SetSelected(true);

        var skill = GameDataManager.Skills.Get(selectedSkillSlot.SkillId);
        if (skill == null) return;

        confirmText.text = $"{skill.skillName}\n{skill.price}G에 구매하시겠습니까?";
        confirmPanel.SetActive(true);
    }

    public void OnEquippedSlotClicked(SkillShopEquippedSlotUI slot)
    {
        if (!waitingForReplace) return;

        var skill = GameDataManager.Skills.Get(pendingSkillId);
        if (skill == null || goldWallet == null || skillEquipment == null)
        {
            ExitReplaceMode();
            return;
        }

        if (!goldWallet.SpendGold(skill.price))
        {
            ExitReplaceMode();
            return;
        }

        skillEquipment.SetSlot(slot.SlotIndex, skill.skillId);
        ExitReplaceMode();
        ResetSelection();
    }

    // ── 구매 확인 ─────────────────────────────────────────────────

    private void OnConfirm()
    {
        confirmPanel.SetActive(false);
        if (selectedSkillSlot == null || goldWallet == null || skillEquipment == null) return;

        var skill = GameDataManager.Skills.Get(selectedSkillSlot.SkillId);
        if (skill == null) return;

        int emptySlot = FindEmptyEquippedSlot();
        if (emptySlot >= 0)
        {
            if (!goldWallet.SpendGold(skill.price)) return;
            skillEquipment.SetSlot(emptySlot, skill.skillId);
            ResetSelection();
        }
        else
        {
            // 빈 슬롯 없음 → 교체 모드 진입 (취소 불가)
            pendingSkillId    = skill.skillId;
            waitingForReplace = true;
            if (closeButton != null) closeButton.interactable = false;
            replaceModePanel?.SetActive(true);
        }
    }

    private void OnCancelConfirm()
    {
        confirmPanel.SetActive(false);
        selectedSkillSlot?.SetSelected(false);
        selectedSkillSlot = null;
    }

    // ── 교체 모드 ─────────────────────────────────────────────────

    private void ExitReplaceMode()
    {
        waitingForReplace = false;
        pendingSkillId    = -1;
        if (closeButton != null) closeButton.interactable = true;
        replaceModePanel?.SetActive(false);
    }

    private int FindEmptyEquippedSlot()
    {
        for (int i = 0; i < equippedSlots.Length; i++)
            if (skillEquipment.GetEquipped(i) == null) return i;
        return -1;
    }

    // ── 선택 초기화 ───────────────────────────────────────────────

    private void ResetSelection()
    {
        selectedSkillSlot?.SetSelected(false);
        selectedSkillSlot = null;
    }

    // ── 이벤트 ───────────────────────────────────────────────────

    private void OnSkillEquipped(int slotIndex, int skillId)
    {
        if (!panel.activeSelf || (uint)slotIndex >= equippedSlots.Length) return;
        equippedSlots[slotIndex].SetSkill(skillId >= 0 ? GameDataManager.Skills.Get(skillId) : null);
    }

    private void OnGoldChanged(int total)
    {
        if (goldText != null) goldText.text = $"{total}G";
    }
}
