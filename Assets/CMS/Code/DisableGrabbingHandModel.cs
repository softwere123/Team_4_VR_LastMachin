using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit; //xr���ͷ�����Ŷ�� ����ϱ����� ���ӽ����̽�

//�׷�Ƽ�� ��Ҹ� ������� �ո��� ��Ȱ��ȭ ���Ѽ�
//�׷�Ƽ�� ��Ҹ� ���� �ڿ������� ������ �ϱ����� ��ũ��Ʈ

//23������ 47������ �ٲٱ⸸ �ϸ� �� �ٸ� �������� ���� ���ù����� �ڵ��
public class DisableGrabbingHandModel : MonoBehaviour
{

    public GameObject leftHandModel; //�޼ո� �׸�
    public GameObject rightHandModel; //�����ո� �׸�
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractables = GetComponent<XRGrabInteractable>(); //
        grabInteractables.selectEntered.AddListener(HideGrabbingHand); //��±�ɻ��� ���Խ� ���Ұ��� addlinstener�� �߰�
        grabInteractables.selectExited.AddListener(ShowGrabbingHand); //��±�ɻ��� ����� ���Ұ��� addlinstener�� addlintenr �ż��� ����� �߰�
    }
    public void HideGrabbingHand(SelectEnterEventArgs args) //��±�ɻ��� ���Խ� ���Ұ��� ���Ŀ� ���� ����Ʈ �κ��� �̰��� �߰��� ����
    {
        //�޼����� ������ else if������ ���������� ������ ��� 
        if (args.interactorObject.transform.tag == "Left Hand") //�޼���Ʈ�ѷ��϶� 
        {
            leftHandModel.SetActive(false); //�޼ո��� ��Ȱ��ȭ
        }
        else if (args.interactorObject.transform.tag == "Right Hand") //��������Ʈ�ѷ��϶�
        {
            rightHandModel.SetActive(false); //�����ո��� ��Ȱ��ȭ
        }

    }

    public void ShowGrabbingHand(SelectExitEventArgs args) //��±�ɻ��� ����� ���Ұ���
    {
        //�޼����� ������ else if������ ���������� ������ ��� 
        if (args.interactorObject.transform.tag == "Left Hand") //�޼���Ʈ�ѷ��϶� 
        {
            leftHandModel.SetActive(true); //�޼ո��� ��Ȱ��ȭ
        }
        else if (args.interactorObject.transform.tag == "Right Hand") //��������Ʈ�ѷ��϶�
        {
            rightHandModel.SetActive(true); //�����ո��� ��Ȱ��ȭ
        }

    }
        // Update is called once per frame

}
