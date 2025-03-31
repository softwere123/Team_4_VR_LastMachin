using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;


//보이는 코드 요약
public class ButtonPushOpenDoor_Now : MonoBehaviour
{
    public Animator animator;
    public string boolName = "Open";
    // Start is called before the first frame update
    public void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => ToggleDoorOpen());
        
    }


    // Update is called once per frame
    public void ToggleDoorOpen()
    {
        bool isOpen = animator.GetBool(boolName);
        animator.SetBool(boolName, !isOpen);

    }
}
