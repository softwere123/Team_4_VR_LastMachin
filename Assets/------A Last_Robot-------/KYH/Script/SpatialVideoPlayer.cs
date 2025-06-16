using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class SpatialVideoPlayer : MonoBehaviour
{
    void Start()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        AudioSource audioSource = GetComponent<AudioSource>();

        // 비디오와 오디오 연결
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // 3D 사운드 설정
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 3f;
        audioSource.maxDistance = 15f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        // 자동 재생
        videoPlayer.Play();
        audioSource.Play();

        Debug.Log("공간감 있는 비디오 사운드가 재생됩니다.");
    }
}
