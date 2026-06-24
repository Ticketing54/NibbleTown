using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NpcController : MonoBehaviour
{
    [SerializeField] private string            hintText  = "대화하기";
    [SerializeField] private CinemachineCamera npcCamera;
    [SerializeField] private Transform         playerSeat;

    public string HintText => hintText;

    private bool inConversation;
    private bool isDaytime;

    private IMovementLock       playerMover;
    private CharacterController playerCC;
    private Transform           playerTransform;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (npcCamera != null) npcCamera.Priority = 0;
    }

    private void OnEnable()
    {
        GameEvents.OnNpcConversationCloseRequested += OnCloseRequested;
        GameEvents.OnPhaseChanged                  += OnPhaseChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnNpcConversationCloseRequested -= OnCloseRequested;
        GameEvents.OnPhaseChanged                  -= OnPhaseChanged;
        if (inConversation) ForceEndConversation();
    }

    private void OnPhaseChanged(int _day, StagePhase _phase)
    {
        isDaytime = _phase == StagePhase.Day;
        if (!isDaytime && inConversation)
            GameEvents.RaiseNpcConversationCloseRequested();
    }

    private void OnCloseRequested()
    {
        if (inConversation) StartCoroutine(CoEndConversation());
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.TryGetComponent<IMovementLock>(out playerMover)) return;

        playerCC        = _other.GetComponent<CharacterController>();
        playerTransform = _other.transform;

        if (isDaytime)
        {
            _other.GetComponent<PlayerInteraction>()?.SetNearNpc(this);
            GameEvents.RaiseInteractionTextShow(true, $"{hintText} [F]");
        }
        else
        {
            GameEvents.RaiseInteractionTextShow(true, "낮에만 이용할 수 있습니다");
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (!_other.TryGetComponent<IMovementLock>(out _)) return;

        _other.GetComponent<PlayerInteraction>()?.ClearNearNpc(this);
        playerMover     = null;
        playerCC        = null;
        playerTransform = null;
        if (!inConversation)
            GameEvents.RaiseInteractionTextShow(false, string.Empty);
    }

    public void Interact()
    {
        if (!inConversation) StartCoroutine(CoStartConversation());
    }

    private IEnumerator CoStartConversation()
    {
        playerMover?.LockMovement(true);
        GameEvents.RaisePlayerInputLocked(true);
        GameEvents.RaiseInteractionTextShow(false, string.Empty);

        bool fadeDone = false;
        GameEvents.RaiseFadeIn(onComplete: () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        // 위치 및 방향 이동
        if (playerSeat != null && playerTransform != null)
        {
            if (playerCC != null) playerCC.enabled = false;
            playerTransform.position = playerSeat.position;
            playerTransform.rotation = playerSeat.rotation;
            if (playerCC != null) playerCC.enabled = true;
        }

        // 카메라 전환 후 블렌딩 완료 대기
        if (npcCamera != null)
        {
            npcCamera.Priority = 100;
            
            yield return null;

            var brain = Camera.main?.GetComponent<CinemachineBrain>();
            if (brain != null)
                yield return new WaitUntil(() => !brain.IsBlending);
        }

        inConversation = true;

        fadeDone = false;
        GameEvents.RaiseFadeOut(onComplete: () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        GameEvents.RaiseNpcConversationChanged(true);
    }

    private IEnumerator CoEndConversation()
    {
        bool fadeDone = false;
        GameEvents.RaiseFadeIn(onComplete: () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        if (npcCamera != null) npcCamera.Priority = 0;
        inConversation = false;
        GameEvents.RaiseNpcConversationChanged(false);
        playerMover?.LockMovement(false);
        GameEvents.RaisePlayerInputLocked(false);

        fadeDone = false;
        GameEvents.RaiseFadeOut(onComplete: () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        if (playerMover != null)
            GameEvents.RaiseInteractionTextShow(true, $"{hintText} [F]");
    }

    private void ForceEndConversation()
    {
        StopAllCoroutines();
        if (npcCamera != null) npcCamera.Priority = 0;
        inConversation = false;
        GameEvents.RaiseNpcConversationChanged(false);
        playerMover?.LockMovement(false);
        GameEvents.RaisePlayerInputLocked(false);
    }
}
