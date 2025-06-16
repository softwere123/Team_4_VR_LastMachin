using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHealth_HG : MonoBehaviour
{
    [Header("💔 체력 설정")]
    public int maxHealth = 100;          // 최대 체력
    private int currentHealth;           // 현재 체력

    [Header("🧠 컴포넌트 참조")]
    private Animator anim;               // 애니메이터
    private NavMeshAgent agent;          // 이동 에이전트
    private bool isDead = false;         // 사망 여부

    void Start()
    {
        currentHealth = maxHealth;       // 시작 시 풀 체력
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 외부에서 데미지를 받을 때 호출
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 이미 죽은 경우 무시
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// 죽음 처리 함수 (체력 0 이하)
    /// </summary>
    void Die()
    {
        isDead = true;

        // Animator 파라미터로 죽음 전이 유도
        anim.SetBool("IsDead", true);

        // 이동 중지
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
        }

        // 공격, 리로드, 걷기 중단
        anim.SetBool("CanShoot", false);
        anim.SetBool("IsWalking", false);
        anim.SetBool("Reload", false);
        anim.SetBool("SeePlayer", false);

        // 이 스크립트 또는 AI 스크립트 비활성화 (선택)
        // this.enabled = false;
    }

    /// <summary>
    /// 죽음 애니메이션 끝에서 호출할 이벤트 함수
    /// </summary>
    public void OnDieAnimationEnd()
    {
        // 죽음 애니메이션 끝나면 오브젝트 삭제
        Destroy(gameObject); // 또는 SetActive(false) 등 선택 가능
    }

    /// <summary>
    /// 현재 체력 반환 (디버그용)
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
