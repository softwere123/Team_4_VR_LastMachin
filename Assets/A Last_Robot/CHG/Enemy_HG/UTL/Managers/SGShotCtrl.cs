using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGShotCtrl : MonoBehaviour
{
    public bool _shooting;

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
        public SGBaseShot shotObj;
        public float afterDelay = 0.1f;
    }

    public SGUtil.AXIS axisMove = SGUtil.AXIS.X_AND_Y;
    public bool inheritAngle = false;
    public bool startOnAwake = true;
    public float startOnAwakeDelay = 1f;
    public bool startOnEnable = false;
    public float startOnEnableDelay = 1f;
    public bool loop = true;

    public List<ShotInfo> shotList = new List<ShotInfo>();

    public UpdateStep updateStep;
    private int nowIndex;
    private float delayTimer;

    private bool isInitialized = false;

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

        Managers.ShotManager.AddShot(this);

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
            Managers.ShotManager.RemoveShot(this);
        }
    }

    public void UpdateShot(float deltaTime)
    {
        if (_shooting == false)
        {
            return;
        }

        ShotInfo nowShotInfo = shotList[nowIndex];

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

        if (updateStep == UpdateStep.WaitDelay)
        {
            // 설정한 afterDelay 만큼 대기
            if (nowShotInfo.afterDelay > 0 && nowShotInfo.afterDelay > delayTimer)
            {
                delayTimer += deltaTime;
            }
            else
            {
                // ❗ 수정됨: 더 이상 afterDelay를 0.1로 강제로 바꾸지 않음
                delayTimer = 0f;
                updateStep = UpdateStep.UpdateIndex;
            }
        }

        if (updateStep == UpdateStep.UpdateIndex)
        {
            // 다음 인덱스로 넘어가기
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

        if (updateStep == UpdateStep.StartShot)
        {
            UpdateShot(deltaTime); // 재귀 호출 (새 인덱스 기준으로 바로 발사)
        }
        else if (updateStep == UpdateStep.FinishShot)
        {
            _shooting = false;
        }
    }

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
        if (enableShot == false)
        {
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
                    break;
                }
            }
            if (enableDelay == false)
            {
                return;
            }
        }

        if (_shooting)
        {
            return;
        }

        _shooting = true;
        delayTimer = startDelay;
        updateStep = delayTimer > 0f ? UpdateStep.StartDelay : UpdateStep.StartShot;

        nowIndex = 0;
    }

    // 필요 시 수동으로 정지시킬 수 있는 함수
    public void StopShot()
    {
        _shooting = false;
        updateStep = UpdateStep.FinishShot;
    }
}
