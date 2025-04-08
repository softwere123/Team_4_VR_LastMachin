using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.XR.OpenVR;
using Autohand.Demo;

//실제 보여지는 부분: 라디우스 형태 Ui로 선택시 상태값을 변환한다

//코드 3줄요약 리스트로 이미지생성 트랜스폼으로 위치값가져오기  리스트를 UI 프리팹과 바꿔치는 함수생성
// 4개의 5개 원하는 만큼 생성가능 그후 손의 위치값과 라디우스의 거리를 피타고라스 정리로 계산후 방사형 백터 값가져오고
// 그값이 1씩 늘어나는 포이치문 생성후 1마다 변환값 적어주면 끝 아직 더 연구가 필요 다 이해못험
public class RadialSelectiom : MonoBehaviour
{


    //public bool ButtonReleased(CommonButton button)
    //{
    //    if (button == CommonButton.none)
    //        return false;

    //    if (device.TryGetFeatureValue(GetCommonButton(button), out bool pressed))
    //    {
    //        return !pressed; // 버튼에서 손을 뗀 경우
    //    }

    //    return false;
    //}

    //public OVRInputBOuttom spawnButton; // 이부분이 가져올수없어 다른것으로 대체요망 오토핸드에서 상호작용가능한걸로

    private XRHandControllerLink handController; //  손 컨트롤러 링크를 가져온다
    
    public AudioSource radialSound; // 추가: 사운드를 재생할 오디오 소스
    private bool isPlayingSound = false; // 현재 사운드가 재생 중인지 체크

    public CommonButton spawnButton = CommonButton.primaryButton; // 버튼을 눌렀는지 체크하는 변수
    private bool isSlowed = false;// 타임 스케일을 조절하기 위한 변수
    private bool soundPlayed = false; // 사운드가 재생되었는지 체크하는 변수
    [Range(2 , 10)] //이미지 갯수 <범위 드래그 범위 설정
    public int numberOfRadialPart; //여기에 범위 설정된 숫자를 넣어준다 
    public GameObject radialPartPrefab;//생성할 이미지를 넣어준다 
    public Transform radialPartCanvas;  //이미지를 생성할 캔버스의 위치를 설정한다
    public float angleBetweenPart = 10f; //이미지 간의 각도를 설정한다
    public Transform handTransform; //손의 위치를 설정한다

    public UnityEvent<int> OnPartSelected; //이벤트를 설정한다

    private List<GameObject> spawnedParts = new List<GameObject>(); //생성된 이미지를 리스트에 넣어준다 
    private int currentSelectedRadialPart = 0; //현재 선택된 이미지를 설정한다

    public AudioSource clickSound; // Inspector에서 소리 연결해주기


    void Start()
    {

        handController = GetComponent<XRHandControllerLink>();
        if (handController == null)
        {
            Debug.LogError("XRHandControllerLink가 없음! Auto Hand가 올바르게 설정되었는지 확인하세요.");
        }
    }
    // Start is called before the first frame update

    void Update()
    {
        if (handController != null)
        {
            if (handController.ButtonPressed(spawnButton))
            { // 버튼 눌렀을 때
                SpawnedRadoalPart();
            }

            if (handController.ButtonPressed(spawnButton))
            { // 버튼 계속 누르고 있을 때
                GetSelectedRadiaPart();
            }

            if (!handController.ButtonPressed(spawnButton))
            { // 버튼에서 손 뗄 때
                HideAndTriggerSelected();
            }
        }
    }





    public void HideAndTriggerSelected() //선택된 이미지를 숨기고 이벤트를 발생시킨다
    {
        
        OnPartSelected.Invoke(currentSelectedRadialPart); //이벤트를 발생시킨다
        radialPartCanvas.gameObject.SetActive(false); //이미지를 숨긴다
    }
    public void GetSelectedRadiaPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        //손의 위치를 설정한다 에를 들어 A부터 B 그 가는거리를 핸드 트랜스폼으로 백터를 가져와 어떤 라디우스 이미지를 확인했는지 보여준더
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward); //각도가 180이상으로 안가게 ㅏㅁ든다

        if (angle < 0)       
            angle += 360;
        
        //각도를 얻었다
        Debug.Log("ANGLE" + angle);

        currentSelectedRadialPart = (int) angle * numberOfRadialPart / 360;  //int로 변환

        for (int i = 0; i < spawnedParts.Count; i++) //이미지 방사형값으로 어디 부분이지 검사
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.yellow; //이미지 색상을 흰색으로 설정
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;

            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white; //이미지 색상을 흰색으로 설정
                spawnedParts[i].transform.localScale = Vector3.one;

            }
        }
       
       
    }

    public void SpawnedRadoalPart() //생성된 이미지를 라디우스 형태로 배치하는 함수
    {
        radialPartCanvas.gameObject.SetActive(true); //이미지를 보여준다
                                                     //radialPartCanvas.rotation = handTransform.rotation; //손의 위치를 설정한다   
                                                     //radialPartCanvas.position = handTransform.position; //손의 위치를 설정한다   
                                                     // 손의 위치만 따라가고
        radialPartCanvas.position = handTransform.position;

        // 손의 정면(forward) 방향만 맞춘다 (회전 전체를 복사하지 않고)
        radialPartCanvas.forward = handTransform.forward;

        foreach (var item in spawnedParts ) //생성된 이미지를 리스트에 넣어준다
        {
            Destroy(item); //이미지를 삭제한다
        }

        spawnedParts.Clear(); //리스트를 비운다

        for (int i = 0; i < numberOfRadialPart; i++) //이미지 갯수만큼 반복 즉 위에 넣은 숫자만큼 생성후 나눠주는 역활 예술적이군.
        {
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart /2; //회전시킬 음식  //z 축으로 회전해 포이치문이 반대로간다 그걸 수정하려면 i-1로 바꾸면된다
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle); // 회전 시킬음식 그릇 생성

            GameObject spawnedRadoalPart = Instantiate(radialPartPrefab, radialPartCanvas); // 음식 회전후 음식추가할 공장 생성 후 위치 그릇 지정
            spawnedRadoalPart.transform.position = radialPartCanvas.position;    //그릇의 위치
            spawnedRadoalPart.transform.localEulerAngles= radialPartEulerAngle;  // 그릇의 각도

            spawnedRadoalPart.GetComponent<Image>().fillAmount = (1 / (float)numberOfRadialPart) - (angleBetweenPart/360);   //플레이팅 이미지 넣고 각도 및 포지션 설정 , 얼마나 생성하였는지 숫자 측정

            spawnedParts.Add(spawnedRadoalPart); //생성된 이미지를 리스트에 넣어준다
        }
    }
}
