using UnityEngine;
using UnityEngine.Rendering;

public class CaveDarkness : MonoBehaviour
{
    public Volume darkVolume;        // 어두운 효과 프로필
    public Transform player;         // 플레이어
    public Transform caveExitPoint;  // 동굴 출구 중심
    public float radius = 10f;       // 밝아지는 거리

    void Update()
    {
        float dist = Vector3.Distance(player.position, caveExitPoint.position);
        float t = Mathf.Clamp01(dist / radius);
        darkVolume.weight = 1 - t; // 출구에 가까울수록 어두움 → 밝음
    }
}
