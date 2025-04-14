using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class SGSprrialShot : SGBaseShot
{
    public float starAngle = 180f; //시간
    public float shigtAngle = 10f;  //움직이는 각도
    public float betweenDealy = 0.2f;
    private int nowindex;
    private float delayTimer;

    public override void Shot()
    {
        if (projectileNum <= 0 || projectileSpeed <= 0f)  //옵션값 검사
        {
            return;
        }

        if (_shooting)
        {
            return;
        }
        _shooting = true;
        nowindex = 0;
        delayTimer = 0;
    }
    protected virtual void Update()
    {
        if (_shooting == false)
        {
            return;
        }
        delayTimer -= SGTimer.Instance.deltaTime;
        while (delayTimer <= 0)
        {
            SGProjectile projectile = GetProjectile(transform.position);
            if (projectile == null )
            {
                FinishedShot();
                return;
            }
            float angle = starAngle + (shigtAngle * nowindex);
            ShotProjectile(projectile, projectileSpeed, angle);
            nowindex++;
            if (nowindex >= projectileNum)
            {
                FinishedShot();
                return;

            }
            delayTimer += betweenDealy;

        }
    }

    // Start is called before the first frame update


    // Update is called once per frame

}
