using UnityEngine;
using Autohand;
using Autohand.Demo;

// 버튼을 누를 때마다 비행 모드를 켜고 끈다 (AutoHandPlayer.ToggleFlying).
// 비행 중일 때만 SetElemental의 자석 당김이 켜지도록 되어 있어서(그라운드 마찰과 안 겹치게),
// 이 스크립트가 비행 상태를 결정하는 유일한 진입점이다.
public class FlyToggle : MonoBehaviour
{
    public XRHandControllerLink handController;
    public CommonButton toggleButton = CommonButton.menuButton;

    private bool wasButtonPressed = false;

    private void Update()
    {
        if (handController == null)
            return;

        bool isPressedNow = handController.ButtonPressed(toggleButton);

        if (isPressedNow && !wasButtonPressed)
        {
            if (AutoHandPlayer.Instance != null)
                AutoHandPlayer.Instance.ToggleFlying();
        }

        wasButtonPressed = isPressedNow;
    }
}
