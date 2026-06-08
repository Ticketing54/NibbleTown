using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private GameObject weapon;

    [Header("Tools")]
    [SerializeField] private GameObject fishingRod;
    [SerializeField] private GameObject pickaxe;

    private IInteractionState interactionState;

    private void Awake()
    {
        interactionState = GetComponent<IInteractionState>();
    }

    private bool isCombatMode;

    private void Start()
    {
        HideAll();
    }

    private void OnEnable()
    {
        if (interactionState != null)
        {
            interactionState.OnInteractionStarted += OnInteractionStarted;
            interactionState.OnInteractionEnded   += OnInteractionEnded;
        }
        GameEvents.OnNightBegin += EnterCombatMode;
        GameEvents.OnDayBegin   += ExitCombatMode;
    }

    private void OnDisable()
    {
        if (interactionState != null)
        {
            interactionState.OnInteractionStarted -= OnInteractionStarted;
            interactionState.OnInteractionEnded   -= OnInteractionEnded;
        }
        GameEvents.OnNightBegin -= EnterCombatMode;
        GameEvents.OnDayBegin   -= ExitCombatMode;
    }

    private void EnterCombatMode()
    {
        isCombatMode = true;
        HideAll();
        ShowCurrentWeapon();
    }

    private void ExitCombatMode()
    {
        isCombatMode = false;
        HideAll();
    }

    public void RefreshWeapon()
    {
        HideAll();
        ShowCurrentWeapon();
    }

    private void OnInteractionStarted(InteractionAnimType _animType)
    {
        HideAll();
        switch (_animType)
        {
            case InteractionAnimType.Fish: Show(fishingRod); break;
            case InteractionAnimType.Mine: Show(pickaxe);    break;
        }
    }

    private void OnInteractionEnded()
    {
        HideAll();
        if (isCombatMode) ShowCurrentWeapon();
    }

    private void ShowCurrentWeapon() => Show(weapon);

    private void Show(GameObject _obj)
    {
        if (_obj != null) _obj.SetActive(true);
    }

    private void HideAll()
    {
        if (weapon     != null) weapon.SetActive(false);
        if (fishingRod != null) fishingRod.SetActive(false);
        if (pickaxe    != null) pickaxe.SetActive(false);
    }
}
