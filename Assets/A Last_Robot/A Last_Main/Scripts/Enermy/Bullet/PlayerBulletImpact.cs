using UnityEngine;
using UnityEngine.AI;

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
        if (enemyHealth == null)
            enemyHealth = TryAttachEnemyHealth(other);

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    // 실제 적 프리팹(PolyartCharacter-Guard/Sentry 등)에는 EnemyHealth_HG가 아직 붙어있지 않은
    // 경우가 많아서 데미지가 그냥 무시된다. Animator+NavMeshAgent를 이미 갖춘 적(EnemyHealth_HG의
    // RequireComponent 조건)이면 여기서 자동으로 붙여준다. 이미 조건을 만족하는 대상에만 붙이므로
    // RequireComponent가 NavMeshAgent 같은 컴포넌트를 새로 끼워넣는 부작용은 없다.
    private EnemyHealth_HG TryAttachEnemyHealth(Collider other)
    {
        var animator = other.GetComponentInParent<Animator>();
        var agent = other.GetComponentInParent<NavMeshAgent>();

        if (animator == null || agent == null || animator.gameObject != agent.gameObject)
            return null;

        return animator.gameObject.AddComponent<EnemyHealth_HG>();
    }
}
