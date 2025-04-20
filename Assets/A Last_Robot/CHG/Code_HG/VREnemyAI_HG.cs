using UnityEngine;
using UnityEngine.AI;

public class VREnemyAI_HG : MonoBehaviour
{
    [Header("기본 설정")]
    public Transform player;                 // 추적할 플레이어 위치
    private NavMeshAgent agent;              // NavMeshAgent 컴포넌트 참조
    private Vector3 originPosition;          // 적의 초기 위치 저장

    [Header("AI 거리 설정")]
    public float chaseRange = 10f;           // 추적을 시작하는 거리
    public float attackRange = 5f;           // 공격 가능한 거리
    public float returnRange = 15f;          // 복귀를 시작하는 거리 기준

    [Header("공격 설정")]
    public float attackCooldown = 2f;        // 공격 쿨타임 (현재 사용 안 함)
    private float lastAttackTime = 0f;       // 마지막 공격 시점 저장 (현재 사용 안 함)

    [Header("탄약 설정")]
    public int maxAmmo = 10;                 // 탄창 크기 설정
    private int currentAmmo;                 // 현재 남아있는 탄약 수

    [Header("애니메이터")]
    public Animator anim;                    // Animator 컴포넌트 참조

    private enum State { Idle, Chase, Attack, Return, Reload, Die } // 적의 상태를 정의
    private State currentState = State.Idle; // 현재 상태 초기화

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();             // NavMeshAgent 초기화
        anim = GetComponent<Animator>();                  // Animator 초기화
        originPosition = transform.position;              // 적의 시작 위치 저장
        lastAttackTime = -attackCooldown;                 // 공격 바로 가능하게 초기화
        currentHealth = maxHealth;                        // 체력 초기화
        currentAmmo = maxAmmo;                            // 탄약을 최대치로 설정
        EnterIdleState();                                 // 초기 상태로 Idle 설정
    }

    void Update()
    {
        if (anim.GetBool("IsDead")) return;              // 죽었으면 아무것도 하지 않음

        float distance = Vector3.Distance(player.position, transform.position); // 플레이어와의 거리 계산

        switch (currentState)
        {
            case State.Idle:
                if (distance <= chaseRange)
                    EnterChaseState();                   // 플레이어가 가까우면 추적 상태로 전환
                break;

            case State.Chase:
                if (distance <= attackRange)
                {
                    agent.isStopped = true;              // 공격 거리 도달 시 멈추고
                    EnterAttackState();                  // 공격 상태로 전환
                }
                else if (distance > returnRange)
                {
                    agent.isStopped = false;
                    EnterReturnState();                  // 너무 멀어지면 복귀 상태로 전환
                }
                else
                {
                    agent.isStopped = false;
                    ChasePlayer();                       // 계속 추적
                }
                break;

            case State.Attack:
                agent.isStopped = true;                  // 공격 중에는 이동 멈춤
                break;                                   // 공격 로직은 애니메이션 이벤트에서 처리

            case State.Return:
                float distanceToOrigin = Vector3.Distance(transform.position, originPosition);
                if (distanceToOrigin < 0.2f)
                {
                    anim.SetBool("IsWalking", false);
                    EnterIdleState();                    // 복귀 완료하면 Idle 상태로 전환
                }
                else
                {
                    ReturnToOrigin();                    // 복귀 진행 중
                }
                break;

            case State.Reload:
                Debug.Log($"🔄 현재 상태: Reload | Reload: {anim.GetBool("Reload")} | IsReloading: {anim.GetBool("IsReloading")}");
                if (!anim.GetBool("Reload"))
                {
                    bool canSeePlayer = anim.GetBool("SeePlayer");
                    bool hasAmmo = anim.GetBool("HasAmmo");

                    if (canSeePlayer && hasAmmo)
                        EnterAttackState();              // 다시 공격 상태로
                    else if (canSeePlayer)
                        EnterChaseState();               // 다시 추적 상태로
                    else
                        EnterReturnState();              // 보이지 않으면 복귀 상태로
                }
                break;

            case State.Die:
                agent.isStopped = true;                  // 죽으면 이동 정지
                break;
        }

        RotateTowards(player.position);                  // 항상 플레이어 쪽을 바라보게 회전
    }

    void EnterIdleState()
    {
        currentState = State.Idle;                       // 상태 변경
        anim.SetBool("IsWalking", false);               // 애니메이터 값 설정
        anim.SetBool("SeePlayer", false);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
    }

    void EnterChaseState()
    {
        currentState = State.Chase;
        anim.SetBool("IsWalking", true);
        anim.SetBool("SeePlayer", true);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
    }

    void EnterAttackState()
    {
        if (currentState == State.Attack)
        {
            anim.ResetTrigger("Shoot");                 // 트리거 초기화 (반복 발사 지원)
            anim.SetTrigger("Shoot");                   // 트리거 재설정 → 애니메이션 반복 발동 가능
            Debug.Log("🔁 Attack 상태 반복 실행 중 → 애니메이션 재트리거");
            return;
        }

        currentState = State.Attack;                     // 상태 변경
        anim.SetBool("IsWalking", false);
        anim.SetBool("CanShoot", true);
        anim.SetTrigger("Shoot");                       // 공격 애니메이션 실행
        anim.SetBool("Reload", false);
    }

    void EnterReturnState()
    {
        currentState = State.Return;
        anim.SetBool("IsWalking", true);
        anim.SetBool("SeePlayer", false);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
    }

    void EnterReloadState()
    {
        currentState = State.Reload;                     // 상태 변경
        Debug.Log("🔁 EnterReloadState() 호출됨 → Reload 애니메이션 진입 시도");
        anim.SetBool("Reload", true);                   // 애니메이션 상태값 변경
        anim.SetBool("IsReloading", true);
    }

    void EnterDieState()
    {
        currentState = State.Die;                        // 상태 변경
        anim.SetBool("IsDead", true);
        anim.SetBool("IsWalking", false);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
        agent.isStopped = true;                          // 이동 정지
    }

    // 🔥 애니메이션 이벤트에서 호출되는 총 발사 처리 함수
    public void FireBullet_Event()
    {
        if (currentAmmo > 0)
        {
            Debug.Log("🔥 애니메이션 이벤트로 총 발사!");

            // 총소리 재생 (AudioManager 사용 시)
            // AudioManager.Instance.Play("Enemy_Shot");

            FireBullet();                                // 실제 총알 발사 처리
            currentAmmo--;                               // 탄약 차감
            anim.SetBool("HasAmmo", currentAmmo > 0);   // 탄약 유무 반영

            if (currentAmmo == 0)
            {
                Debug.Log("🔁 탄약 0 → Reload 상태 전이");
                EnterReloadState();                      // 탄약 없으면 리로드 상태 진입
            }
        }
    }

    void FireBullet()
    {
        Debug.Log("🔫 FireBullet() 호출됨 - 총알 생성 처리 위치");
        // 총알 Instantiate 또는 탄막 시스템 실행 위치
    }

    void ChasePlayer()
    {
        if (agent.enabled)
            agent.SetDestination(player.position);      // 플레이어 위치로 이동 지시
    }

    void ReturnToOrigin()
    {
        if (agent.enabled)
            agent.SetDestination(originPosition);       // 시작 지점으로 복귀 지시
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized; // 방향 벡터 계산
        direction.y = 0f; // y축 회전 제거 (수평 회전만 적용)

        if (direction.magnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction); // 회전 방향 계산
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 부드럽게 회전 적용
        }
    }

    [Header("체력 설정")]
    public int maxHealth = 100;             // 최대 체력
    private int currentHealth;              // 현재 체력

    public void TakeDamage(int damage)
    {
        if (anim.GetBool("IsDead")) return; // 이미 죽은 상태면 무시

        currentHealth -= damage;            // 데미지 적용

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EnterDieState();               // 체력 0 이하 → 사망 처리
        }
    }

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);              // 사망 애니메이션 끝나면 오브젝트 삭제
    }

    public void OnReloadComplete()
    {
        Debug.Log("✅ OnReloadComplete() 호출됨 → 탄약 재장전 완료");

        anim.SetBool("Reload", false);     // 리로드 상태 해제
        anim.SetBool("IsReloading", false);

        currentAmmo = maxAmmo;             // 탄약 최대치로 복원
        anim.SetBool("HasAmmo", true);    // 애니메이터 상태 반영
    }

    public int GetCurrentHealth()
    {
        return currentHealth;             // 외부에서 현재 체력 조회용
    }
}






