using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
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

        // 비(非)키네마틱 리지드바디에만 작용하도록 확인
        Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;

        // 지정된 레이어에만 작용하도록 확인
        var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0) return;

        // 아래에 있는 오브젝트는 밀지 않음
        if (hit.moveDirection.y < -0.3f) return;

        // 이동 방향으로부터 밀 방향 계산, 수평 움직임만 포함
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        // 밀칠 힘을 적용하고 세기를 고려
        body.AddForce(pushDir * strength, ForceMode.Impulse);
	}
}