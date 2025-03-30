using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class ObjectInteraction : MonoBehaviour
{
    public PlayableDirector timeline1;  // 첫 번째 오브젝트 타임라인
    public PlayableDirector timeline2;  // 두 번째 오브젝트 타임라인 (자동 할당됨)
    public GameObject object1; // 첫 번째 오브젝트
    public GameObject object2; // 두 번째 오브젝트
    public Transform player; // 플레이어 위치
    public float interactionDistance = 3f; // E 키를 누를 수 있는 거리
    public string nextSceneName; // 전환할 씬 이름

    void Start()
    {
        // timeline2가 할당되지 않았다면 자동으로 찾기
        if (timeline2 == null)
        {
            timeline2 = GameObject.Find("Timeline_Object2")?.GetComponent<PlayableDirector>();

            if (timeline2 == null)
            {
                Debug.LogError("timeline2가 할당되지 않았고, 자동으로 찾을 수도 없음! Inspector에서 직접 설정하세요.");
                return;
            }
        }

        // 타임라인이 끝나면 씬 전환 이벤트 연결
        timeline2.stopped += OnTimeline2End;
        Debug.Log("타임라인 이벤트 연결 완료");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceToObj1 = Vector3.Distance(player.position, object1.transform.position);
            float distanceToObj2 = Vector3.Distance(player.position, object2.transform.position);

            if (object1.activeSelf && distanceToObj1 <= interactionDistance)
            {
                Debug.Log("첫 번째 오브젝트 타임라인 실행");
                timeline1.Play();
                Invoke(nameof(DisableObject1), (float)timeline1.duration);
            }
            else if (object2.activeSelf && distanceToObj2 <= interactionDistance)
            {
                Debug.Log("두 번째 오브젝트 타임라인 실행");
                timeline2.Play();
                // object2 비활성화는 timeline2.stopped에서 처리하도록 변경
            }

            else
            {
                Debug.Log("거리가 너무 멀어서 E키 입력 무시됨.");
            }
        }
    }

    void DisableObject1()
    {
        object1.SetActive(false);
        Debug.Log("첫 번째 오브젝트 비활성화됨");
    }

    void DisableObject2()
    {
        object2.SetActive(false);
        Debug.Log("두 번째 오브젝트 비활성화됨");
    }

    void OnTimeline2End(PlayableDirector director)
    {
        if (director == timeline2)
        {
            Debug.Log("타임라인 종료, 씬 전환 시작!");
            
            
            // object2 끄기
            object2.SetActive(false);


            //씬 전환
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("다른 타임라인이 종료됨");
        }
    }
}
