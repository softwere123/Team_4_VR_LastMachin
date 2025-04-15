using UnityEngine;

public class LinearProjectile : MonoBehaviour
{
    public float speed = 5f; // 이동 속도

    void Update()
    {
        // X축으로 이동 (로컬 좌표 기준)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 만약 월드 좌표 기준으로 이동하려면:
        // transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}