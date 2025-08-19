using UnityEngine;

public class CubeTriggerUI : MonoBehaviour
{
    [System.Serializable]
    public class TriggerPair
    {
        public Collider triggerCube;   // 트리거로 쓸 큐브 (Is Trigger 체크)
        public GameObject targetUI;    // 켜질 UI
    }

    public TriggerPair[] triggers; // 여러 개 매핑 가능

    void Start()
    {
        // 시작할 때 모든 UI 끄기
        foreach (var t in triggers)
        {
            if (t.targetUI != null)
                t.targetUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var t in triggers)
        {
            if (other == t.triggerCube) // 플레이어가 해당 큐브에 들어오면
            {
                if (t.targetUI != null)
                    t.targetUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var t in triggers)
        {
            if (other == t.triggerCube) // 플레이어가 해당 큐브에서 나가면
            {
                if (t.targetUI != null)
                    t.targetUI.SetActive(false);
            }
        }
    }
}
