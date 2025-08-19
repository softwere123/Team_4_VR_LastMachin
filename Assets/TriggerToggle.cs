using UnityEngine;

public class TriggerToggle : MonoBehaviour
{
    [Tooltip("트리거 안에 들어오면 켜지고, 나가면 꺼질 오브젝트")]
    public GameObject targetObject;

    private void Start()
    {
        if (targetObject != null)
            targetObject.SetActive(false); // 처음에는 꺼져 있게
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어만 반응하게
        {
            if (targetObject != null)
                targetObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }
    }
}
