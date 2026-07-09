using UnityEngine;

public class EnemyBulletHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [Range(0f, 1f)]
    public float minVisibleScale = 0.3f; // 체력이 거의 다 깎여도 아예 안 보이게 사라지진 않게 하는 최소 크기 비율

    private Vector3 originalScale;
    private Renderer cachedRenderer;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;
        originalScale = transform.localScale;

        cachedRenderer = GetComponentInChildren<Renderer>();
        if (cachedRenderer != null)
            originalColor = cachedRenderer.material.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateDamageVisual();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 맞을 때마다 남은 체력 비율만큼 크기를 줄이고 붉게 물들여서, 총알 체력이 깎이는 게 실시간으로 보이게 한다.
    private void UpdateDamageVisual()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        transform.localScale = originalScale * Mathf.Lerp(minVisibleScale, 1f, healthRatio);

        if (cachedRenderer != null)
            cachedRenderer.material.color = Color.Lerp(Color.red, originalColor, healthRatio);
    }
}
