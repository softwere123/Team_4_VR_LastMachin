using UnityEngine;

public class EnemyBulletImpact : MonoBehaviour
{
    public int damage = 40;

    private void OnTriggerEnter(Collider other)
    {
        BulletDestroyed playerHealth = other.GetComponent<BulletDestroyed>();
        if (playerHealth != null)
        {
            playerHealth.PlayerDamaged(damage);
            Destroy(gameObject);
        }
    }
}
