using UnityEngine;
using Autohand;

// "Flying" 물리 버튼(PhysicsGadgetButton 등)의 OnPressed 이벤트에 ActivateFlying()을 연결해서 쓴다.
// 누르는 순간 무중력 비행 모드로 전환되고, RobotHand L/R(손)을 뒤로 저으면(패들) 앞으로 나아간다.
// 끄는 기능은 없음 - 버튼 쪽 lockOnPressed를 켜서 물리적으로도 다시 안 눌리게 하는 걸 권장.
//
// 2차 수정: AutoHandPlayer.Instance 싱글톤에만 기대지 않고, RobotHand L/R과 플레이어
// Rigidbody를 인스펙터에서 직접 연결할 수 있게 했다. 1차 버전이 안 먹었던 이유로 가장 의심되는 건
// 여러 플레이어 리그(예: 예전 Auto Hand Player Container UI Option2 vs 지금 쓰는 XR)가 섞여있거나
// Editor의 Reload Domain 설정 때문에 AutoHandPlayer.Instance가 엉뚱한(오래된) 인스턴스를
// 가리키는 경우라, 직접 참조로 고정해서 그 문제를 피한다.
public class FlyOnButtonPress : MonoBehaviour
{
    [Header("직접 연결 (비워두면 Awake에서 AutoHandPlayer.Instance 기준으로 자동 채움)")]
    public Hand handLeft;
    public Hand handRight;
    public Rigidbody playerBody;

    [Header("손 허우적(패들) 전진")]
    public bool usePaddleMovement = true;
    public float paddleForce = 5f;     // 패들 추진력 배수
    public float paddleMinSpeed = 1f;  // 이 속도 이상으로 손을 뒤로 저어야 추진력이 붙음

    private bool isFlyingActive = false;

    private void Awake()
    {
        var player = AutoHandPlayer.Instance;
        if (player == null)
            return;

        if (handLeft == null) handLeft = player.handLeft;
        if (handRight == null) handRight = player.handRight;
        if (playerBody == null) playerBody = player.body;
    }

    // PhysicsGadgetButton.OnPressed (또는 이에 준하는 버튼 이벤트)에 연결
    public void ActivateFlying()
    {
        Debug.Log("[FlyOnButtonPress] ActivateFlying 호출됨 - 버튼 배선은 정상");

        var player = AutoHandPlayer.Instance;
        if (player != null)
        {
            player.useGrounding = false;
            if (player.body != null)
            {
                player.body.useGravity = false;
                if (playerBody == null)
                    playerBody = player.body;
            }
        }
        else if (playerBody != null)
        {
            playerBody.useGravity = false;
        }

        isFlyingActive = true;
    }

    private void Update()
    {
        if (!isFlyingActive || !usePaddleMovement || playerBody == null)
            return;

        var player = AutoHandPlayer.Instance;
        Vector3 forward = (player != null && player.forwardFollow != null)
            ? player.forwardFollow.forward
            : playerBody.transform.forward;

        ApplyHandPaddle(handRight, forward);
        ApplyHandPaddle(handLeft, forward);
    }

    // 손이 전방 반대(뒤)로 움직이는 속도 성분만 추출해서 추력으로 쓴다 - 손을 뒤로 저으면 앞으로 나아간다.
    private void ApplyHandPaddle(Hand hand, Vector3 forward)
    {
        if (hand == null || hand.body == null)
            return;

        float backwardSpeed = -Vector3.Dot(hand.body.velocity, forward);

        if (backwardSpeed > paddleMinSpeed)
        {
            playerBody.AddForce(forward * backwardSpeed * paddleForce, ForceMode.Acceleration);
        }
    }
}
