using UnityEngine;
using UnityEngine.AI;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("기본 설정")]
    public Transform player;                 // 플레이어 위치
    private NavMeshAgent agent;              // 네비메시 에이전트
    private Vector3 originPosition;          // 처음 위치 저장

    [Header("AI 거리 설정")]
    public float chaseRange = 10f;           // 추격 시작 거리
    public float attackRange = 5f;           // 공격 시작 거리
    public float returnRange = 15f;          // 돌아가기 시작 거리

    [Header("애니메이터")]
    public Animator anim;                    // 애니메이터 컴포넌트

    // 적 상태 정의
    private enum State { Idle, Chase, Attack, Return, Reload }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originPosition = transform.position;

        EnterIdleState(); // 게임 시작 시 Idle 상태로 진입
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distance <= chaseRange)
                    EnterChaseState();
                break;

            case State.Chase:
                if (distance <= attackRange)
                    EnterAttackState();
                else if (distance > returnRange)
                    EnterReturnState();
                else
                    ChasePlayer();
                break;

            case State.Attack:
                if (distance > attackRange)
                    EnterChaseState();
                break;

            case State.Return:
                if (Vector3.Distance(transform.position, originPosition) < 0.5f)
                    EnterIdleState();
                else
                    ReturnToOrigin();
                break;

            case State.Reload:
                // 필요 시 리로드 시간 딜레이 추가 가능
                break;
        }

        RotateTowards(player.position); // 항상 플레이어 방향을 바라봄
    }

    // ===================== 상태 전환 함수 =====================

    void EnterIdleState()
    {
        currentState = State.Idle;

        anim.SetBool("IsWalking", false);
        anim.SetBool("SeePlayer", false);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
        anim.SetBool("Die", false);

        // 🎧 사운드는 Animation Event에서 재생됨
    }

    void EnterChaseState()
    {
        currentState = State.Chase;

        anim.SetBool("IsWalking", true);
        anim.SetBool("SeePlayer", true);
        anim.SetBool("CanShoot", false);

        // 🎧 뛰는 소리: Animation Event에서 PlayRunSound() 호출
    }

    void EnterAttackState()
    {
        currentState = State.Attack;

        anim.SetBool("IsWalking", false);
        anim.SetBool("CanShoot", true);

        FireBullet(); // 총알 발사

        // 🎧 총소리: Animation Event에서 PlayAttackSound() 호출
    }

    void EnterReturnState()
    {
        currentState = State.Return;

        anim.SetBool("IsWalking", true);
        anim.SetBool("SeePlayer", false);

        // 🎧 걷는 소리: Animation Event에서 PlayWalkSound() 호출
    }

    void EnterReloadState()
    {
        currentState = State.Reload;

        anim.SetBool("Reload", true);

        // 🎧 리로드 사운드: Animation Event에서 PlayReloadSound() 호출
    }

    // ===================== AI 행동 =====================

    void ChasePlayer()
    {
        if (agent.enabled)
            agent.SetDestination(player.position);
    }

    void ReturnToOrigin()
    {
        if (agent.enabled)
            agent.SetDestination(originPosition);
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;

        if (direction.magnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void FireBullet()
    {
        Debug.Log("🔫 총알 발사 시도");

        // 총알 시스템이 있다면 여기서 Instantiate 등 처리
        // 사운드는 애니메이션 이벤트에서 처리됨
    }
}



