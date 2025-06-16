using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGShotCtrl : MonoBehaviour
{
    // 🔔 발사체가 실제 발사되었을 때 외부에서 호출할 수 있도록 이벤트 제공
    public System.Action onProjectileFired;

    // 🔫 외부에서 발사를 시작/중지할 수 있는 속성 (setter 내부에서 로직 처리)
    public bool Shooting
    {
        get => _shooting;
        set
        {
            if (value)
                StartShotRoutine();  // true일 때 탄막 시작
            else
                StopShot();          // false일 때 탄막 중지
        }
    }

    public bool _shooting; // 현재 발사 중인지 여부를 저장하는 내부 변수

    // 💡 탄막 상태를 정의한 열거형
    public enum UpdateStep
    {
        StartDelay,
        StartShot,
        WaitDelay,
        UpdateIndex,
        FinishShot,
    }

    [Serializable]
    public class ShotInfo
    {
        public SGBaseShot shotObj;      // 발사할 탄막 오브젝트
        public float afterDelay = 0.1f; // 다음 탄막까지 대기 시간
    }

    // 🔁 방향 및 설정 관련 옵션들
    public SGUtil.AXIS axisMove = SGUtil.AXIS.X_AND_Y;
    public bool inheritAngle = false;
    public bool startOnAwake = true;
    public float startOnAwakeDelay = 1f;
    public bool startOnEnable = false;
    public float startOnEnableDelay = 1f;
    public bool loop = false;

    public List<ShotInfo> shotList = new List<ShotInfo>(); // 여러 발사 정보를 리스트로 관리

    // 🔄 내부 상태 관리용 변수들
    public UpdateStep updateStep;
    private int nowIndex;
    private float delayTimer;

    private bool isInitialized = false;

    // 🟢 Start에서 초기 탄막 자동 실행 여부 확인
    private void Start()
    {
        if (startOnAwake)
        {
            StartShotRoutine(startOnAwakeDelay);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForSingleton());
    }

    // ✅ 싱글톤 매니저 초기화 기다리는 루틴
    private IEnumerator WaitForSingleton()
    {
        while (!isInitialized)
        {
            if (Managers.Instance != null && Managers.Instance.IsInitialized())
            {
                isInitialized = true;
            }
            yield return null;
        }

        Managers.ShotManager.AddShot(this); // 매니저에 등록

        if (startOnEnable)
        {
            StartShotRoutine(startOnEnableDelay);
        }
    }

    private void OnDestroy()
    {
        _shooting = false;

        if (Managers.ShotManager != null)
        {
            Managers.ShotManager.RemoveShot(this); // 파괴 시 매니저에서 제거
        }
    }

    // 🔁 프레임마다 탄막 갱신
    private void Update()
    {
        UpdateShot(Time.deltaTime);
    }

    // 🔄 탄막 실행 메인 로직
    public void UpdateShot(float deltaTime)
    {
        if (_shooting == false)
        {
            return;
        }

        ShotInfo nowShotInfo = shotList[nowIndex];

        // ⏳ 1. 시작 지연 처리
        if (updateStep == UpdateStep.StartDelay)
        {
            if (delayTimer > 0f)
            {
                delayTimer -= deltaTime;
                return;
            }
            else
            {
                delayTimer = 0f;
                updateStep = UpdateStep.StartShot;
            }
        }

        // 🔫 2. 실제 발사 처리
        if (updateStep == UpdateStep.StartShot)
        {
            if (nowShotInfo.shotObj != null)
            {
                nowShotInfo.shotObj.SetShotCtrl(this);
                nowShotInfo.shotObj.Shot();
            }

            delayTimer = 0f;
            updateStep = UpdateStep.WaitDelay;
        }

        // ⏱️ 3. 발사 후 딜레이
        if (updateStep == UpdateStep.WaitDelay)
        {
            if (nowShotInfo.afterDelay > 0 && nowShotInfo.afterDelay > delayTimer)
            {
                delayTimer += deltaTime;
            }
            else
            {
                delayTimer = 0f;
                updateStep = UpdateStep.UpdateIndex;
            }
        }

        // 🔁 4. 다음 탄막 인덱스로
        if (updateStep == UpdateStep.UpdateIndex)
        {
            if (loop || nowIndex < shotList.Count - 1)
            {
                nowIndex = (int)Mathf.Repeat(nowIndex + 1f, shotList.Count);
                updateStep = UpdateStep.StartShot;
            }
            else
            {
                updateStep = UpdateStep.FinishShot;
            }
        }

        // 📌 재귀적 실행
        if (updateStep == UpdateStep.StartShot)
        {
            UpdateShot(deltaTime);
        }
        else if (updateStep == UpdateStep.FinishShot)
        {
            _shooting = false;
        }
    }

    /// <summary>
    /// 🚀 발사 루틴을 시작
    /// </summary>
    public void StartShotRoutine(float startDelay = 0f)
    {
        if (shotList == null || shotList.Count <= 0)
        {
            return;
        }

        bool enableShot = false;
        for (int i = 0; i < shotList.Count; i++)
        {
            if (shotList[i].shotObj != null)
            {
                enableShot = true;
                break;
            }
        }
        if (!enableShot) return;

        if (loop)
        {
            bool enableDelay = false;
            for (int i = 0; i < shotList.Count; i++)
            {
                if (shotList[i].afterDelay > 0f)
                {
                    enableDelay = true;
                    break;
                }
            }
            if (!enableDelay) return;
        }

        if (_shooting) return;

        _shooting = true;
        delayTimer = startDelay;
        updateStep = delayTimer > 0f ? UpdateStep.StartDelay : UpdateStep.StartShot;
        nowIndex = 0;
    }

    /// <summary>
    /// 🛑 발사 중지
    /// </summary>
    public void StopShot()
    {
        _shooting = false;
        updateStep = UpdateStep.FinishShot;
    }

    /// <summary>
    /// ✅ 외부에서 호출 가능한 단순 발사 시작 메서드 (추가된 부분)
    /// </summary>
    public void StartShot()
    {
        StartShotRoutine(); // 내부 루틴 실행
    }
}


