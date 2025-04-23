using UnityEngine;
using UnityEngine.UI;

public class StartButtonHandler : MonoBehaviour
{
    public Button startButton;         // UI 버튼
    public GameObject targetObject;    // 클릭 후 비활성화할 오브젝트
    public AudioSource audioSource;    // 사운드 재생용
    public GameObject uiCanvas;        // 끌 UI 캔버스

    private bool hasPlayed = false;    // 사운드 재생 여부 체크

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        // targetObject는 처음부터 활성화되어 있어야 하므로 SetActive(false) 제거
    }

    public void OnStartClicked()
    {
        if (!hasPlayed && audioSource != null)
        {
            audioSource.Play();       // 사운드 1회 재생
            hasPlayed = true;
        }

        if (targetObject != null)
            targetObject.SetActive(false); // 오브젝트 비활성화

        if (uiCanvas != null)
            uiCanvas.SetActive(false);    // UI 캔버스 끄기
    }
}