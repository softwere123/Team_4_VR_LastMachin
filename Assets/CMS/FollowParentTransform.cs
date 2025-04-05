using UnityEngine;

public class FollowParentTransform : MonoBehaviour
{
    public Transform parentTransform;

    void LateUpdate() // <- 업데이트 타이밍을 'LateUpdate'로!
    {
        if (parentTransform != null)
        {
            transform.position = parentTransform.position;
            transform.rotation = parentTransform.rotation;
           
        }
    }
}