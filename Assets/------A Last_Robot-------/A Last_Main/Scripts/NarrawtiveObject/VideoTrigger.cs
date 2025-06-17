using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    public Button playButton;
    public GameObject videoDisplay; // RawImage가 있는 오브젝트
    public VideoPlayer videoPlayer;

    void Start()
    {
        // 버튼 클릭 이벤트 등록
        playButton.onClick.AddListener(PlayVideo);

        // 처음엔 영상 비활성화
        videoDisplay.SetActive(false);
    }

    void PlayVideo()
    {
        videoDisplay.SetActive(true);
        videoPlayer.Play();
    }
}