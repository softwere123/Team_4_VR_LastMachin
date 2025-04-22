using UnityEngine;

public class DetectionZone_HG : MonoBehaviour
{
    [Header("감지 범위 (시각화 용도)")]
    public Vector3 gizmoSize = new Vector3(5f, 3f, 10f);

    [Header("플레이어가 감지 구역 안에 있는지 여부")]
    public bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🟢 플레이어 DetectionZone 진입");
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🔴 플레이어 DetectionZone 이탈");
            playerInside = false;
        }
    }

    // ✅ 인스펙터에서 설정한 gizmoSize로 Gizmo 크기 조절 가능
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = playerInside ? Color.green : Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, gizmoSize);
    }
}


