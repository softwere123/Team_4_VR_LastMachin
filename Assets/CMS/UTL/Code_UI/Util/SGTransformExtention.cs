using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Transform에 대한 확장 메서드를 제공하는 클래스
// Transform 객체에 추가적인 유용한 기능을 제공
public static class SGTransformExtention
{
    // Transform을 초기화하는 메서드 (위치, 회전, 크기 모두 초기화)
    // 위치(worldSpace), 회전(worldSpace), 크기를 기본값으로 리셋
    public static void ResetTransform(this Transform self, bool worldSpace = false)
    {
        self.ResetPosition(worldSpace);  // 위치 리셋
        self.ResetRotation(worldSpace); // 회전 리셋
        self.ResetLocalScale();         // 크기 리셋
    }

    // Transform의 위치를 초기화하는 메서드
    // worldSpace가 true면 월드 위치, false면 로컬 위치를 초기화
    public static void ResetPosition(this Transform self, bool worldSpace = false)
    {
        if (worldSpace)
        {
            // 월드 좌표 위치를 (0, 0, 0)으로 설정
            self.position = SGUtil.VECTOR3_ZERO;
        }
        else
        {
            // 로컬 좌표 위치를 (0, 0, 0)으로 설정
            self.localPosition = SGUtil.VECTOR3_ZERO;
        }
    }

    // Transform의 회전을 초기화하는 메서드
    // worldSpace가 true면 월드 회전, false면 로컬 회전을 초기화
    public static void ResetRotation(this Transform self, bool worldSpace = false)
    {
        if (worldSpace)
        {
            // 월드 좌표 회전을 기본값(Quaternion.identity)으로 설정
            self.rotation = Quaternion.identity;
        }
        else
        {
            // 로컬 좌표 회전을 기본값(Quaternion.identity)으로 설정
            self.localRotation = Quaternion.identity;
        }
    }

    // Transform의 로컬 스케일(크기)을 초기화하는 메서드
    // 로컬 스케일을 기본값 (1, 1, 1)로 설정
    public static void ResetLocalScale(this Transform self)
    {
        self.localScale = SGUtil.VECTOR3_ONE;
    }

    // Transform의 X축 회전 각도를 설정하는 메서드
    public static void SetEulerAnglesX(this Transform self, float x)
    {
        // 현재 Transform의 회전 각도를 가져옴
        Vector3 selfAngles = self.eulerAngles;

        // X축 각도를 설정하고, Y, Z축은 유지
        self.rotation = Quaternion.Euler(x, selfAngles.y, selfAngles.z);
    }

    // Transform의 Y축 회전 각도를 설정하는 메서드
    public static void SetEulerAnglesY(this Transform self, float y)
    {
        // 현재 Transform의 회전 각도를 가져옴
        Vector3 selfAngles = self.eulerAngles;

        // Y축 각도를 설정하고, X, Z축은 유지
        self.rotation = Quaternion.Euler(selfAngles.x, y, selfAngles.z);
    }

    // Transform의 Z축 회전 각도를 설정하는 메서드
    public static void SetEulerAnglesZ(this Transform self, float z)
    {
        // 현재 Transform의 회전 각도를 가져옴
        Vector3 selfAngles = self.eulerAngles;

        // Z축 각도를 설정하고, X, Y축은 유지
        self.rotation = Quaternion.Euler(selfAngles.x, selfAngles.y, z);
    }
}
