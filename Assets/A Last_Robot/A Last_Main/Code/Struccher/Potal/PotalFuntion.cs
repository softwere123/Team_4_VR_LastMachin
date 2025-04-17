using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotalFuntion : MonoBehaviour
{
    // 활성화할 오브젝트와 위치를 저장하는 리스트
    [System.Serializable]
    public class ActivatableObject
    {
        public GameObject obj; // 활성화할 오브젝트
        public Vector3 position; // 오브젝트 활성화 위치
    }

    public List<ActivatableObject> objectsToActivate; // 리스트
    public AudioSource activateSound; // 사운드 추가

    private void Start()
    {
        // TriggerZone_CMS 스크립트에서 OnEnterEvent 발생 시 InsideTrash 함수 실행
        GetComponent<PotalTag>().OnEnterEvent.AddListener(InsideTrash);
    }

    // 트리거에 진입한 오브젝트를 비활성화하고, 해당 오브젝트만 활성화하는 함수
    public void InsideTrash(GameObject go)
    {
        // 들어온 오브젝트를 비활성화
        go.SetActive(false);

        // 리스트에서 들어온 오브젝트와 동일한 경우만 활성화하고 위치 설정
        foreach (ActivatableObject activatable in objectsToActivate)
        {
            if (activatable.obj == go) // 들어온 오브젝트와 같은 경우만 실행
            {
                activatable.obj.SetActive(true); // 오브젝트 활성화
                activatable.obj.transform.position = activatable.position; // 위치 설정
                break; // 하나만 실행 후 루프 종료
            }
        }

        // 사운드가 할당되어 있으면 재생
        if (activateSound != null)
        {
            activateSound.Play();
        }
    }
}
