using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 5f; // 총알 속도

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
