using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string nextSceneName; // 이동할 씬 이름
    public Vector3 spawnPosition; // 스폰될 위치
    private bool isPlayerInTrigger = false; // 플레이어가 범위 안에 있는지 확인

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true; // 플레이어가 범위 안에 들어왔음을 저장
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false; // 범위 밖으로 나가면 false로 변경
        }
    }

    private void Update()
    {
        // 플레이어가 트리거 안에 있고, E 키를 누르면 씬 이동
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            SpawnManager.Instance.SetSpawnPosition(nextSceneName, spawnPosition); // 다음 씬 스폰 위치 저장
            SceneManager.LoadScene(nextSceneName); // 씬 변경
        }
    }
}
