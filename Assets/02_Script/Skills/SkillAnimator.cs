using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkillAnimator : MonoBehaviour
{
    [SerializeField] private string placeholderClipName = "SkillBase";

    private Animator                  animator;
    private AnimatorOverrideController overrideController;

    private static readonly int SkillHash = Animator.StringToHash("Skill");

    private bool isLooping;

    private void Awake()
    {
        animator           = GetComponent<Animator>();
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    private void Update()
    {
        if (!isLooping) return;
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("SkillBase") && info.normalizedTime >= 0.85f)
            animator.Play("SkillBase", 0, 0f);
    }

    public void Play(AnimationClip clip)
    {
        if (clip == null) return;
        overrideController[placeholderClipName] = clip;
        animator.SetTrigger(SkillHash);
    }

    public void PlayLooping(AnimationClip clip)
    {
        if (clip == null) return;
        isLooping = true;
        overrideController[placeholderClipName] = clip;
        animator.SetTrigger(SkillHash);
    }

    public void StopLooping() => isLooping = false;

    public IEnumerator WaitForCompletion()
    {
        // 트랜지션이 시작될 때까지 한 프레임 대기
        yield return null;

        // SkillBase 상태로 진입할 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkillBase"))
            yield return null;

        // SkillBase 재생이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("SkillBase") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
