using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Vector3 spawnPosition = new Vector3(0, 1, 0); // 원하는 위치 설정

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 찾기
        if (player != null)
        {
            player.transform.position = spawnPosition; // 위치 변경
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없음!");
        }
    }
}






