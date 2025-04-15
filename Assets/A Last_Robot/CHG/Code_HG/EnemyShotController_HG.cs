using UnityEngine;

public class EnemyShotController_HG : MonoBehaviour
{
    [Header("참조 컴포넌트")]
    public Animator animator;          // 애니메이터 참조
    public SGShotCtrl shotCtrl;       // SGShotCtrl 참조

    [Header("상태 이름 설정")]
    public string attackStateName = "Attack";   // 발사할 애니메이션 상태
    public string reloadStateName = "Reload";   // 재장전 중인 상태

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 재장전 중이면 발사 멈춤
        if (stateInfo.IsName(reloadStateName))
        {
            if (shotCtrl.Shooting)
                shotCtrl.Shooting = false;
        }
        // 공격 상태면 발사 시작
        else if (stateInfo.IsName(attackStateName))
        {
            if (!shotCtrl.Shooting)
                shotCtrl.Shooting = true;
        }
        // 그 외 상태도 발사 중지
        else
        {
            if (shotCtrl.Shooting)
                shotCtrl.Shooting = false;
        }
    }
}
