using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElementChange : MonoBehaviour
{
    private int ElementalStatus = 0;
    void Start()
    {

    }
    // 불오브젝트 만드는 코드 ChangerbleElement 태그가 있으면 
    public void fireElement()
    {
        ElementalStatus = 1;
    }
    void OnTriggerEnter(Collider Object)
    {
        if (Object.gameObject.tag == "Wood" && ElementalStatus == 1)
        {
            Destroy(Object);
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

    }
}
