using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGLinearShot : SGBaseShot
{
    public float angle = 180.0f;
    public float betweenDelay = 0.1f;

    private int nowIndex;
    public float delayTimer;
    public float delayTime;
    private void Start()
    {
        delayTime = delayTimer;
    }

    public override void Shot()
    {
        if (projectileNum <= 0 || projectileSpeed <= 0)
        {
            return;
        }
        if (_shooting)
        {
            return;
        }
        _shooting = true;
        delayTimer = delayTime;
    }

    protected virtual void Update()
    {
        if (_shooting == false)
        {
            return;
        }
        delayTimer -= SGTimer.Instance.deltaTime;

        while (delayTimer < 0)
        {
            SGProjectile projectile = GetProjectile(transform.position);
            if (projectile == null)
            {
                break;
            }

            ShotProjectile(projectile, projectileSpeed, angle);
            projectile.UpdateMove(-delayTimer);
            nowIndex++;

            if (nowIndex >= projectileNum)
            {
                FinishedShot();
                return;
            }

            delayTimer += betweenDelay;
        }

    }
}