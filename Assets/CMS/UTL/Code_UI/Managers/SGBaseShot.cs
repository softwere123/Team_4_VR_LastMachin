using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine;

// SGBaseShot: 모든 발사 패턴의 기본 클래스
// 이 클래스를 상속받아 다양한 Shot(발사) 패턴을 구현할 수 있음
public abstract class SGBaseShot : MonoBehaviour
{
    // 발사체(Projectile) 프리팹
    public GameObject projectilePrefab;

    // 발사 설정 변수들
    public int projectileNum = 10;                // 한 번에 발사할 발사체 수
    public float projectileSpeed = 2f;           // 발사체의 기본 속도
    public float accelerationSpeed = 0f;         // 발사체의 가속도
    public bool useMaxSpeed = false;             // 최대 속도 제한 여부
    public float maxSpeed = 0f;                  // 발사체의 최대 속도
    public bool useMinSpeed = false;             // 최소 속도 제한 여부
    public float minSpeed = 0f;                  // 발사체의 최소 속도
    public float accelerationTurn = 0f;          // 가속 중 회전 속도

    // 발사체의 일시 정지/재개 관련 옵션
    public bool usePauseAndResume = false;       // 발사체의 일시 정지/재개 기능 사용 여부
    public float pauseTime = 0f;                 // 일시 정지 시간
    public float resumeTime = 0f;                // 재개 시간

    // 발사체 자동 해제 옵션
    public bool useAutoRelease = false;          // 발사체 자동 해제(파괴) 여부
    public float autoReleaseTime = 10f;          // 해제 전 대기 시간

    // 발사 진행 중인지 여부
    protected bool _shooting;

    // Shot 컨트롤러 참조 변수 (SGShotCtrl)
    private SGShotCtrl _shotCtrl;

    // Shot 컨트롤러 접근 프로퍼티
    public SGShotCtrl shotCtrl
    {
        get
        {
            if (_shotCtrl == null)
            {
                // SGShotCtrl은 부모 객체에서 찾아 참조
                _shotCtrl = GetComponentInParent<SGShotCtrl>();
            }
            return _shotCtrl;
        }
    }

    // 발사 중 상태 확인 프로퍼티
    public bool shooting { get { return _shooting; } }

    // 포함 클래스에서 오버라이드 가능 (기본값 false)
    public virtual bool lockOnShot { get { return false; } }

    // 발사 종료 시 호출
    protected virtual void OnDestroy()
    {
        _shooting = false;
    }

    // 발사를 위한 추상 메서드 (상속받는 클래스에서 구현 필요)
    public abstract void Shot();

    // 외부에서 Shot 컨트롤러를 설정
    public void SetShotCtrl(SGShotCtrl shotCtrl)
    {
        _shotCtrl = shotCtrl;
    }

    // 발사가 성공적으로 이루어졌음을 알림(상속 클래스에서 커스터마이징 가능)
    protected virtual void FiredShot()
    {
    }

    // 발사 완료 시 호출되는 메서드 (기본 동작: 발사 상태 초기화)
    public virtual void FinishedShot()
    {
        _shooting = false;
    }

    // 발사체(Projectile)를 생성하거나 가져오는 메서드
    protected SGProjectile GetProjectile(Vector3 position, bool forceInstantiate = false)
    {
        if (projectilePrefab == null)
        {
            // 발사체 프리팹이 없을 경우 null 반환
            return null;
        }

        // SGObjectPool에서 발사체를 가져옴 (오브젝트 풀 패턴 사용)
        return SGObjectPool.Instance.Getprojectile(projectilePrefab, position, forceInstantiate);
    }

    // 발사체를 발사하는 메서드
    protected void ShotProjectile(SGProjectile projectile, float speed, float angle, bool homing = false, Transform homingTarget = null, float homingAngleSpeed = 0f,
        bool sinWave = false, float sinWaveSpeed = 0f, float sinWaveRangeSize = 0f, bool sinWaveInverse = false)
    {
        if (projectile == null)
        {
            // 발사체가 없으면 아무 작업도 하지 않음
            return;
        }

        // 발사체의 발사 초기값 설정 및 발사
        projectile.Shot(this, speed, angle,
            accelerationSpeed, accelerationTurn, homing, homingTarget, homingAngleSpeed,
            sinWave, sinWaveSpeed, sinWaveRangeSize, sinWaveInverse,
            usePauseAndResume, pauseTime, resumeTime,
            useAutoRelease, autoReleaseTime,
            _shotCtrl.axisMove, _shotCtrl.inheritAngle,
            useMaxSpeed, maxSpeed, useMinSpeed, minSpeed);
    }
}
