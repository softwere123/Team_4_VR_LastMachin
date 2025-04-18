using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioManager_HG : MonoBehaviour
{
    [Header("🎧 오디오 클립들")]
    public AudioClip shootClip;
    public AudioClip idleClip;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip reloadClip;

    [Header("⚙️ 설정")]
    public float idleSoundInterval = 4f;

    private AudioSource audioSource;
    private Coroutine idleRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // 3D 사운드 설정
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 500f;

        Debug.Log("✅ AudioSource 초기화 완료! spatialBlend = " + audioSource.spatialBlend);
    }

    void Update()
    {
        // 🔁 테스트용 키: T 누르면 총소리 재생
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("🎧 테스트: T 키 입력 감지됨 → 총소리 재생 시도");
            PlayShootSound();
        }
    }

    // 🔫 총소리 재생
    public void PlayShootSound()
    {
        Debug.Log("🧨 PlayShootSound() 실행됨!");

        if (shootClip != null)
        {
            Debug.Log("✅ shootClip 존재! → 오디오 재생 시도");
            audioSource.PlayOneShot(shootClip);
        }
        else
        {
            Debug.LogWarning("❌ shootClip이 null입니다! 사운드 재생 실패");
        }
    }

    // 🚶 걷기
    public void PlayWalkSound()
    {
        Debug.Log("👣 걷기 사운드 호출");
        if (walkClip != null)
            audioSource.PlayOneShot(walkClip);
    }

    // 🏃 뛰기
    public void PlayRunSound()
    {
        Debug.Log("🏃 뛰기 사운드 호출");
        if (runClip != null)
            audioSource.PlayOneShot(runClip);
    }

    // 🔄 리로드
    public void PlayReloadSound()
    {
        Debug.Log("🔁 리로드 사운드 호출");
        if (reloadClip != null)
            audioSource.PlayOneShot(reloadClip);
    }

    // 🌀 Idle 루프 시작
    public void StartIdleSoundLoop()
    {
        Debug.Log("💤 Idle 루프 시작 시도");

        if (idleClip != null && idleRoutine == null)
        {
            idleRoutine = StartCoroutine(IdleSoundLoop());
            Debug.Log("💤 Idle 사운드 루프 코루틴 시작됨");
        }
        else if (idleClip == null)
        {
            Debug.LogWarning("❌ idleClip이 null입니다! Idle 사운드 재생 불가");
        }
    }

    // ❌ Idle 루프 종료
    public void StopIdleSoundLoop()
    {
        Debug.Log("🛑 Idle 루프 정지 요청");

        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
            Debug.Log("🛑 Idle 코루틴 중지 완료");
        }
    }

    // 🔁 Idle 반복 재생 코루틴
    private IEnumerator IdleSoundLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleSoundInterval);
            Debug.Log("💤 Idle 루프 재생");
            audioSource.PlayOneShot(idleClip);
        }
    }
}


