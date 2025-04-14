using System.Collections;
using UnityEngine;

/// <summary>
/// 모든 탄막 베이스 클래스. SGLinearShot 등은 이걸 상속함.
/// </summary>
public abstract class SGBaseShot : MonoBehaviour
{
    public GameObject projectilePrefab;

    public int projectileNum = 10;
    public float projectileSpeed = 2f;
    public float accelerationSpeed = 0f;
    public bool useMaxSpeed = false;
    public float maxSpeed = 0f;
    public bool useMinSpeed = false;
    public float minSpeed = 0f;
    public float accelerationTurn = 0f;
    public bool usePauseAndResume = false;
    public float pauseTime = 0f;
    public float resumeTime = 0f;
    public bool useAutoRelease = false;
    public float autoReleaseTime = 10f;

    protected bool _shooting; // 현재 발사 중인지

    private SGShotCtrl _shotCtrl;

    public SGShotCtrl shotCtrl
    {
        get
        {
            if (_shotCtrl == null)
            {
                _shotCtrl = GetComponentInParent<SGShotCtrl>();
            }
            return _shotCtrl;
        }
    }

    public bool shooting => _shooting; // 읽기 전용 속성
    public virtual bool lockOnShot => false;

    protected virtual void OnDestroy()
    {
        _shooting = false;
    }

    /// <summary>
    /// 탄막 실행 함수 (자식 클래스에서 구현해야 함)
    /// </summary>
    public abstract void Shot();

    public void SetShotCtrl(SGShotCtrl shotCtrl)
    {
        _shotCtrl = shotCtrl;
    }

    protected virtual void FiredShot() { }

    public virtual void FinishedShot()
    {
        _shooting = false;
    }

    /// <summary>
    /// 매 프레임마다 호출되는 업데이트 함수 (추적/파동 등에서 오버라이드 가능)
    /// </summary>
    public virtual void UpdateShot(float deltaTime)
    {
        // 기본은 아무것도 안 함. 필요 시 자식 클래스에서 구현
    }

    protected SGProjectile GetProjectile(Vector3 position, bool forceInstantiate = false)
    {
        if (projectilePrefab == null)
            return null;

        return SGObjectPool.Instance.Getprojectile(projectilePrefab, position, forceInstantiate);
    }

    protected void ShotProjectile(SGProjectile projectile, float speed, float angle, bool homing = false,
        Transform homingTarget = null, float homingAngleSpeed = 0f, bool sinWave = false, float sinWaveSpeed = 0f,
        float sinWaveRangeSize = 0f, bool sinWaveInverse = false)
    {
        if (projectile == null) return;

        projectile.Shot(this, speed, angle, accelerationSpeed, accelerationTurn,
            homing, homingTarget, homingAngleSpeed,
            sinWave, sinWaveSpeed, sinWaveRangeSize, sinWaveInverse,
            usePauseAndResume, pauseTime, resumeTime,
            useAutoRelease, autoReleaseTime,
            _shotCtrl.axisMove, _shotCtrl.inheritAngle,
            useMaxSpeed, maxSpeed, useMinSpeed, minSpeed);
    }
}
