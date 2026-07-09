using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyed : MonoBehaviour
{

    private void Start()
    {
        PlayerHP = PlayerMax;
    }

    public int PlayerHP;
    public int PlayerMax = 100;

    public void PlayerDamaged(int Damage)
    {
        PlayerHP -= Damage;
        PlayerHP = Mathf.Max(PlayerHP, 0);

        if (PlayerHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        GameManager.instance.Restart.SetActive(true);
    }
    // Start is called before the first frame update

    // Update is called once per frame

}
