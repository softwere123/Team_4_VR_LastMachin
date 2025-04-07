using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도

    public int PlayerHP;
    public int PlayerMax = 100;

    private void Start()
    {
        PlayerHP = PlayerMax;
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f ).normalized * moveSpeed;

        // Rigidbody를 사용하여 움직임
        // Rigidbody가 없으면 Transform을 사용할 수도 있습니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = movement;
    }

    /*public void PlayerDamaged(int Damage)
    {
        PlayerHP -= Damage;

        if( PlayerHP < 0 )
        {
            Die();
        }
    }

    public void Die()
    {
       this.gameObject.SetActive( false );
        Debug.Log("실행1");
        
     
    }*/
}
