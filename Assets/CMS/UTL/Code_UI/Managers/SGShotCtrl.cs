using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SGShotCtrl : MonoBehaviour
{
    public bool _shooting;
    // 현재 발사 상태를 나타내는 변수

    public enum UpdateStep
    {
        StartDelay,  // 발사 시작 전 대기 상태
        StartShot,   // 발사 시작
        WaitDelay,   // 다음 발사를 위해 대기
        UpdateIndex, // 다음 발사체로 업데이트
        FinishShot,  // 발사 완료
    }

    [Serializable]
    public class ShotInfo
    {
        public SGBaseShot shotObj;
        // 발사체 오브젝트

        public float afterDelay = 0.1f;
        // 발사체 발사 후 대기 시간
    }

    public SGUtil.AXIS axisMove = SGUtil.AXIS.X_AND_Y;
    // 발사체의 이동 축 (X-Y 축으로 기본 설정)

    public bool inheritAngle = false;
    // 발사체 발사 시 부모 각도를 상속받을지 여부

    public bool startOnAwake = true;
    // 오브젝트 생성 시 자동 발사 여부

    public float startOnAwakeDelay = 1f;
    // 생성 후 발사 전 대기 시간

    public bool startOnEnable = false;
    // 활성화 시 자동 발사 여부

    public float startOnEnableDelay = 1f;
    // 활성화 후 발사 전 대기 시간

    public bool loop = true;
    // 발사 루프 여부

    public List<ShotInfo> shotList = new List<ShotInfo>();
    // 발사체의 발사 설정 리스트

    public UpdateStep updateStep;
    // 발사 상태를 나타내는 변수 (초기값 설정 가능)

    private int nowIndex;
    // 현재 발사중인 발사체의 인덱스

    private float delayTimer;
    // 발사 대기 타이머

    private bool isInitialized = false;
    // 초기화 완료 여부

    private void Start()
    {
        if (startOnAwake)
        {
            StartShotRoutine(startOnAwakeDelay);
            // Awake 시 발사 루틴 시작
        }
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForSingleton());
        // Singleton이 준비될 때까지 대기
    }

    private IEnumerator WaitForSingleton()
    {
        while (!isInitialized)
        {
            if (Managers.Instance != null && Managers.Instance.IsInitialized())
            {
                isInitialized = true;
                // 초기화 완료 설정
            }
            yield return null;
        }

        Managers.ShotManager.AddShot(this);
        // Managers의 발사 관리자에 추가

        if (startOnEnable)
        {
            StartShotRoutine(startOnEnableDelay);
            // Enable 시 발사 루틴 시작
        }
    }

    private void OnDestroy()
    {
        _shooting = false;

        if (Managers.ShotManager != null)
        {
            Managers.ShotManager.RemoveShot(this);
            // Managers에서 발사 제거
        }
    }

    public void UpdateShot(float deltaTime)
    {
        if (_shooting == false)
        {
            return;
            // 발사 중이 아니면 처리 종료
        }

        if (updateStep == UpdateStep.StartDelay)
        {
            if (delayTimer > 0f)
            {
                delayTimer -= deltaTime;
                // 대기 타이머 감소
                return;
            }
            else
            {
                delayTimer = 0f;
                updateStep = UpdateStep.StartShot;
                // 발사 시작 상태로 전환
            }
        }

        ShotInfo nowShotInfo = shotList[nowIndex];
        // 현재 발사체 설정 가져오기

        if (updateStep == UpdateStep.StartShot)
        {
            if (nowShotInfo.shotObj != null)
            {
                nowShotInfo.shotObj.SetShotCtrl(this);
                // 발사체의 컨트롤러 설정
                nowShotInfo.shotObj.Shot();
                // 발사 실행
            }

            delayTimer = 0f;
            updateStep = UpdateStep.WaitDelay;
            // 대기 상태로 전환
        }

        if (updateStep == UpdateStep.WaitDelay)
        {
            if (nowShotInfo.afterDelay > 0 && nowShotInfo.afterDelay > delayTimer)
            {
                delayTimer += deltaTime;
                // 후행 대기 타이머 증가
            }
            else
            {
                nowShotInfo.afterDelay = 0.1f;
                delayTimer = 0f;
                updateStep = UpdateStep.UpdateIndex;
                // 다음 발사체로 이동
            }
        }

        if (updateStep == UpdateStep.UpdateIndex)
        {
            if (loop || nowIndex < shotList.Count - 1)
            {
                nowIndex = (int)Mathf.Repeat(nowIndex + 1f, shotList.Count);
                // 루프를 처리하여 인덱스 갱신
                updateStep = UpdateStep.StartShot;
            }
            else
            {
                updateStep = UpdateStep.FinishShot;
                // 발사가 완료된 상태로 전환
            }
        }

        if (updateStep == UpdateStep.StartShot)
        {
            UpdateShot(deltaTime);
            // 발사 진행
        }
        else if (updateStep == UpdateStep.FinishShot)
        {
            _shooting = false;
            // 발사 종료
        }
    }

    public void StartShotRoutine(float startDelay = 0f)
    {
        if (shotList == null || shotList.Count <= 0)
        {
            // 샷 리스트가 비어있음
            return;
        }

        bool enableShot = false;
        for (int i = 0; i < shotList.Count; i++)
        {
            if (shotList[i].shotObj != null)
            {
                enableShot = true;
                // 샷 리스트에 유효한 발사체가 있음
                break;
            }
        }

        if (enableShot == false)
        {
            // 오브젝트 셋팅 안됨
            return;
        }

        if (loop)
        {
            bool enableDelay = false;
            for (int i = 0; i < shotList.Count; i++)
            {
                if (0f < shotList[i].afterDelay)
                {
                    enableDelay = true;
                    // 하나 이상의 발사체에 딜레이가 있음
                    break;
                }
            }
            if (enableDelay == false)
            {
                // 샷 딜레이 없음
                return;
            }
        }

        if (_shooting)
        {
            // 이미 발사 중임
            return;
        }

        _shooting = true;
        // 발사 상태 true로 설정
        delayTimer = startDelay;
        // 발사 전 딜레이 타이머 설정
        updateStep = delayTimer > 0f ? UpdateStep.StartDelay : UpdateStep.StartShot;
        // 발사 상태 초기화

        nowIndex = 0;
        // 발사체 인덱스 초기화
    }
}
