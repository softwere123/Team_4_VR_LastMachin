using UnityEngine;

public class PlayerBulletImpact : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        EnemyBulletHealth enemyBullet = other.GetComponent<EnemyBulletHealth>();
        if (enemyBullet != null)
        {
            enemyBullet.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        EnemyHealth_HG enemyHealth = other.GetComponentInParent<EnemyHealth_HG>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
