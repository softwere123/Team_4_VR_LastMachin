using UnityEngine;
using UnityEngine.InputSystem;

public class VRClutchCrouch : MonoBehaviour
{
    public InputActionReference crouchAction;      // grip 같은 버튼 입력
    public Transform xrOrigin;                     // XR Origin 전체
    public float crouchAmount = -0.4f;             // 시야 낮추는 정도
    public CapsuleCollider bodyCollider;           // 피격용 콜라이더

    private Vector3 originStartPos;
    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        originStartPos = xrOrigin.localPosition;

        if (bodyCollider != null)
        {
            originalHeight = bodyCollider.height;
            originalCenter = bodyCollider.center;
        }
    }

    void OnEnable()
    {
        crouchAction.action.started += OnCrouchStart;
        crouchAction.action.canceled += OnCrouchEnd;
    }

    void OnDisable()
    {
        crouchAction.action.started -= OnCrouchStart;
        crouchAction.action.canceled -= OnCrouchEnd;
    }

    void OnCrouchStart(InputAction.CallbackContext ctx)
    {
        xrOrigin.localPosition = originStartPos + new Vector3(0, crouchAmount, 0);

        if (bodyCollider != null)
        {
            bodyCollider.height = originalHeight + crouchAmount;
            bodyCollider.center = originalCenter + new Vector3(0, crouchAmount / 2f, 0);
        }
    }

    void OnCrouchEnd(InputAction.CallbackContext ctx)
    {
        xrOrigin.localPosition = originStartPos;

        if (bodyCollider != null)
        {
            bodyCollider.height = originalHeight;
            bodyCollider.center = originalCenter;
        }
    }
}
