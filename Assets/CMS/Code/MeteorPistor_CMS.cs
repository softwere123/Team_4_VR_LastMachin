using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//XR 인터랙션 툴킷을 가져온다
// 코드 요약:
public class MeteorPistor_CMS : MonoBehaviour
{
    public ParticleSystem particles;
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
    }
    public void StopShoot()
    {
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Update is called once per frame
    }
}
