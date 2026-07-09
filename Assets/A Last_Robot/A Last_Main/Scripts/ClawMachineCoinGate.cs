using UnityEngine;
using UnityEngine.XR.Content.Interaction;

// XRPushButton의 OnPress/OnRelease를 ClawMachine.OnButtonPress/OnButtonRelease 대신
// 여기로 연결한다. 코인이 있어야만 실제로 ClawMachine에 버튼 입력을 전달한다.
// (ClawMachine.cs는 Unity 샘플 원본이라 직접 건드리지 않고 이 게이트로 감쌌다.)
public class ClawMachineCoinGate : MonoBehaviour
{
    public ClawMachine clawMachine;
    public int costPerPlay = 1;

    // XRPushButton.OnPress에 연결
    public void OnButtonPress()
    {
        if (CoinManager.instance == null || !CoinManager.instance.TrySpendCoin(costPerPlay))
            return; // 코인 없으면 버튼이 그냥 안 먹힌다

        clawMachine.OnButtonPress();
    }

    // XRPushButton.OnRelease에 연결
    public void OnButtonRelease()
    {
        clawMachine.OnButtonRelease();
    }
}
