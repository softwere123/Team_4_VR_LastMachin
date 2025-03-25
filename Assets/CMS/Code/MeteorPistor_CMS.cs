using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; //XR ���ͷ��� ��Ŷ�� �����´�

// �ڵ� �˰��� ���� ���:
// �� �ڵ�� Braekable�� ��ȣ�ۿ��ϴ� Ŭ������ 
// MeteorPistor_CMS���� �߻�Ǵ� �������� �ε����� Ư�� ������Ʈ�� ã�Ƽ� Break �޼ҵ带 �����Ѵ�.
// 24,29�� 30~41������ ��ɵ��� XR�� ��ȣ�ۿ��� XRGrabInteractable�� ���� ����Ѵ�
// 30~41�� ����ĳ��Ʈ�� ��ƼŬ�� ȣ���ϴ� �Լ��̴�
// 47~56�� ����ĳ��Ʈ �Ÿ��� ���̾� ����ũ�� Bool�Լ��� ���ǽ��� ����� Break �޼ҵ带 �����Ѵ��Ѵ�
// 58~63�� �� ���ǽĵ��� ������Ʈ �Լ��� �Լ� Ȱ��ȭ ��Ų��.
// ���� �������� ��� ���: ���̳� ��ü���� ������ �μ��� ����� ������ Ŭ����

// �ڵ� ���: 
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
