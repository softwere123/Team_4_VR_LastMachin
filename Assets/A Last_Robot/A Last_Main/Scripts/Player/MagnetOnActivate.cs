using UnityEngine;
using Autohand;

// "Sphere (1)"처럼 손 스퀴즈로 SetActive(true/false) 펄스되는 오브젝트에 붙인다.
// 이 오브젝트가 켜지는 순간(OnEnable) 플레이어도 같이 자석에 끌려가게 하고,
// 꺼지는 순간(OnDisable) 당김을 멈춘다. 기존 On Squeeze/On Unsqueeze 이벤트 배선은
// 그대로 두고, SetActive가 트리거하는 Unity 생명주기 콜백만 이용한다.
//
// 비행 모드일 때만 당기는 이유: AutoHandPlayer.groundedDrag가 바닥에 서있을 때
// 매 FixedUpdate마다 속도를 거의 0으로 눌러버려서, MagneticSource의 AddForce와
// 충돌해 떨리거나(우다다) 붕 뜨는 문제가 생긴다. 비행 중엔 grounded 판정 자체가
// 없어서 이 문제가 애초에 발생하지 않는다.
public class MagnetOnActivate : MonoBehaviour
{
    public MagneticSource pullSource; // 비워두면 이 오브젝트의 MagneticSource를 자동으로 사용
    public float pullStrength = 10f;

    private void Awake()
    {
        if (pullSource == null)
            pullSource = GetComponent<MagneticSource>();
    }

    private void OnEnable()
    {
        UpdatePull();
    }

    private void OnDisable()
    {
        if (pullSource != null)
            pullSource.strength = 0f;
    }

    private void Update()
    {
        // 켜져있는 동안, 비행 모드가 중간에 켜지거나 꺼져도 즉시 반영되도록 계속 확인한다.
        UpdatePull();
    }

    private void UpdatePull()
    {
        if (pullSource == null)
            return;

        bool isFlying = AutoHandPlayer.Instance != null && !AutoHandPlayer.Instance.useGrounding;
        pullSource.strength = isFlying ? pullStrength : 0f;
    }
}
