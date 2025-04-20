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
    public float attackCooldown = 2f;        // 공격 쿨타임 (이제는 사용 안 함)
    private float lastAttackTime = 0f;       // 마지막 공격 시점 저장 (이제는 사용 안 함)

    [Header("탄약 설정")]
    public int maxAmmo = 10;                 // 탄창 크기
    private int currentAmmo;                 // 현재 남은 탄약 수

    [Header("애니메이터")]
    public Animator anim;                    // Animator 컴포넌트 참조

    private enum State { Idle, Chase, Attack, Return, Reload, Die } // AI 상태 정의
    private State currentState = State.Idle; // 현재 상태 초기화

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();             // NavMeshAgent 초기화
        anim = GetComponent<Animator>();                  // Animator 초기화
        originPosition = transform.position;              // 초기 위치 저장
        lastAttackTime = -attackCooldown;                 // 바로 공격 가능하게 초기화
        currentHealth = maxHealth;                        // 체력 초기화
        currentAmmo = maxAmmo;                            // 탄약 최대치로 설정
        EnterIdleState();                                 // 초기 상태 설정
    }

    void Update()
    {
        if (anim.GetBool("IsDead")) return;              // 죽었으면 처리 중단

        float distance = Vector3.Distance(player.position, transform.position); // 플레이어와 거리 측정

        switch (currentState)
        {
            case State.Idle:
                if (distance <= chaseRange)
                    EnterChaseState();                   // 추적 범위 이내면 추적 상태로 전환
                break;

            case State.Chase:
                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    EnterAttackState();                  // 공격 범위 이내면 공격 상태로 전환
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
                agent.isStopped = true;                  // 공격 중에는 멈춤
                break;                                   // 발사는 애니메이션 이벤트에서 처리됨

            case State.Return:
                float distanceToOrigin = Vector3.Distance(transform.position, originPosition);
                if (distanceToOrigin < 0.2f)
                {
                    anim.SetBool("IsWalking", false);
                    EnterIdleState();                    // 원래 자리로 돌아오면 Idle
                }
                else
                {
                    ReturnToOrigin();                    // 원래 자리로 복귀
                }
                break;

            case State.Reload:
                Debug.Log($"🔄 현재 상태: Reload | Reload: {anim.GetBool("Reload")} | IsReloading: {anim.GetBool("IsReloading")}");
                if (!anim.GetBool("Reload"))
                {
                    bool canSeePlayer = anim.GetBool("SeePlayer");
                    bool hasAmmo = anim.GetBool("HasAmmo");

                    if (canSeePlayer && hasAmmo)
                        EnterAttackState();              // 다시 공격
                    else if (canSeePlayer)
                        EnterChaseState();               // 다시 추적
                    else
                        EnterReturnState();              // 시야에서 사라졌으면 복귀
                }
                break;

            case State.Die:
                agent.isStopped = true;                  // 죽으면 멈춤
                break;
        }

        RotateTowards(player.position);                  // 항상 플레이어 쪽 바라보기
    }

    void EnterIdleState()
    {
        currentState = State.Idle;
        anim.SetBool("IsWalking", false);
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
        currentState = State.Attack;
        anim.SetBool("IsWalking", false);
        anim.SetBool("CanShoot", true);
        anim.SetTrigger("Shoot");                     // 애니메이션 트리거 발동 (이벤트로 FireBullet_Event() 호출됨)
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
        currentState = State.Reload;
        Debug.Log("🔁 EnterReloadState() 호출됨 → Reload 애니메이션 진입 시도");
        anim.SetBool("Reload", true);
        anim.SetBool("IsReloading", true);
    }

    void EnterDieState()
    {
        currentState = State.Die;
        anim.SetBool("IsDead", true);
        anim.SetBool("IsWalking", false);
        anim.SetBool("CanShoot", false);
        anim.SetBool("Reload", false);
        agent.isStopped = true;
    }

    // 🔥 애니메이션 이벤트에서 호출되는 함수
    public void FireBullet_Event()
    {
        if (currentAmmo > 0)
        {
            Debug.Log("🔥 애니메이션 이벤트로 총 발사!");

            // 총소리 재생 (AudioManager 사용 시)
            // AudioManager.Instance.Play("Enemy_Shot");

            // 실제 총알 발사 로직 실행
            FireBullet();

            // 탄약 차감
            currentAmmo--;

            // 애니메이터에 HasAmmo 상태 갱신
            anim.SetBool("HasAmmo", currentAmmo > 0);

            // 탄약 다 떨어지면 리로드 상태 진입
            if (currentAmmo == 0)
            {
                Debug.Log("🔁 탄약 0 → Reload 상태 전이");
                EnterReloadState();
            }
        }
    }

    void FireBullet()
    {
        Debug.Log("🔫 FireBullet() 호출됨 - 총알 생성 처리 위치");
        // 실제 총알 Instantiate 또는 SGShotCtrl 호출 처리 위치
    }

    void ChasePlayer()
    {
        if (agent.enabled)
            agent.SetDestination(player.position);  // 플레이어 위치로 이동
    }

    void ReturnToOrigin()
    {
        if (agent.enabled)
            agent.SetDestination(originPosition);   // 원래 위치로 이동
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized; // 방향 벡터 계산
        direction.y = 0f; // 수직 축 회전 방지

        if (direction.magnitude > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction); // 바라볼 방향 계산
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 부드럽게 회전
        }
    }

    [Header("체력 설정")]
    public int maxHealth = 100;         // 최대 체력
    private int currentHealth;          // 현재 체력

    public void TakeDamage(int damage)
    {
        if (anim.GetBool("IsDead")) return; // 이미 죽었으면 무시

        currentHealth -= damage;            // 데미지 적용

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EnterDieState();               // 체력이 0 이하되면 사망 상태 전이
        }
    }

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);              // 애니메이션 이벤트에서 호출됨
    }

    public void OnReloadComplete()
    {
        Debug.Log("✅ OnReloadComplete() 호출됨 → 탄약 재장전 완료");

        anim.SetBool("Reload", false);     // Reload 상태 해제
        anim.SetBool("IsReloading", false);

        currentAmmo = maxAmmo;             // 탄약 최대치로 복원
        anim.SetBool("HasAmmo", true);    // 애니메이터에 탄약 있음 표시
    }

    public int GetCurrentHealth()
    {
        return currentHealth;             // 외부에서 체력 조회용
    }
}





