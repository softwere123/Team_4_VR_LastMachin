using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // �ݶ��̵� �Լ��� �ҷ����� ���� �̺�Ʈ ���� ������ ����Ѵ�.
public class TriggerZone : MonoBehaviour
{
    public string targetTag; // Ÿ�� �±� �̸��� �����Ѵ�.
    public UnityEvent<GameObject> OnEnterEvent; // �ݶ��̵� �Լ��� �ҷ����� ���� �̺�Ʈ ���� �������� ���ӿ�����Ʈ �̹�Ʈ Ÿ���� ������ onEnterEvent�� �ִ´�
                                                

    private void OnTriggerEnter(Collider other) //pruvate void OnTriggerEnter(Collider other) �Լ��� ����Ͽ� �ݶ��̵� �Լ��� �ҷ��´�.
    {
      
            if(other.gameObject.tag == targetTag) //���⼭ �ѹ��� �˻��� Ȯ���� �±װ� �������ϰ�� ���������� ������
            {
                OnEnterEvent.Invoke(other.gameObject); //�̺�Ʈ�� ȣ���Ѵ�.

            }
        
    }
}
