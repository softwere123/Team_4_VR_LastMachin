using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڵ� �˰��� ���� ���:
// �� �ڵ�� MeteoPistor_CMS�� ��ȣ�ۿ��ϴ� Ŭ������ 
// MeteorPistor_CMS���� �߻�Ǵ� �������� �ε����� ������Ʈ�� ã�Ƽ� Break �޼ҵ带 ȣ���Ѵ�.
// ȣ�� ���ǽ����δ� 1. 2�ʰ� ����ĳ��Ʈ�� ���ӽ��Ѿ� �ߵ��ϸ� 2. ȣ��� �������۳�Ʈ ��Ȼ��ȭ& ���� ������Ʈ Ȱ��ȭ .

// ���� �������� ��� ���: ���̳� ��ü���� ������ �μ��� ����� ������ Ŭ����

// ������:����� ������ Ÿ�Ӷ��� �̺�Ʈ�� ó�� ����
public class Breakable_CMS : MonoBehaviour
{
    public List<GameObject> breakablepiecces;   // 
    public float timeToBraek = 2;
    public float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach ( var item in breakablepiecces)
        {
            item.SetActive(false);
        }

    }

    public void Break()
    {
        timer += Time.deltaTime;

        if (timer > timeToBraek)
        {
            foreach (var item in breakablepiecces)
            {
                item.SetActive(true);
                item.transform.parent = null;
            }

            gameObject.SetActive(false);
        }
     

    }
}
