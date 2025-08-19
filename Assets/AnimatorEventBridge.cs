using UnityEngine;

public class AnimatorEventBridge : MonoBehaviour
{
    public Animator animator;
    public string boolName = "Open";

    // UnityEvent에서 true/false 구분 없이 호출 가능
    public void SetOpenTrue()
    {
        animator.SetBool(boolName, true);
    }

    public void SetOpenFalse()
    {
        animator.SetBool(boolName, false);
    }

    // 토글버전도 필요하면
    public void ToggleOpen()
    {
        bool isOpen = animator.GetBool(boolName);
        animator.SetBool(boolName, !isOpen);
    }
}
