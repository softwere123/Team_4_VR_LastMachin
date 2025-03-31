using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 코드 알고리즘 세줄 요약:
// 이 코드는 MeteoPistor_CMS와 상호작용하는 클래스로 
// MeteorPistor_CMS에서 발사되는 레이저가 부딪히는 오브젝트를 찾아서 Break 메소드를 호출한다.
// 호출 조건식으로는 1. 2초간 레이캐스트를 지속시켜야 발동하며 2. 호출시 상위컴퍼넌트 비횔성화& 하위 컴포넌트 활성화 .

// 실제 보여지는 기능 요약: 돌이나 물체들을 총으로 부수는 기능을 구현한 클래스

// 보완점:사운드는 미포함 타임라인 이벤트로 처리 예정
public class Breakable_CMS : MonoBehaviour
{
    public List<GameObject> breakablepiecces;   // 
    public float timeToBraek = 2;
    public float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach ( var item in breakablepiecces)
        {
            item.SetActive(false);
        }

    }

    public void Break()
    {
        timer += Time.deltaTime;

        if (timer > timeToBraek)
        {
            foreach (var item in breakablepiecces)
            {
                item.SetActive(true);
                item.transform.parent = null;
            }

            gameObject.SetActive(false);
        }
     

    }
}
