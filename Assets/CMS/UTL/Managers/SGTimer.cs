using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SGTimer: 시간을 관리하고, 프레임 단위로 Projectile 및 Shot을 업데이트하는 클래스
public class SGTimer : MonoBehaviour
{
    static SGTimer s_instance;
    // 싱글톤 인스턴스를 저장

    public static SGTimer Instance { get { Init(); return s_instance; } }
    // SGTimer의 싱글톤 인스턴스를 반환

    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@SGTimer");
            // SGTimer GameObject를 찾음

            if (go == null)
            {
                go = new GameObject { name = "@SGTimer" };
                // 없으면 새로운 GameObject 생성

                go.AddComponent<SGTimer>();
                // SGTimer 컴포넌트 추가
            }

            DontDestroyOnLoad(go);
            // 씬 변경 시 SGTimer가 파괴되지 않도록 유지

            s_instance = go.GetComponent<SGTimer>();
            // SGTimer 인스턴스 참조
        }
    }

    private const float FIXED_DELTA_TIME_BASE = (1f / 60f);
    // 고정 프레임(delta time)의 기준값 (60 FPS 기준)

    [SerializeField]
    private SGUtil.TIME m_deltaTimeType = SGUtil.TIME.DELTA_TIME;
    // delta time의 유형 (DELTA_TIME, UNSCALED_DELTA_TIME, FIXED_DELTA_TIME 중 하나)

    private float _deltaTime;
    // Time.deltaTime 값 저장

    private float _deltaTimeUnscaled;
    // Time.unscaledDeltaTime 값 저장

    private float _deltaTimeFixed;
    // 고정된 delta time 값

    private float _deltaFrameCount;
    // 프레임 단위로 계산된 delta frame 값

    private float _deltaFrameCountUnscaled;
    // unscaled delta frame 값

    private float _deltaFrameCountFixed;
    // 고정된 delta frame 값

    private float _totalFrameCount;
    // delta frame의 누적 합

    private float _totalFrameCountUnscaled;
    // unscaled frame의 누적 합

    private float _totalFrameCountFixed;
    // 고정된 frame 값의 누적 합

    private bool _pausing;
    // 타이머의 일시 정지 상태

    public SGUtil.TIME deltaTimeType { get { return m_deltaTimeType; } set { m_deltaTimeType = value; } }
    // delta time 계산 방법을 설정하거나 반환

    public bool pausing { get { return _pausing; } }
    // 현재 일시 정지 상태를 반환

    public float deltaTime
    {
        get
        {
            if (_pausing)
            {
                return 0f;
                // 정지 상태면 delta time은 0
            }

            switch (m_deltaTimeType)
            {
                case SGUtil.TIME.UNSCALED_DELTA_TIME:
                    return _deltaTimeUnscaled;
                // 사용하는 시간이 unscaled time인 경우

                case SGUtil.TIME.FIXED_DELTA_TIME:
                    return _deltaTimeFixed;
                // 사용하는 시간이 고정 time인 경우

                case SGUtil.TIME.DELTA_TIME:
                default:
                    return _deltaTime;
                    // 기본적으로 delta time 반환
            }
        }
    }

    public float deltaFrameCount
    {
        get
        {
            if (_pausing)
            {
                return 0f;
                // 정지 상태면 delta frame count도 0
            }

            switch (m_deltaTimeType)
            {
                case SGUtil.TIME.UNSCALED_DELTA_TIME:
                    return _deltaFrameCountUnscaled;
                // unscaled delta frame 반환

                case SGUtil.TIME.FIXED_DELTA_TIME:
                    return _deltaFrameCountFixed;
                // 고정 delta frame 반환

                case SGUtil.TIME.DELTA_TIME:
                default:
                    return _deltaFrameCount;
                    // 기본 delta frame 반환
            }
        }
    }

    public float totalFrameCount
    {
        get
        {
            switch (m_deltaTimeType)
            {
                case SGUtil.TIME.UNSCALED_DELTA_TIME:
                    return _totalFrameCountUnscaled;
                // unscaled total frame 반환

                case SGUtil.TIME.FIXED_DELTA_TIME:
                    return _totalFrameCountFixed;
                // 고정된 total frame 반환

                case SGUtil.TIME.DELTA_TIME:
                default:
                    return _totalFrameCount;
                    // 기본 total frame 반환
            }
        }
    }

    public void Awake()
    {
        UpdateTimes();
        // 타이머 초기화를 호출
    }

    private void Update()
    {
        UpdateTimes();
        // 매 프레임 시간을 갱신

        Managers.projectileManager.Updateprojectiles(deltaTime);
        // Projectile 관리자 업데이트 호출

        Managers.ShotManager.UpdateShots(deltaTime);
        // Shot 관리자 업데이트 호출
    }

    private void UpdateTimes()
    {
        _deltaTime = Time.deltaTime;
        // Time.deltaTime 값 저장

        _deltaTimeUnscaled = Time.unscaledDeltaTime;
        // Time.unscaledDeltaTime 값 저장

        float nowFps = 0;
        // 현재 프레임 속도(FPS) 초기화

        int vSyncCount = QualitySettings.vSyncCount;
        // VSync 설정 값 확인

        if (vSyncCount == 1)
        {
            nowFps = Screen.currentResolution.refreshRate;
            // VSync 활성화: 화면 주사율 사용
        }
        else if (vSyncCount == 2)
        {
            nowFps = Screen.currentResolution.refreshRate / 2f;
            // VSync 2배 비율 사용
        }
        else
        {
            nowFps = Application.targetFrameRate;
            // 지정된 목표 프레임 속도 사용
        }

        if (nowFps > 0)
        {
            _deltaTimeFixed = FIXED_DELTA_TIME_BASE * (60 / nowFps);
            // 고정된 delta time 계산
        }
        else
        {
            _deltaTimeFixed = 0;
            // FPS가 0이면 delta time을 0으로 설정
        }

        _deltaFrameCount = _deltaTime / FIXED_DELTA_TIME_BASE;
        // delta frame 계산

        _deltaFrameCountUnscaled = _deltaTimeUnscaled / FIXED_DELTA_TIME_BASE;
        // unscaled delta frame 계산

        _deltaFrameCountFixed = _deltaTimeFixed / FIXED_DELTA_TIME_BASE;
        // 고정된 delta frame 계산

        if (_pausing == false)
        {
            _totalFrameCount += _deltaFrameCount;
            // delta frame 값을 총 frame 카운트에 누적

            _totalFrameCountUnscaled += _deltaFrameCountUnscaled;
            // unscaled frame 값 누적

            _totalFrameCountFixed += _deltaFrameCountFixed;
            // 고정된 frame 값 누적
        }
    }

    public void Pause()
    {
        _pausing = true;
        // 타이머 일시 정지
    }

    public void Resume()
    {
        _pausing = false;
        // 타이머 재개
    }
}
