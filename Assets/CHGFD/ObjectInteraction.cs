using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectInteraction : MonoBehaviour
{
    public GameObject targetObject; // 활성화할 오브젝트
    public string nextSceneName; // 전환할 씬 이름
    private bool isPlayerNear = false; // 플레이어가 근처에 있는지 확인

    void Update()
    {
        // 플레이어가 근처에 있고, E 키를 누르면 실행
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            targetObject.SetActive(true); // 오브젝트 활성화
            Invoke("ChangeScene", 1f); // 1초 후 씬 전환 (연출을 위해 딜레이)
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName); // 씬 전환
    }

    // 플레이어가 범위 안에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌했는지 확인
        {
            isPlayerNear = true;
        }
    }

    // 플레이어가 범위 밖으로 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
