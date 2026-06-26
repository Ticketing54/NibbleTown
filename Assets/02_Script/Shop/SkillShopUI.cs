using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillShopUI : MonoBehaviour
{
    [Header("왼쪽 - 스킬 목록")]
    [SerializeField] private Transform        skillListContainer;
    [SerializeField] private SkillShopSlotUI  skillSlotPrefab;

    [Header("오른쪽 - 장착 슬롯")]
    [SerializeField] private SkillShopEquippedSlotUI[] equippedSlots = new SkillShopEquippedSlotUI[3];

    [Header("확인창")]
    [SerializeField] private GameObject      confirmPanel;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button          confirmBtn;
    [SerializeField] private Button          cancelBtn;

    [Header("공통")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button          closeButton;
    [SerializeField] private GameObject      panel;

    private ISkillEquipment skillEquipment;
    private IGoldWallet     goldWallet;

    private readonly List<SkillShopSlotUI> skillSlots = new();
    private SkillShopSlotUI          selectedSkillSlot;
    private SkillShopEquippedSlotUI  selectedEquippedSlot;

    private void Awake()
    {
        confirmPanel.SetActive(false);
        panel.SetActive(false);
        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(() => confirmPanel.SetActive(false));
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
        selectedSkillSlot    = null;
        selectedEquippedSlot = null;
        PopulateSkillList(skills);
        RefreshEquippedSlots();
        GameEvents.RaiseGoldRefreshRequested();
    }

    public void Close() => panel.SetActive(false);

    private void PopulateSkillList(IReadOnlyList<int> skills)
    {
        foreach (var s in skillSlots) Destroy(s.gameObject);
        skillSlots.Clear();

        if (skills == null) return;

        foreach (int skillId in skills)
        {
            var skill = GameDataManager.Skills.Get(skillId);
            if (skill == null) continue;

            var slot = Instantiate(skillSlotPrefab, skillListContainer, false);
            slot.gameObject.SetActive(true);
            slot.Init(skill, this);
            skillSlots.Add(slot);
        }
    }

    private void RefreshEquippedSlots()
    {
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            equippedSlots[i].Init(i, this);
            equippedSlots[i].SetSkill(skillEquipment?.GetEquipped(i));
        }
    }

    // ── 클릭 콜백 ─────────────────────────────────────────────────

    // 왼쪽 스킬 선택
    public void OnSkillSlotClicked(SkillShopSlotUI slot)
    {
        selectedSkillSlot?.SetSelected(false);
        selectedSkillSlot = slot;
        selectedSkillSlot.SetSelected(true);
        TryShowConfirm();
    }

    // 오른쪽 장착 슬롯 선택
    public void OnEquippedSlotClicked(SkillShopEquippedSlotUI slot)
    {
        selectedEquippedSlot?.SetSelected(false);
        selectedEquippedSlot = slot;
        selectedEquippedSlot.SetSelected(true);
        TryShowConfirm();
    }

    private void TryShowConfirm()
    {
        if (selectedSkillSlot == null || selectedEquippedSlot == null) return;

        var skill = GameDataManager.Skills.Get(selectedSkillSlot.SkillId);
        if (skill == null) return;

        confirmText.text = $"{skill.skillName}을(를)\n슬롯 {selectedEquippedSlot.SlotIndex + 1}에 {skill.price}G로 배정하시겠습니까?";
        confirmPanel.SetActive(true);
    }

    // ── 구매 확인 ─────────────────────────────────────────────────

    private void OnConfirm()
    {
        confirmPanel.SetActive(false);
        if (selectedSkillSlot == null || selectedEquippedSlot == null) return;
        if (goldWallet == null || skillEquipment == null) return;

        var skill = GameDataManager.Skills.Get(selectedSkillSlot.SkillId);
        if (skill == null) return;

        if (!goldWallet.SpendGold(skill.price)) return;

        skillEquipment.SetSlot(selectedEquippedSlot.SlotIndex, skill.skillId);

        selectedSkillSlot.SetSelected(false);
        selectedEquippedSlot.SetSelected(false);
        selectedSkillSlot    = null;
        selectedEquippedSlot = null;
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
