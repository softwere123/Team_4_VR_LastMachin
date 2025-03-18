using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit; //xr인터랙션툴킷을 사용하기위한 네임스페이스

//네러티브 요소를 잡았을떄 손모델을 비활성화 시켜서
//네러티브 요소를 더욱 자연스럽게 느끼게 하기위한 스크립트

//23번부터 47번에서 바꾸기만 하면 또 다른 구현력을 가진 무궁무진한 코드다
public class DisableGrabbingHandModel : MonoBehaviour
{

    public GameObject leftHandModel; //왼손모델 그릇
    public GameObject rightHandModel; //오른손모델 그릇
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractables = GetComponent<XRGrabInteractable>(); //
        grabInteractables.selectEntered.AddListener(HideGrabbingHand); //잡는기능사용시 진입시 뭐할건지 addlinstener로 추가
        grabInteractables.selectExited.AddListener(ShowGrabbingHand); //잡는기능사용시 후퇴시 뭐할건지 addlinstener로 addlintenr 매서드 등록툴 추가
    }
    public void HideGrabbingHand(SelectEnterEventArgs args) //잡는기능사용시 진입시 뭐할건지 추후에 사운드 이팩트 부분을 이곳에 추가할 예정
    {
        //왼손으로 잡을때 else if문으로 오른손으로 잡을때 기능 
        if (args.interactorObject.transform.tag == "Left Hand") //왼손컨트롤러일때 
        {
            leftHandModel.SetActive(false); //왼손모델을 비활성화
        }
        else if (args.interactorObject.transform.tag == "Right Hand") //오른손컨트롤러일때
        {
            rightHandModel.SetActive(false); //오른손모델을 비활성화
        }

    }

    public void ShowGrabbingHand(SelectExitEventArgs args) //잡는기능사용시 후퇴시 뭐할건지
    {
        //왼손으로 잡을때 else if문으로 오른손으로 잡을때 기능 
        if (args.interactorObject.transform.tag == "Left Hand") //왼손컨트롤러일때 
        {
            leftHandModel.SetActive(true); //왼손모델을 비활성화
        }
        else if (args.interactorObject.transform.tag == "Right Hand") //오른손컨트롤러일때
        {
            rightHandModel.SetActive(true); //오른손모델을 비활성화
        }

    }
        // Update is called once per frame

}
