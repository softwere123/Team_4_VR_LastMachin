using System.Collections.Generic;
using UnityEngine;
using Autohand;
using System;

public class SetElemental : MonoBehaviour
{
    public List<DistanceGrabbable> magneticObjects; // 마그네틱: DistanceGrabbable 리스트
    public List<Grabbable> fireObjects;             // 파이어: Grabbable 리스트
    public bool isMagnetic = false;
    public bool isFire = false;

    [Header("자석 당김 (플레이어를 손 쪽으로 끌어당기는 MagneticSource)")]
    public MagneticSource magnetPullSource; // 오른손 자석 포인트에 붙인 MagneticSource (예: RobotHand(R)/GameObjectm)
    public float magnetPullStrength = 10f;  // 자석 모드 + 비행 중일 때 실제로 적용할 힘

    [Header("손 허우적(패들) 전진 - 마그네틱일 때만, 1차 실험")]
    public bool usePaddleMovement = true;
    public float paddleForce = 5f;     // 패들 추진력 배수
    public float paddleMinSpeed = 1f;  // 이 속도 이상으로 손을 뒤로 저어야 추진력이 붙음

    // AutoHandPlayer.groundedDrag(기본 10000)는 바닥에 서있을 때 매 FixedUpdate마다
    // 속도를 거의 0으로 눌러버려서, MagneticSource의 AddForce와 충돌해 "우다다" 떨리거나
    // 붕 뜨는 부작용이 생긴다. 그 값을 건드리는 대신, 자석 당김은 비행 모드(AutoHandPlayer의
    // useGrounding이 꺼진 상태)일 때만 켜지도록 해서 애초에 groundedDrag와 안 겹치게 한다.
    private void Update()
    {
        SetMagnetPull(isMagnetic);
        UpdatePaddleMovement();
    }

    public void SetType(int index)
    {
        Debug.Log($"SetType 호출됨, index: {index}");

        isMagnetic = (index == 1);
        isFire = (index == 2);

        SetMagneticObjects(isMagnetic);
        SetGrabbable(fireObjects, isFire);
        SetFlying(isMagnetic);
    }

    // 마그네틱 원소를 선택하면 비행 모드로 전환하고, 다른 원소를 선택하면 다시 보행 모드로 되돌린다.
    private void SetFlying(bool flying)
    {
        var player = AutoHandPlayer.Instance;
        if (player == null)
            return;

        player.useGrounding = !flying;
        if (player.body != null)
            player.body.useGravity = !flying;
    }

    private void SetMagnetPull(bool active)
    {
        if (magnetPullSource == null)
            return;

        bool isFlying = AutoHandPlayer.Instance != null && !AutoHandPlayer.Instance.useGrounding;

        // strength만 0으로 낮춰서, 트리거 감지(OnTriggerEnter/Exit)는 계속 살아있게 유지한다.
        // MagneticSource 자체를 enabled=false로 끄면 이미 범위 안에 있던 대상은
        // 재진입 전까지 다시 등록되지 않아, 자석을 켠 순간 바로 안 끌려가는 문제가 생긴다.
        magnetPullSource.strength = (active && isFlying) ? magnetPullStrength : 0f;
    }

    // 마그네틱 모드에서 손을 뒤로 "허우적(패들)"거리면 그 반대 방향(앞)으로 나아가는 1차 실험 로직.
    // 정교한 수영 동작 판정이 아니라, 손의 속도를 몸 전방축에 투영해서 뒤로 젓는 성분만 추력으로 쓴다.
    private void UpdatePaddleMovement()
    {
        if (!usePaddleMovement || !isMagnetic)
            return;

        var player = AutoHandPlayer.Instance;
        if (player == null || player.body == null)
            return;

        Vector3 forward = player.forwardFollow != null ? player.forwardFollow.forward : player.transform.forward;

        ApplyHandPaddle(player.handRight, forward, player.body);
        ApplyHandPaddle(player.handLeft, forward, player.body);
    }

    private void ApplyHandPaddle(Hand hand, Vector3 forward, Rigidbody playerBody)
    {
        if (hand == null || hand.body == null)
            return;

        // 손이 전방 반대(뒤)로 움직이는 속도 성분만 추출 - 손을 뒤로 저으면 앞으로 나아간다.
        float backwardSpeed = -Vector3.Dot(hand.body.velocity, forward);

        if (backwardSpeed > paddleMinSpeed)
        {
            playerBody.AddForce(forward * backwardSpeed * paddleForce, ForceMode.Acceleration);
        }
    }

    private void SetMagneticObjects(bool enable)
    {
        foreach (var distanceGrab in magneticObjects)
        {
            distanceGrab.enabled = enable;
            Debug.Log($"Magnetic Object '{distanceGrab.gameObject.name}' set to {(enable ? "ENABLED" : "DISABLED")}");
        }
    }

    private void SetGrabbable(List<Grabbable> grabbables, bool enable)
    {
        foreach (var grab in grabbables)
        {
            grab.enabled = enable;
            Debug.Log($"Fire Object '{grab.gameObject.name}' set to {(enable ? "ENABLED" : "DISABLED")}");
        }
    }
}