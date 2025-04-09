using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public int damage = 10; // 투사체 공격력
    void Start()
    {
        Destroy(gameObject, 6f); // 3초 후에 오브젝트 삭제
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit != null) // Unit 클래스를 상속받은 오브젝트인지 확인
        {
            unit.TakeDamage(damage);
            Destroy(gameObject); // 투사체 제거
        }
    }
}
