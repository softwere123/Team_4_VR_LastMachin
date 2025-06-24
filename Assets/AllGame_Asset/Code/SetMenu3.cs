using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// 원소 선택 기능을 관리하는 스크립트 총3개의 메뉴를 관리하여 Set meu3로 명명   
public class SetMenu3 : MonoBehaviour
{
    //원래는 드롭박스에 스냅턴과    컨티뉴스턴을 넣어서 사용하려고 했으나    유니티6.0에서는 이미 내장되어있어서 사용하지 않음
    //public ActionBasedSnapTurnProvider snapturn;
    //public ActionBasedContinuousTurnProvider continiousTurn;

    // Fwood, Ffire, Ftelport 배열을 선언 인터랙테이블에 기능으로 사용할 배열과 조건을 가져옴
    public XRSimpleInteractable[] Fwood;
    public XRSimpleInteractable[] Ffire;
    //public XRRayInteractor[] Ftelport;  //텔포트 기능을 사용할때 사용할 배열 기능 구현예정

    // SetTypeFromIndex 메서드를 사용하여 인덱스에 따라 오브젝트 변경
    public void SetTypeFromIndex(int index) //dropdown에서 선택한 인덱스를 받아와서 해당 인덱스에 맞는 오브젝트를 활성화
    {
        //기본상태는 0,  일경우 모든 오브젝트 비활성화 , 드롭박스로 1.2.3 경우를 선택가능하다
        if (index == 0)
        {
            //배열에 있는 상호작용 기능을 비활성화
            SetFwoodObjectsEnabled(false);
            SetFfireObjectsEnabled(false);
            //SetFtelportObjectsEnabled(false);
        }
        else if (index == 1)
        {
            //배열에 있는 상호작용 fwood의 상호작용만 기능을 활성화
            SetFwoodObjectsEnabled(true);
            SetFfireObjectsEnabled(false);
           // SetFtelportObjectsEnabled(false);
        }
        else if (index == 2)
        {
            //배열에 있는 상호작용 ffire의 상호작용만 기능을 활성화
            SetFwoodObjectsEnabled(false);
            SetFfireObjectsEnabled(true);
           // SetFtelportObjectsEnabled(false);
        }
        else if (index == 3)
        {
            //배열에 있는 상호작용 ftelport의 상호작용만 기능을 활성화
            SetFwoodObjectsEnabled(false);
            SetFfireObjectsEnabled(false);
            //SetFtelportObjectsEnabled(true);
        }
    }

    // Fwood 배열의 모든 오브젝트의 enabled 값을 설정하는 메서드(아직이해못함)
    //아마 평소애는 비활성화되어있는데 이 메서드를 통해 활성화되는것으로 추정 그것이 
    private void SetFwoodObjectsEnabled(bool enabled)
    {
        foreach (XRSimpleInteractable fwoodObject in Fwood)
        {
            fwoodObject.enabled = enabled;
        }
    }

    // Ffire 배열의 모든 오브젝트의 enabled 값을 설정하는 메서드
    private void SetFfireObjectsEnabled(bool enabled)
    {
        foreach (XRSimpleInteractable ffireObject in Ffire)
        {
            ffireObject.enabled = enabled;
        }
    }

    //// Ftelport 배열의 모든 오브젝트의 enabled 값을 설정하는 메서드
    //private void SetFtelportObjectsEnabled(bool enabled)
    //{
    //    foreach (XRRayInteractor ftelportObject in Ftelport)
    //    {
    //        ftelportObject.enabled = enabled;
    //    }
    //}
}
