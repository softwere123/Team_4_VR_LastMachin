using UnityEngine;

public class HologramFollowPlayer : MonoBehaviour
{
    public Transform player;  // 플레이어의 위치를 참조할 변수

    void Update()
    {
        if (player != null)
        {
            // 쿼드가 플레이어를 바라보도록 회전시킴
            Vector3 directionToPlayer = player.position - transform.position;  // 쿼드에서 플레이어 방향을 계산
            directionToPlayer.y = 0;  // y축 회전만 변경 (z축이나 x축 회전이 생기지 않도록)

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);  // 방향을 회전으로 변환
            transform.rotation = lookRotation;  // 쿼드 회전 적용
        }
    }
}
