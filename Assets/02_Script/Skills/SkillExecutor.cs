using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SkillBook))]
[RequireComponent(typeof(CharacterStat))]
public class SkillExecutor : MonoBehaviour
{
    [SerializeField] private InputActionReference skill1Action;
    [SerializeField] private InputActionReference skill2Action;
    [SerializeField] private InputActionReference skill3Action;
    [SerializeField] private LayerMask            monsterLayer;

    private SkillBook     skillBook;
    private CharacterStat stat;
    private float[]       cooldownTimers = new float[3];

    private void Awake()
    {
        skillBook = GetComponent<SkillBook>();
        stat      = GetComponent<CharacterStat>();
    }

    private void OnEnable()
    {
        skill1Action.action.Enable();
        skill2Action.action.Enable();
        skill3Action.action.Enable();
        skill1Action.action.performed += OnSkill1Input;
        skill2Action.action.performed += OnSkill2Input;
        skill3Action.action.performed += OnSkill3Input;
    }

    private void OnDisable()
    {
        skill1Action.action.performed -= OnSkill1Input;
        skill2Action.action.performed -= OnSkill2Input;
        skill3Action.action.performed -= OnSkill3Input;
        skill1Action.action.Disable();
        skill2Action.action.Disable();
        skill3Action.action.Disable();
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;
    }

    private void OnSkill1Input(InputAction.CallbackContext _) => TryExecute(0);
    private void OnSkill2Input(InputAction.CallbackContext _) => TryExecute(1);
    private void OnSkill3Input(InputAction.CallbackContext _) => TryExecute(2);

    private void TryExecute(int _slotIndex)
    {
        SkillData skill = skillBook.GetEquipped(_slotIndex);
        if (skill == null)               return;
        if (cooldownTimers[_slotIndex] > 0f) return;
        if (stat.CurrentMP < skill.mpCost)   return;

        stat.UseMP(skill.mpCost);

        var ctx = new SkillContext
        {
            caster      = gameObject,
            stat        = stat,
            targetLayer = monsterLayer
        };

        skill.Execute(ctx);
        cooldownTimers[_slotIndex] = skill.cooldown;
        GameEvents.RaiseSkillUsed(_slotIndex);
    }

    public float GetCooldownRatio(int _slotIndex)
    {
        SkillData skill = skillBook.GetEquipped(_slotIndex);
        if (skill == null || skill.cooldown <= 0f) return 0f;
        return Mathf.Clamp01(cooldownTimers[_slotIndex] / skill.cooldown);
    }
}
