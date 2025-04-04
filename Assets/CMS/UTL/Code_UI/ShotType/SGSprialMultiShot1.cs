using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGSprialMultiShot : SGBaseShot
{
    public int spiralWayNum = 4;
    public float startAngle = 180f;
    public float shiftAngle = 5f;
    public float betweenDealy = 0.2f;
    private int nowIndex;
    private float delayTimer;
    public override void Shot()             //Shot 필수 구현 함수 (SGBaseShot)
    {
        if (projectileNum <= 0 || projectileSpeed <= 0f || spiralWayNum <= 0)    //옵션값검사
        {
            return;
        }
        if (_shooting)
        {
            return;
        }
        _shooting = true;
        nowIndex = 0;
        delayTimer = 0;
    }
    protected virtual void Update()
    {
        if (_shooting == false)
        {
            return;
        }
        delayTimer -= SGTimer.Instance.deltaTime;

        while (delayTimer <= 0)        //총알 딜레이가 다 될경우
        {
            float spiralWayShiftAngle = 360f / spiralWayNum;

            for (int i = 0; i < spiralWayNum; i++)
            {
                SGProjectile projectile = GetProjectile(transform.position);
                if (projectile == null)
                {
                    break;
                }
                float angle = startAngle + (spiralWayShiftAngle * i) + (shiftAngle * Mathf.Floor(nowIndex / spiralWayNum));
                ShotProjectile(projectile, projectileSpeed, angle);
                projectile.UpdateMove(-delayTimer);
                nowIndex++;
                if (nowIndex >= projectileNum)
                {
                    break;
                }
            }
            FiredShot();
            if (nowIndex >= projectileNum)
            {
                FinishedShot();
                return;
            }
            delayTimer += betweenDealy;
        }
    }
}