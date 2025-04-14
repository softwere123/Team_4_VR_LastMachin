using UnityEngine;

public class HologramVisibility : MonoBehaviour
{
    public Transform hologram;  // 홀로그램 오브젝트 (영상)
    public Transform player;    // 플레이어
    public Camera playerCamera; // 플레이어 카메라
    public float maxDistance = 10f;  // 플레이어가 볼 수 있는 최대 거리
    public float detectionAngle = 30f; // 플레이어가 볼 수 있는 시야각 (도 단위)
    public Renderer hologramRenderer;  // 홀로그램의 렌더러 (영상이 있는 게임 오브젝트)
    public float fadeSpeed = 1f; // 투명해지는 속도

    private void Update()
    {
        // 플레이어와 홀로그램 간의 거리 계산
        float distance = Vector3.Distance(player.position, hologram.position);

        // 플레이어가 홀로그램을 바라보고 있는지 확인 (각도)
        Vector3 toHologram = hologram.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, toHologram);

        // 영상 보이기/숨기기 (알파 값 조정)
        if (distance <= maxDistance && angle <= detectionAngle)
        {
            // 영상이 보이도록 설정 (알파 값을 1로 서서히 증가)
            FadeIn();
        }
        else
        {
            // 영상이 보이지 않도록 설정 (알파 값을 0으로 서서히 감소)
            FadeOut();
        }
    }

    // 영상이 서서히 보이도록 알파 값을 증가시키기
    private void FadeIn()
    {
        Color color = hologramRenderer.material.color;
        color.a = Mathf.Lerp(color.a, 1f, fadeSpeed * Time.deltaTime);
        hologramRenderer.material.color = color;
    }

    // 영상이 서서히 투명하게 만들기 (알파 값을 감소시키기)
    private void FadeOut()
    {
        Color color = hologramRenderer.material.color;
        color.a = Mathf.Lerp(color.a, 0f, fadeSpeed * Time.deltaTime);
        hologramRenderer.material.color = color;
    }
}
