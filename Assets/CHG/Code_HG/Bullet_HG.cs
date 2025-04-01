using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // 총알 속도
    public int damage = 10; // 공격력
    public float lifetime = 3f; // 총알의 수명

    private Rigidbody rb; // Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 총알을 발사 방향으로 이동
        rb.velocity = transform.forward * speed;

        // 총알 수명 제한
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했는지 검사
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 PlayerHealth 컴포넌트를 가져오기
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // 플레이어 체력을 감소시킴
                playerHealth.TakeDamage(damage);
                Debug.Log("[Bullet] 플레이어가 총에 맞음! 현재 체력: " + playerHealth.GetCurrentHealth());
            }
        }

        // 충돌 후 총알 제거
        Destroy(gameObject);
    }
}
