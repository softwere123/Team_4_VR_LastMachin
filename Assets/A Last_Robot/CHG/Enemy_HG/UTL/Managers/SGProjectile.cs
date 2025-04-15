using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SG 시스템용 탄환 클래스 - 개별 탄환의 이동, 자동 삭제, 충돌 처리를 담당
/// </summary>
public class SGProjectile : MonoBehaviour
{
    private Transform transformCache;
    private SGBaseShot parentBaseShot;
    private float speed;
    private float angle;
    private float accelSpeed;
    private float accelTurn;
    private bool homing;
    private Transform homingTarget;
    private float homingAngleSpeed;
    private bool sinWave;
    private float sinWaveSpeed;
    private float sinWaveRangeSize;
    private bool sinWaveInverse;
    private bool pauseAndResume;
    private float pauseTime;
    private float resumeTime;
    private bool useAutoRelease;
    private float autoReleaseTime;
    private SGUtil.AXIS axisMove;
    private bool useMaxSpeed;
    private float maxSpeed;
    private bool useMinSpeed;
    private float minSpeed;

    private float baseAngle;
    private float selfFrameCnt;
    private float selfTimeCount;

    private bool shooting;

    private bool _reserveReleaseOnShot;
    private bool _reserveReleaseOnShotIsDestroy;

    public bool reserveReleaseOnShot { get { return _reserveReleaseOnShot; } set { _reserveReleaseOnShot = value; } }
    public bool reserveReleaseOnShotIsDestroy { get { return _reserveReleaseOnShotIsDestroy; } set { _reserveReleaseOnShotIsDestroy = value; } }

    public virtual bool isActive { get { return gameObject.activeSelf; } }

    public float _DeadTimer = 30.0f;
    private float _DeadCheckTimer;

    private void Awake()
    {
        transformCache = transform;
        _DeadCheckTimer = 0.0f;
    }

    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void Update()
    {
        _DeadCheckTimer += Time.deltaTime;

        if (_DeadCheckTimer >= _DeadTimer)
            Destroy(this);
    }

    public void Shot(SGBaseShot parentBaseShot,
                     float speed, float angle, float accelSpeed, float accelTurn,
                     bool homing, Transform homingTarget, float homingAngleSpeed,
                     bool sinWave, float sinWaveSpeed, float sinWaveRangeSize, bool sinWaveInverse,
                     bool pauseAndResume, float pauseTime, float resumeTime,
                     bool useAutoRelease, float autoReleaseTime,
                     SGUtil.AXIS axisMove, bool inheritAngle,
                     bool useMaxSpeed, float maxSpeed, bool useMinSpeed, float minSpeed)
    {
        if (shooting)
        {
            return;
        }
        shooting = true;

        this.parentBaseShot = parentBaseShot;

        this.speed = speed;
        this.angle = angle;
        this.accelSpeed = accelSpeed;
        this.accelTurn = accelTurn;
        this.homing = homing;
        this.homingTarget = homingTarget;
        this.homingAngleSpeed = homingAngleSpeed;
        this.sinWave = sinWave;
        this.sinWaveSpeed = sinWaveSpeed;
        this.sinWaveRangeSize = sinWaveRangeSize;
        this.sinWaveInverse = sinWaveInverse;
        this.pauseAndResume = pauseAndResume;
        this.pauseTime = pauseTime;
        this.resumeTime = resumeTime;
        this.useAutoRelease = useAutoRelease;
        this.autoReleaseTime = autoReleaseTime;
        this.axisMove = axisMove;
        this.useMaxSpeed = useMaxSpeed;
        this.maxSpeed = maxSpeed;
        this.useMinSpeed = useMinSpeed;
        this.minSpeed = minSpeed;

        baseAngle = 0f;
        if (inheritAngle && this.parentBaseShot.lockOnShot == false)
        {
            if (this.axisMove == SGUtil.AXIS.X_AND_Z)
            {
                baseAngle = this.parentBaseShot.shotCtrl.transform.eulerAngles.y;
            }
            else
            {
                baseAngle = this.parentBaseShot.shotCtrl.transform.eulerAngles.z;
            }
        }

        if (this.axisMove == SGUtil.AXIS.X_AND_Z)
        {
            transformCache.SetEulerAnglesY(baseAngle - this.angle);
        }
        else
        {
            transformCache.SetEulerAnglesZ(baseAngle + this.angle);
        }

        selfFrameCnt = 0f;
        selfTimeCount = 0f;

        if (_reserveReleaseOnShot)
        {
            SGObjectPool.Instance.ReleaseProjectile(this, _reserveReleaseOnShotIsDestroy);
        }
    }

    public void UpdateMove(float deltaTime)
    {
        if (shooting == false)
        {
            return;
        }

        selfTimeCount += deltaTime;

        // [1] 시간이 다 되면 오토 릴리즈
        if (useAutoRelease && autoReleaseTime > 0f)
        {
            if (selfTimeCount >= autoReleaseTime)
            {
                SGObjectPool.Instance.ReleaseProjectile(this);
                return;
            }
        }

        // [2] 일시 정지 기능 처리
        if (pauseAndResume && pauseTime >= 0f && resumeTime > pauseTime)
        {
            if (pauseTime <= selfTimeCount && selfTimeCount < resumeTime)
            {
                return;
            }
        }

        Vector3 myAngles = transformCache.rotation.eulerAngles;

        Quaternion newRotation = transformCache.rotation;
        if (homing)
        {
            if (homingTarget != null && 0f < homingAngleSpeed)
            {
                float rotAngle = SGUtil.GetAngleFromTwoPosition(transformCache, homingTarget, axisMove);
                float myAngle = 0f;
                if (axisMove == SGUtil.AXIS.X_AND_Z)
                {
                    myAngle = -myAngles.y;
                }
                else
                {
                    myAngle = myAngles.z;
                }

                float toAngle = Mathf.MoveTowardsAngle(myAngle, rotAngle, deltaTime * homingAngleSpeed);

                if (axisMove == SGUtil.AXIS.X_AND_Z)
                {
                    newRotation = Quaternion.Euler(myAngles.x, -toAngle, myAngles.z);
                }
                else
                {
                    newRotation = Quaternion.Euler(myAngles.x, myAngles.y, toAngle);
                }
            }
        }
        else if (sinWave)
        {
            angle += (accelTurn * deltaTime);
            if (0f < sinWaveSpeed && 0f < sinWaveRangeSize)
            {
                float waveAngle = angle + (sinWaveRangeSize / 2f * (Mathf.Sin(selfFrameCnt * sinWaveSpeed / 100f) * (sinWaveInverse ? -1f : 1f)));
                if (axisMove == SGUtil.AXIS.X_AND_Z)
                {
                    newRotation = Quaternion.Euler(myAngles.x, baseAngle - waveAngle, myAngles.z);
                }
                else
                {
                    newRotation = Quaternion.Euler(myAngles.x, myAngles.y, baseAngle + waveAngle);
                }
            }
            selfFrameCnt += SGTimer.Instance.deltaFrameCount;
        }
        else
        {
            float addAngle = accelTurn * deltaTime;
            if (axisMove == SGUtil.AXIS.X_AND_Z)
            {
                newRotation = Quaternion.Euler(myAngles.x, myAngles.y - addAngle, myAngles.z);
            }
            else
            {
                newRotation = Quaternion.Euler(myAngles.x, myAngles.y, myAngles.z + addAngle);
            }
        }

        speed += (accelSpeed * deltaTime);

        if (useMaxSpeed && speed > maxSpeed)
        {
            speed = maxSpeed;
        }

        if (useMinSpeed && speed < minSpeed)
        {
            speed = minSpeed;
        }

        Vector3 newPosition;
        if (axisMove == SGUtil.AXIS.X_AND_Z)
        {
            newPosition = transformCache.position + (transformCache.forward * (speed * deltaTime));
        }
        else
        {
            newPosition = transformCache.position + (transformCache.up * (speed * deltaTime));
        }

        transformCache.SetPositionAndRotation(newPosition, newRotation);
    }

    public void OnFinishedShot()
    {
        if (shooting == false)
        {
            return;
        }
        shooting = false;

        parentBaseShot = null;
        homingTarget = null;
        transformCache.ResetPosition();
        transformCache.ResetRotation();

        _reserveReleaseOnShot = false;
        _reserveReleaseOnShotIsDestroy = false;
    }

    private void OnDestroy()
    {
        if (SGObjectPool.Instance != null)
        {
            SGObjectPool.Instance.ReleaseProjectile(this);
        }
    }

    /// <summary>
    /// 플레이어와 충돌 시 즉시 총알 반납
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // "Player" 태그와 충돌한 경우만 처리
        if (other.CompareTag("Player"))
        {
            // TODO: 여기에 플레이어 피격 처리 로직 넣기 (예: PlayerHealth.TakeDamage(damage);)

            // 즉시 총알 풀에 반납
            SGObjectPool.Instance.ReleaseProjectile(this);
        }
    }
}
