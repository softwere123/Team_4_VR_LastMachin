using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 spawnPos = SpawnManager.Instance.GetSpawnPosition(currentScene);
        transform.position = spawnPos; // 씬에 맞는 위치에서 스폰
    }
}
