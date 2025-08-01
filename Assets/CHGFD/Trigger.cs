using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{
    public string nextSceneName; // 넘어갈 씬 이름

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 닿았을 때만 실행
        {
            SceneManager.LoadScene(nextSceneName); // 다음 씬으로 전환
        }
    }
}