using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Y축으로의 이동 속도
    public float rotationSpeed = 50.0f; // 회전 속도
    private float direction = 1.0f; // Y축 이동 방향
    [SerializeField] private float minY = -5.0f; // Y축 최소값
    [SerializeField] private float maxY = 5.0f; // Y축 최대값

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
        RotateObject();
    }

    void MoveObject()
    {
        // Y축으로 위아래로 움직임
        transform.position += new Vector3(0, direction * moveSpeed * Time.deltaTime, 0);

        // Y축의 경계에 도달하면 방향 전환
        if (transform.position.y >= maxY || transform.position.y <= minY)
        {
            direction *= -1;
        }
    }

    void RotateObject()
    {
        // 불규칙한 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
