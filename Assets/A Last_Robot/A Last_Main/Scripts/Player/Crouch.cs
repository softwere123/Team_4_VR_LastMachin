using UnityEngine;
using Autohand.Demo;

public class VRClutchCrouch : MonoBehaviour
{
    public XRHandControllerLink handController;
    public CommonButton toggleButton = CommonButton.secondaryButton;

    public Transform xrOrigin;
    public float crouchAmount = -0.4f;
    public CapsuleCollider bodyCollider;

    private Vector3 originStartPos;
    private float originalHeight;
    private Vector3 originalCenter;

    private bool isCrouching = false;
    private bool wasButtonPressed = false;

    void Start()
    {
        // XR Origin 초기 위치 저장
        originStartPos = xrOrigin.localPosition;

        // 콜라이더 초기값 저장
        if (bodyCollider != null)
        {
            originalHeight = bodyCollider.height;
            originalCenter = bodyCollider.center;
        }

    }

    private void Update()
    {
        if (handController == null) return;

        // 버튼 현재 상태 확인
        bool isPressedNow = handController.ButtonPressed(toggleButton);

        // 버튼이 막 눌린 순간 감지 (ButtonDown 효과)
        if (isPressedNow && !wasButtonPressed)
        {
            if (!isCrouching)
            {
                OnCrouchStart();
                isCrouching = true;
            }
            else
            {
                OnCrouchEnd();
                isCrouching = false;
            }
        }

        // 버튼 상태 업데이트
        wasButtonPressed = isPressedNow;
    }

    void OnCrouchStart()
    {
        xrOrigin.localPosition = originStartPos + new Vector3(0, crouchAmount, 0);

        if (bodyCollider != null)
        {
            bodyCollider.height = originalHeight + crouchAmount;
            bodyCollider.center = originalCenter + new Vector3(0, crouchAmount / 2f, 0);
        }

        Debug.Log("앉기 시작 (토글)");
    }

    void OnCrouchEnd()
    {
        xrOrigin.localPosition = originStartPos;

        if (bodyCollider != null)
        {
            bodyCollider.height = originalHeight;
            bodyCollider.center = originalCenter;
        }

        Debug.Log("일어서기 (토글)");
    }
}
