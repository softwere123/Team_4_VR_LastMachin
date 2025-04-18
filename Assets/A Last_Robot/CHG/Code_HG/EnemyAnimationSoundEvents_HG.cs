using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAnimationSoundEvents_HG : MonoBehaviour
{
    [Header("🎧 상태별 애니메이션 사운드")]
    public AudioClip idleClip;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip attackClip;
    public AudioClip reloadClip;
    public AudioClip dieClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayIdleSound()
    {
        if (idleClip != null)
        {
            Debug.Log("💤 Idle 사운드 재생");
            audioSource.PlayOneShot(idleClip);
        }
    }

    public void PlayWalkSound()
    {
        if (walkClip != null)
        {
            Debug.Log("🚶 걷기 사운드 재생");
            audioSource.PlayOneShot(walkClip);
        }
    }

    public void PlayRunSound()
    {
        if (runClip != null)
        {
            Debug.Log("🏃 뛰기 사운드 재생");
            audioSource.PlayOneShot(runClip);
        }
    }

    public void PlayAttackSound()
    {
        if (attackClip != null)
        {
            Debug.Log("🔫 공격 사운드 재생");
            audioSource.PlayOneShot(attackClip);
        }
    }

    public void PlayReloadSound()
    {
        if (reloadClip != null)
        {
            Debug.Log("🔄 리로드 사운드 재생");
            audioSource.PlayOneShot(reloadClip);
        }
    }

    public void PlayDieSound()
    {
        if (dieClip != null)
        {
            Debug.Log("☠️ 죽음 사운드 재생");
            audioSource.PlayOneShot(dieClip);
        }
    }
}
