using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string nextSceneName; // 전환할 씬의 이름

    private void OnDisable()
    {
        SceneManager.LoadScene(nextSceneName); // 오브젝트가 비활성화되면 씬 전환
    }
}