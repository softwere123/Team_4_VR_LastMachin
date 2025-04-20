using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.XR.OpenVR;
using Autohand.Demo;

/// <summary>
/// 손의 위치에 따라 방사형 UI를 생성하고 선택하는 기능을 제공하는 클래스입니다.
/// </summary>
public class RadialSelectiom : MonoBehaviour
{
    private XRHandControllerLink handController; // 손 컨트롤러 입력을 위한 링크
    public AudioSource radialSound;              // 사운드 재생용 오디오 소스
    private bool isPlayingSound = false;         // 사운드 재생 상태 확인용 변수

    public CommonButton spawnButton = CommonButton.primaryButton; // UI를 호출할 버튼
    private bool isSlowed = false;             // 시간 축소 상태 확인 변수
    private bool soundPlayed = false;          // 사운드가 재생되었는지 여부

    [Range(1, 10)]
    public int numberOfRadialPart;             // 생성할 UI 파트의 개수

    public GameObject radialPartPrefab;        // UI 파트 프리팹
    public Transform radialPartCanvas;         // UI가 배치될 캔버스
    public float angleBetweenPart = 10f;       // 파트 간의 간격 각도
    public Transform handTransform;            // 손의 위치를 참조할 트랜스폼

    public UnityEvent<int> OnPartSelected;     // 파트 선택 시 호출될 이벤트

    private List<GameObject> spawnedParts = new();  // 생성된 파트 저장용 리스트
    private int currentSelectedRadialPart = 0;      // 현재 선택된 파트 인덱스

    public AudioSource clickSound;             // 클릭 사운드 소스

    void Start()
    {
        handController = GetComponent<XRHandControllerLink>();
        if (handController == null)
        {
            Debug.LogError("XRHandControllerLink 컴포넌트를 찾을 수 없습니다. AutoHand 설정을 확인해주세요.");
        }
    }

    void Update()
    {
        if (handController == null)
            return;

        if (handController.ButtonPressed(spawnButton))
        {
            SpawnedRadoalPart();   // 버튼을 누르면 UI 생성
            GetSelectedRadiaPart(); // 버튼을 누르고 있는 동안 파트 선택 감지
        }

        if (!handController.ButtonPressed(spawnButton))
        {
            HideAndTriggerSelected(); // 버튼에서 손을 뗐을 때 선택 확정
        }
    }

    /// <summary>
    /// 선택된 파트를 숨기고 해당 이벤트를 호출합니다.
    /// </summary>
    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// 손의 위치를 기준으로 현재 선택된 파트를 계산하고 강조 표시합니다.
    /// </summary>
    public void GetSelectedRadiaPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);
        if (angle < 0)
            angle += 360;

        Debug.Log("ANGLE: " + angle);

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360f);

        // 선택된 파트를 강조하고 나머지는 기본 상태로 설정
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            var image = spawnedParts[i].GetComponent<Image>();
            if (i == currentSelectedRadialPart)
            {
                image.color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                image.color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// 방사형으로 UI 파트를 생성하고 손 위치에 배치합니다.
    /// </summary>
    public void SpawnedRadoalPart()
    {
        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.forward = handTransform.forward;

        foreach (var item in spawnedParts)
        {
            Destroy(item); // 기존 파트 제거
        }
        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = i * 360f / numberOfRadialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle + 60);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            // 이미지가 차지할 비율 설정 (간격 고려)
            spawnedRadialPart.GetComponent<Image>().fillAmount = (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

            spawnedParts.Add(spawnedRadialPart);
        }
    }
}

