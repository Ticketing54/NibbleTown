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

    private void Start()
    {
        HideAll();
        ShowCurrentWeapon();
    }

    private void OnEnable()
    {
        if (interactionState == null) return;
        interactionState.OnInteractionStarted += OnInteractionStarted;
        interactionState.OnInteractionEnded   += OnInteractionEnded;
    }

    private void OnDisable()
    {
        if (interactionState == null) return;
        interactionState.OnInteractionStarted -= OnInteractionStarted;
        interactionState.OnInteractionEnded   -= OnInteractionEnded;
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
        ShowCurrentWeapon();
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
