using UnityEngine;

public class SpecificColliderDestroyer : MonoBehaviour
{
    [Tooltip("충돌을 감지할 대상 오브젝트 A")]
    public GameObject colliderA;

    [Tooltip("충돌을 감지할 대상 오브젝트 B")]
    public GameObject colliderB;

    [Tooltip("충돌 시 삭제할 오브젝트 (colliderA 또는 colliderB 중 하나 지정)")]
    public GameObject objectToDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        // colliderA와 colliderB가 서로 충돌했을 때만 실행
        if ((collision.gameObject == colliderA && this.gameObject == colliderB) ||
            (collision.gameObject == colliderB && this.gameObject == colliderA))
        {
            if (objectToDestroy != null)
            {
                Destroy(objectToDestroy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // colliderA와 colliderB가 서로 Trigger로 겹쳤을 때만 실행
        if ((other.gameObject == colliderA && this.gameObject == colliderB) ||
            (other.gameObject == colliderB && this.gameObject == colliderA))
        {
            if (objectToDestroy != null)
            {
                Destroy(objectToDestroy);
            }
        }
    }
}
