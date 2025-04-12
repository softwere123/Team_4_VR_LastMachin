using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public HealthBar healthBar;
    void Start()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    public override void TakeDamage(int damage)
    {
        Debug.Log("플레이어가 " + damage + "의 데미지를 받았습니다.");

        base.TakeDamage(damage); // 부모 클래스의 TakeDamage 실행

        health = Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
            Debug.Log("체력 바 업데이트: " + health);
        }
    }



}
