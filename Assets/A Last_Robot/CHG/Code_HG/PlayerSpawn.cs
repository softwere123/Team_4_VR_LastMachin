using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        // SpawnManager가 존재하고, 리스폰 상태일 경우만 위치 이동
        if (SpawnManager.Instance != null && SpawnManager.Instance.isRespawning)
        {
            transform.position = SpawnManager.Instance.GetSpawnPoint();

            // 다음 시작은 일반 시작으로 되돌림
            SpawnManager.Instance.isRespawning = false;
        }
        // else → 처음 시작이므로 위치 이동 없이 에디터에서 배치된 위치로 시작
    }
}
