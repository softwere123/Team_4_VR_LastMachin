using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketOpenDoor : MonoBehaviour
{
    public Animator animator;
    public string boolName = "Open";

    private XRSocketInteractor socket;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();

        if (socket != null)
        {
            socket.selectEntered.AddListener(OnItemPlaced);   // 오브젝트 들어옴
            socket.selectExited.AddListener(OnItemRemoved);   // 오브젝트 빠짐
        }
    }

    private void OnDestroy()
    {
        if (socket != null)
        {
            socket.selectEntered.RemoveListener(OnItemPlaced);
            socket.selectExited.RemoveListener(OnItemRemoved);
        }
    }

    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        // 소켓에 들어오면 문 열기
        animator.SetBool(boolName, true);
    }

    private void OnItemRemoved(SelectExitEventArgs args)
    {
        // 소켓에서 빠지면 문 닫기
        animator.SetBool(boolName, false);
    }
}
