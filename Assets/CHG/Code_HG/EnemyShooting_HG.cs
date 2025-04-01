using UnityEngine;
using UnityEngine.AI; // NavMesh를 사용할 경우 필요
using System.Collections; // IEnumerator를 사용하려면 필요!

public class EnemyShooting_HG : MonoBehaviour
{
    public Transform player; // 플레이어 위치
    public float detectionRange = 15.0f; // 적이 플레이어를 감지할 거리
    public float fireRange = 10.0f; // 발사가 가능한 거리
    public float cooldownTime = 1.0f; // 발사 간격 (시간 간격)
    public int ammoCount = 5; // 총알 수
    public float reloadTime = 2.0f; // 재장전 시간

    private float lastFireTime; // 마지막 발사 시간
    private bool isReloading = false; // 재장전 상태 여부

    public Animator animator; // Animator 컨트롤러
    public Transform firePoint; // 총알이 발사될 시작 위치
    public GameObject bulletPrefab; // 총알 프리팹
    public ParticleSystem muzzleFlash; // 총구 파티클 (옵션)

    void Update()
    {
        if (isReloading) return; // 재장전 중에는 아무 동작도 하지 않음

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 감지 범위 안에 있을 때
        if (distanceToPlayer <= detectionRange)
        {
            // 플레이어를 향해 회전
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0f, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 플레이어가 발사 거리 내에 있을 때 총 쏘기
            if (distanceToPlayer <= fireRange && Time.time > lastFireTime + cooldownTime)
            {
                if (ammoCount > 0)
                {
                    Shoot();
                    lastFireTime = Time.time;
                    ammoCount--;
                }
                else
                {
                    StartCoroutine(Reload()); // 재장전 코루틴 호출
                }
            }

            // 애니메이터 SeePlayer 활성화
            animator.SetBool("SeePlayer", true);
        }
        else
        {
            // 플레이어를 감지하지 못할 때
            animator.SetBool("SeePlayer", false);
        }
    }

    void Shoot()
    {
        // 애니메이션 트리거 활성화
        animator.SetTrigger("Shoot");

        // 총구 파티클 실행
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // 총알 생성 및 발사
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * 20f, ForceMode.Impulse);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload"); // 재장전 애니메이션 호출

        yield return new WaitForSeconds(reloadTime); // 재장전 시간 동안 대기

        ammoCount = 5; // 총알 충전
        isReloading = false;
    }
}
