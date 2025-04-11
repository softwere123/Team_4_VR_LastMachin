using UnityEngine;

public class BasicRigidBodyPush_HG : MonoBehaviour
{
	public LayerMask pushLayers;
	public bool canPush;
	[Range(0.5f, 5f)] public float strength = 1.1f;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (canPush) PushRigidBodies(hit);
	}

	private void PushRigidBodies(ControllerColliderHit hit)
	{
        // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

        // 비키지 않는 리지드바디가 맞았는지 확인  
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        // 지정된 레이어의 오브젝트만 밀도록 설정  
        var bodyLayerMask = 1 << body.gameObject.layer;
        if ((bodyLayerMask & pushLayers.value) == 0) return;

        // 플레이어 아래에 있는 오브젝트는 밀지 않음  
        if (hit.moveDirection.y < -0.3f) return;

        // 이동 방향을 기반으로 밀기 방향 계산 (수평 방향만 고려)  
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        // 힘을 적용하여 오브젝트를 밀고, 강도를 반영  
        body.AddForce(pushDir * strength, ForceMode.Impulse);

    }
}