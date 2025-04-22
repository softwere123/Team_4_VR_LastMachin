using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAnimationSoundEvents_HG : MonoBehaviour
{
    [Header("🎧 상태별 애니메이션 사운드")]
    public AudioClip idleClip;       // 💤 Idle 상태 사운드
    public AudioClip walkClip;       // 🚶 걷기 상태 사운드
    public AudioClip runClip;        // 🏃 달리기 상태 사운드
    public AudioClip patrolClip;     // 👣 순찰 상태 사운드
    public AudioClip attackClip;     // 🔫 공격 상태 사운드
    public AudioClip reloadClip;     // 🔄 재장전 사운드
    public AudioClip dieClip;        // ☠️ 사망 사운드

    [Header("🎶 Chase 상태용 BGM")]
    public AudioClip bgmClip;        // 🎼 긴박한 상황에서 사용할 배경음악
    public bool loopBGM = true;      // 🔁 BGM 반복 재생 여부

    private AudioSource audioSource; // 🎵 효과음 재생용 오디오소스
    private AudioSource bgmSource;   // 🎼 BGM 재생 전용 오디오소스

    void Awake()
    {
        // 기본 효과음 재생용 AudioSource 가져오기
        audioSource = GetComponent<AudioSource>();

        // 별도 오브젝트에 BGM 재생용 AudioSource 추가
        GameObject bgmObj = new GameObject("BGM_AudioSource");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = loopBGM;
        bgmSource.volume = 0.5f; // 기본 볼륨 조절
    }

    // 💤 Idle 상태 사운드
    public void PlayIdleSound()
    {
        if (idleClip != null)
        {
            Debug.Log("💤 Idle 사운드 재생");
            audioSource.PlayOneShot(idleClip);
        }
    }

    // 🚶 걷기 상태 사운드
    public void PlayWalkSound()
    {
        if (walkClip != null)
        {
            Debug.Log("🚶 걷기 사운드 재생");
            audioSource.PlayOneShot(walkClip);
        }
    }

    // 🏃 뛰기 상태 사운드
    public void PlayRunSound()
    {
        if (runClip != null)
        {
            Debug.Log("🏃 뛰기 사운드 재생");
            audioSource.PlayOneShot(runClip);
        }
    }

    // 👣 순찰 상태 사운드
    public void PlayPatrolSound()
    {
        if (patrolClip != null)
        {
            Debug.Log("👣 순찰 사운드 재생");
            audioSource.PlayOneShot(patrolClip);
        }
    }

    // 🔫 공격 사운드
    public void PlayAttackSound()
    {
        if (attackClip != null)
        {
            Debug.Log("🔫 공격 사운드 재생");
            audioSource.PlayOneShot(attackClip);
        }
    }

    // 🔄 재장전 사운드
    public void PlayReloadSound()
    {
        if (reloadClip != null)
        {
            Debug.Log("🔄 리로드 사운드 재생");
            audioSource.PlayOneShot(reloadClip);
        }
    }

    // ☠️ 사망 사운드
    public void PlayDieSound()
    {
        if (dieClip != null)
        {
            Debug.Log("☠️ 죽음 사운드 재생");
            audioSource.PlayOneShot(dieClip);
        }
    }

    // ▶️ BGM 재생 함수 (애니메이션 이벤트에서 호출)
    public void PlayBGM()
    {
        if (bgmClip != null && !bgmSource.isPlaying)
        {
            Debug.Log("🎼 [BGM] Chase 애니메이션 시작 → BGM 재생");
            bgmSource.clip = bgmClip;
            bgmSource.loop = loopBGM;
            bgmSource.Play();
        }
    }

    // ⏹️ BGM 정지 함수 (애니메이션 이벤트에서 호출)
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            Debug.Log("⏹️ [BGM] Chase 애니메이션 종료 → BGM 정지");
            bgmSource.Stop();
        }
    }
}
