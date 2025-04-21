using System.Collections;
using UnityEngine;

/// <summary>
/// 🎯 튜토리얼 적 AI
/// - 플레이어를 감지하면 사격
/// - 탄약 소진 시 장전
/// - 제자리에서 회전하며 공격
/// </summary>
public class VREnemyAI_Tutorial_HG : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform player;       // 🎯 플레이어 타겟

    [Header("전투 설정")]
    public float detectRange = 15f; // 감지 범위
    public float attackRange = 8f;  // 공격 범위
    public int maxAmmo = 7;         // 최대 탄약
    public float reloadTime = 2.5f; // 장전 시간

    private int currentAmmo;
    private bool isReloading = false;

    private Animator animator;
    private SGShotCtrl shotCtrl;

    void Start()
    {
        animator = GetComponent<Animator>();
        shotCtrl = GetComponent<SGShotCtrl>();
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            // 플레이어 방향으로 회전
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0; // 수평 회전만
            transform.forward = dir;

            if (distance <= attackRange)
            {
                animator.SetBool("isAttacking", true);
                Attack();
            }
            else
            {
                animator.SetBool("isAttacking", false);
                shotCtrl.Shooting = false;
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
            shotCtrl.Shooting = false;
        }
    }

    /// <summary>
    /// 🔫 총 발사
    /// </summary>
    void Attack()
    {
        if (!shotCtrl._shooting)
        {
            shotCtrl.Shooting = true;
            currentAmmo--;
            Debug.Log("🔥 발사됨! 남은 탄약: " + currentAmmo);

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    /// <summary>
    /// 🔄 장전 루틴
    /// </summary>
    IEnumerator Reload()
    {
        isReloading = true;
        shotCtrl.Shooting = false;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isReloading", true);

        Debug.Log("🔄 장전 중...");
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        animator.SetBool("isReloading", false);
        Debug.Log("✅ 장전 완료");
    }
}

