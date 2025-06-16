using UnityEngine;
using UnityEngine.Video; // 추가

public class ZoneAudioTriggerPause : MonoBehaviour
{
    public AudioSource videoAudio;
    public VideoPlayer videoPlayer; // VideoPlayer 연결을 위한 필드

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.name == "AudioZone1")
        {
            videoAudio.mute = false;
            if (videoPlayer != null)
            {
                videoPlayer.Play(); // 영상 계속 재생
                Debug.Log("AudioZone1: 영상 소리 켜짐 + 영상 재생");
            }
        }
        else if (gameObject.name == "AudioZone2")
        {
            videoAudio.mute = true;
            if (videoPlayer != null)
            {
                videoPlayer.Pause(); // 영상 일시정지
                Debug.Log("AudioZone2: 영상 소리 꺼짐 + 영상 정지");
            }
        }
    }
}

