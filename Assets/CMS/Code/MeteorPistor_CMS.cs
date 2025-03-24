using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; //XR 인터랙션 툴킷을 가져온다

// 코드 알고리즘 세줄 요약:
// 이 코드는 Braekable와 상호작용하는 클래스로 
// MeteorPistor_CMS에서 발사되는 레이저가 부딪히는 특정 오브젝트를 찾아서 Break 메소드를 전송한다.
// 24,29는 30~41까지의 기능들을 XR로 상호작용인 XRGrabInteractable을 통해 출력한다
// 30~41은 레이캐스트와 파티클을 호출하는 함수이다
// 47~56은 레이캐스트 거리와 레이어 마스크를 Bool함수로 조건식을 만들어 Break 메소드를 전송한다한다
// 58~63은 위 조건식들을 업데이트 함수로 게속 활성화 시킨다.
// 실제 보여지는 기능 요약: 돌이나 물체들을 총으로 부수는 기능을 구현한 클래스

// 코드 요약: 
public class MeteorPistor_CMS : MonoBehaviour
{
    public ParticleSystem particles;

    public LayerMask layerMask;
    public Transform shootSource;
    public float distance= 10;

    private bool rayActive = false;
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(x => StarShoot());
        grabInteractable.deactivated.AddListener(x => StopShoot());
    }
    public void StarShoot()
    {
        particles.Play();
        rayActive = true;
    }
    public void StopShoot()
    {
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        rayActive = false;

        // Update is called once per frame
    }

    

    void RaycastCheck()
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(shootSource.position, shootSource.forward, out hit, distance, layerMask);

        if (hasHit)
        {
            hit.transform.gameObject.SendMessage("Break", SendMessageOptions.DontRequireReceiver);
        }
    }

    void Update()
    {
        if (rayActive)
            RaycastCheck();

    }
}
