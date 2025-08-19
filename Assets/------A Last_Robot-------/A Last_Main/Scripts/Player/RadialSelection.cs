using Autohand.Demo;
using UnityEngine;
using UnityEngine.Events;

public class RadialSelection : MonoBehaviour
{
    private XRHandControllerLink handController; // 손 컨트롤러 입력
    public CommonButton spawnButton = CommonButton.primaryButton; // UI를 호출할 버튼

    public Transform handTransform;             // 손의 위치
    public Transform radialPartCanvas;          // 띄울 UI 캔버스

    public UnityEvent OnUIShow;  // UI 켜질 때 이벤트 (옵션)
    public UnityEvent OnUIHide;  // UI 꺼질 때 이벤트 (옵션)

    void Start()
    {
        handController = GetComponent<XRHandControllerLink>();
        if (handController == null)
            Debug.LogError("XRHandControllerLink 컴포넌트를 찾을 수 없습니다.");

        if (radialPartCanvas != null)
            radialPartCanvas.gameObject.SetActive(false); // 시작 시 꺼두기
    }

    void Update()
    {
        if (handController == null || radialPartCanvas == null)
            return;

        if (handController.ButtonPressed(spawnButton))
        {
            radialPartCanvas.gameObject.SetActive(true); // 버튼 누르면 켜기
            radialPartCanvas.position = handTransform.position; // 손 위치에 배치 (옵션)
            radialPartCanvas.forward = handTransform.forward;   // 방향 맞추기 (옵션)
            OnUIShow.Invoke();
        }
        else
        {
            radialPartCanvas.gameObject.SetActive(false); // 버튼 떼면 끄기
            OnUIHide.Invoke();
        }
    }
}
