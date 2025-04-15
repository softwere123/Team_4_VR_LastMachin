using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth_HG : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Transform respawnTarget; // ✅ 여기에 리스폰 위치 지정

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("[PlayerHealth] 현재 체력: " + currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("[PlayerHealth] 플레이어가 사망했습니다!");

        // 리스폰 플래그 활성화
        SpawnManager.Instance.isRespawning = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

