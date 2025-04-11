using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // 플레이어 최대 체력
    private int currentHealth; // 현재 체력

    void Start()
    {
        // 시작 시 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // 데미지를 받아 체력을 감소
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // 0 이하로 떨어지지 않게 제한

        Debug.Log("[PlayerHealth] 현재 체력: " + currentHealth);

        // 체력이 모두 소진되면 사망 처리
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public int GetCurrentHealth()
    {
        // 현재 체력 값을 반환
        return currentHealth;
    }

    void Die()
    {
        Debug.Log("[PlayerHealth] 플레이어가 사망했습니다!");
        // 죽음 처리 로직 (게임 오버, 리스폰)
    }
}
