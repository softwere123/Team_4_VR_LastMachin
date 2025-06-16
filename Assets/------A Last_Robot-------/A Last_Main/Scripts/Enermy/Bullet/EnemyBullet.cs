using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public void DestroyAfterDelay()
    {
        Destroy(gameObject, 0.01f); // 1초 후 파괴
    }
}