using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public Transform respawnPoint;

    public bool isRespawning = false; // 🔥 처음 시작인지 체크하는 플래그

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetSpawnPoint()
    {
        return respawnPoint != null ? respawnPoint.position : Vector3.zero;
    }
}
