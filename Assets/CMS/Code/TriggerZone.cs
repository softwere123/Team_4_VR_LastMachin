using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // 콜라이드 함수를 불러오기 위해 이벤트 전역 변수를 사용한다.
public class TriggerZone : MonoBehaviour
{
    public string targetTag; // 타겟 태그 이름을 설정한다.
    public UnityEvent<GameObject> OnEnterEvent; // 콜라이드 함수를 불러오기 위한 이벤트 전역 변수에서 게임오브젝트 이밴트 타입을 가져와 onEnterEvent에 넣는다
                                                

    private void OnTriggerEnter(Collider other) //pruvate void OnTriggerEnter(Collider other) 함수를 사용하여 콜라이드 함수를 불러온다.
    {
      
            if(other.gameObject.tag == targetTag) //여기서 한번더 검사해 확실히 태그가 붙은것일경우 이프문으로 보내고
            {
                OnEnterEvent.Invoke(other.gameObject); //이벤트를 호출한다.

            }
        
    }
}
