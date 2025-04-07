using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGProjectile : MonoBehaviour
{
    private Transform transformCache;
    // 현재 발사체의 Transform을 캐싱하여 자주 사용되는 Transform 연산의 성능 최적화

    private SGBaseShot parentBaseShot;
    // 발사체를 생성한 부모 객체(SGBaseShot)를 참조 (발사 관련 정보를 관리)

    private float speed;
    // 현재 발사체의 속도

    private float angle;
    // 발사체의 현재 이동 각도

    private float accelSpeed;
    // 발사체의 가속도 (발사체 속도의 증가 또는 감소율)

    private float accelTurn;
    // 발사체의 회전 가속도 (발사체 방향 변경 속도)

    private bool homing;
    // 호밍(추적) 기능 활성화 여부

    private Transform homingTarget;
    // 호밍(추적) 시 목표로 설정된 대상(Target)의 Transform

    private float homingAngleSpeed;
    // 호밍 시 발사체가 목표를 향해 회전하는 속도

    private bool sinWave;
    // 발사체가 사인파 형태로 움직이는 동작 활성화 여부

    private float sinWaveSpeed;
    // 사인파 이동의 속도

    private float sinWaveRangeSize;
    // 사인파의 범위 크기 (사인파의 이동 폭)

    private bool sinWaveInverse;
    // 사인파를 반전하여 움직이게 할지 여부

    private bool pauseAndResume;
    // 발사체에 일시 정지 및 재개 기능을 적용할지 여부

    private float pauseTime;
    // 발사체가 일시 정지 상태가 되는 시간

    private float resumeTime;
    // 발사체가 일시 정지에서 다시 활성화되는 시간

    private bool useAutoRelease;
    // 자동 해제(반환 또는 삭제) 기능 활성화 여부

    private float autoReleaseTime;
    // 발사체가 자동으로 해제되기까지의 시간

    private SGUtil.AXIS axisMove;
    // 발사체가 이동할 축 설정 (SGUtil.AXIS: X-Y 축 또는 X-Z 축)

    private bool useMaxSpeed;
    // 발사체의 최대 속도 제한 여부

    private float maxSpeed;
    // 발사체의 최대 속도

    private bool useMinSpeed;
    // 발사체의 최소 속도 제한 여부

    private float minSpeed;
    // 발사체의 최소 속도

    private float baseAngle;
    // 발사체의 기준이 되는 초기 각도 (발사 방향의 기본 값)

    private float selfFrameCnt;
    // 발사체 발사 후 경과한 프레임 수 (이동 패턴 계산에 사용)

    private float selfTimeCount;
    // 발사체 발사 후 경과한 시간 (생명 주기 관리 및 자동 해제 확인에 사용)

    private bool shooting;
    // 발사체가 발사 중인지 여부를 나타내는 상태 변수

    private bool _reserveReleaseOnShot;
    // 발사체 발사 후 해제 예약 여부 (true일 경우 이후 자동 해제)

    private bool _reserveReleaseOnShotIsDestroy;
    // 발사체 해제 시 완전 파괴(destroy) 여부 (true이면 삭제, false이면 풀로 반환)


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
        // 발사체의 초기 속도를 설정

        this.angle = angle;
        // 발사체의 초기 이동 각도를 설정

        this.accelSpeed = accelSpeed;
        // 발사체의 가속도를 설정 (속도 증가 또는 감소율)

        this.accelTurn = accelTurn;
        // 발사체의 회전 가속도(방향 변경 속도)를 설정

        this.homing = homing;
        // 발사체의 호밍(추적) 기능 활성화 여부를 설정

        this.homingTarget = homingTarget;
        // 호밍의 대상 객체를 설정 (타겟의 Transform)

        this.homingAngleSpeed = homingAngleSpeed;
        // 발사체가 목표를 향해 회전하는 속도를 설정

        this.sinWave = sinWave;
        // 발사체의 사인파 형태의 이동 동작 활성화 여부를 설정

        this.sinWaveSpeed = sinWaveSpeed;
        // 발사체의 사인파 이동 속도를 설정

        this.sinWaveRangeSize = sinWaveRangeSize;
        // 발사체의 사인파 이동 범위(진폭)를 설정

        this.sinWaveInverse = sinWaveInverse;
        // 발사체의 사인파 이동을 반전할지 여부 설정

        this.pauseAndResume = pauseAndResume;
        // 발사체 이동 중 일시 정지 및 재개 기능 활성화 여부를 설정

        this.pauseTime = pauseTime;
        // 발사체가 이동을 일시 정지하는 시간을 설정

        this.resumeTime = resumeTime;
        // 발사체가 일시 정지를 마치고 다시 이동을 재개하는 시간을 설정

        this.useAutoRelease = useAutoRelease;
        // 발사체의 자동 해제 기능 활성화 여부를 설정

        this.autoReleaseTime = autoReleaseTime;
        // 발사체가 자동으로 해제되기 전까지 유지되는 시간을 설정

        this.axisMove = axisMove;
        // 발사체의 이동 축(XY 또는 XZ)을 설정 (SGUtil.AXIS)

        this.useMaxSpeed = useMaxSpeed;
        // 발사체의 최대 속도 제한 기능 활성화 여부를 설정

        this.maxSpeed = maxSpeed;
        // 발사체가 도달할 수 있는 최대 속도를 설정

        this.useMinSpeed = useMinSpeed;
        // 발사체의 최소 속도 제한 기능 활성화 여부를 설정

        this.minSpeed = minSpeed;
        // 발사체가 유지해야 할 최소 속도를 설정

        baseAngle = 0f;
        // 발사체의 초기 기준 각도(base angle)를 0으로 초기화

        if (inheritAngle && this.parentBaseShot.lockOnShot == false)
        // 부모 발사체의 각도를 상속받을지 확인 (inheritAngle이 true이고 lockOnShot이 비활성화된 경우)
        {
            if (this.axisMove == SGUtil.AXIS.X_AND_Z)
            // 발사체가 XZ 축을 기준으로 움직이는 경우
            {
                baseAngle = this.parentBaseShot.shotCtrl.transform.eulerAngles.y;
                // 부모 발사체의 Y축 회전 각도를 초기 기준 각도로 설정
            }
            else
            // 발사체가 XY 축을 기준으로 움직이는 경우
            {
                baseAngle = this.parentBaseShot.shotCtrl.transform.eulerAngles.z;
                // 부모 발사체의 Z축 회전 각도를 초기 기준 각도로 설정
            }
        }

        if (this.axisMove == SGUtil.AXIS.X_AND_Z)
        // 발사체가 XZ 축을 기준으로 움직이는 경우
        {
            transformCache.SetEulerAnglesY(baseAngle - this.angle);
            // 기준 각도에서 발사체의 각도를 빼서 새 Y축 회전을 적용
        }
        else
        // 발사체가 XY 축을 기준으로 움직이는 경우
        {
            transformCache.SetEulerAnglesZ(baseAngle + this.angle);
            // 기준 각도에 발사체의 각도를 더해서 새 Z축 회전을 적용
        }

        selfFrameCnt = 0f;
        // 발사체의 프레임 카운터 초기화 (사인파 이동 등에 활용)

        selfTimeCount = 0f;
        // 발사체의 경과 시간 타이머 초기화

        if (_reserveReleaseOnShot)
        // 발사체가 발사 후 해제 예약 상태일 경우
        {
            SGObjectPool.Instance.ReleaseProjectile(this, _reserveReleaseOnShotIsDestroy);
            // 오브젝트 풀로 반환하거나 파괴
        }
    }

    public void UpdateMove(float deltaTime)
    {
        if (shooting == false)
        {
            return;
        }

        selfTimeCount += deltaTime;

        // 오토 릴리즈 체크
        if (useAutoRelease && autoReleaseTime > 0f)
        {
            if (selfTimeCount >= autoReleaseTime)
            {              
                SGObjectPool.Instance.ReleaseProjectile(this);
                return;
            }
        }

        // 정지하고 다시 돌아갈때 체킹
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
            // 호밍 타겟 설정
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
            // 엑셀레이션 설정
            angle += (accelTurn * deltaTime);
            // 사인 웨이브
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
            // 엑셀레이션 설정
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

        // 엑셀레이션 스피드 설정
        speed += (accelSpeed * deltaTime);

        if (useMaxSpeed && speed > maxSpeed)
        {
            speed = maxSpeed;
        }

        if (useMinSpeed && speed < minSpeed)
        {
            speed = minSpeed;
        }

        // 이동
        Vector3 newPosition;
        if (axisMove == SGUtil.AXIS.X_AND_Z)
        {
            // X and Z axis
            newPosition = transformCache.position + (transformCache.forward * (speed * deltaTime));
        }
        else
        {
            // X and Y axis
            newPosition = transformCache.position + (transformCache.up * (speed * deltaTime));
        }

        // 새로운 포지션과 로테이션 설정
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
}
