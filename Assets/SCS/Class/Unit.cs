using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string UnitName = "UnitName";
    public int maxHealth = 100; // 체력
    public int health;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log(UnitName + "이(가) 쓰러졌습니다.");
        Destroy(gameObject);
    }

}