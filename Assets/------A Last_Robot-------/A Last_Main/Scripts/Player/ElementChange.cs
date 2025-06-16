using System.Collections;
using System.Collections.Generic;
using Autohand;
using Unity.VisualScripting;
using UnityEngine;

public class ElementChange : MonoBehaviour
{
    private int ElementalStatus = 0;
    private Grabbable grabbable;

    void Start()
    {
        grabbable = GetComponent<Grabbable>();
    }
    // 불오브젝트 만드는 코드 ChangerbleElement 태그가 있으면 
    public void FireElement()
    {
        if (grabbable.IsHeld())
        {
            Hand firstHand = grabbable.GetHeldBy()[0];
            var myComponent = firstHand.GetComponent<SetElemental>();


            if (myComponent.isFire ==  true)        
            {
                Transform pointLightTransform = gameObject.transform.Find("Point Light");
                GameObject pointLightObj = pointLightTransform.gameObject;
                pointLightObj.SetActive(true);
                ElementalStatus = 1;
                Debug.Log("불 원소 선택됨");
            }
               
        }

    }
    void OnTriggerEnter(Collider Object)
    {
        if (Object.gameObject.tag == "Wood" && ElementalStatus == 1)
        {
            Debug.Log("타겟 제거됨");
            Destroy(Object.gameObject);
        }
        //if (Object.gameObject.tag == "Wood" && ElementalStatus == 1)
        //{
        //    Destroy(Object);
        //}
        //if (Object.gameObject.tag == "Wood" && ElementalStatus == 1)
        //{
        //    Destroy(Object);
        //}
    }
    void Update()
    {
        if (grabbable.IsHeld())
        {
            var hands = grabbable.GetHeldBy();   // 현재 잡고 있는 손 리스트
        }
        
    }
}
