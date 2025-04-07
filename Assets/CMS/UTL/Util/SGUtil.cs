using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유틸리티 클래스: 게임에서 자주 사용되는 수학적/변환 작업 및 상수를 제공
public static class SGUtil
{
    // Vector3 상수 (자주 사용하는 벡터 값들을 상수로 정의)
    public static readonly Vector3 VECTOR3_ZERO = Vector3.zero;         // (0, 0, 0) 벡터
    public static readonly Vector3 VECTOR3_ONE = Vector3.one;           // (1, 1, 1) 벡터
    public static readonly Vector3 VECTOR3_HALF = new Vector3(0.5f, 0.5f, 0.5f); // (0.5, 0.5, 0.5) 벡터

    // Vector2 상수
    public static readonly Vector2 VECTOR2_ZERO = Vector2.zero;         // (0, 0) 벡터
    public static readonly Vector2 VECTOR2_ONE = Vector2.one;           // (1, 1) 벡터
    public static readonly Vector2 VECTOR2_HALF = new Vector2(0.5f, 0.5f); // (0.5, 0.5) 벡터

    // Quaternion 상수
    public static readonly Quaternion QUATERNION_IDENTITY = Quaternion.identity; // 초기화된 Quaternion (회전 없음)

    // 좌표계 변환이나 이동에 사용할 축을 정의한 열거형 (2개 축만 사용)
    public enum AXIS
    {
        X_AND_Y, // XY 평면에서 동작
        X_AND_Z, // XZ 평면에서 동작
    }

    // 시간의 종류를 정의한 열거형
    public enum TIME
    {
        DELTA_TIME,           // 일반 Delta Time
        UNSCALED_DELTA_TIME,  // TimeScale의 영향을 받지 않는 Delta Time
        FIXED_DELTA_TIME,     // Fixed Update에서 사용되는 Delta Time
    }

    // 두 Transform 간의 각도를 계산
    // 두 Transform 사이에서 특정 축을 기준으로 회전 각도를 반환
    public static float GetAngleFromTwoPosition(Transform fromTrans, Transform toTrans, AXIS axisMove)
    {
        switch (axisMove)
        {
            case AXIS.X_AND_Y:
                // XY 평면에서 회전을 계산 (결과는 Z축 방향)
                return GetZangleFromTwoPosition(fromTrans, toTrans);
            case AXIS.X_AND_Z:
                // XZ 평면에서 회전을 계산 (결과는 Y축 방향)
                return GetYangleFromTwoPosition(fromTrans, toTrans);
            default:
                return 0f; // 기본값은 0도
        }
    }

    // 두 Transform 사이의 XY 평면(Z축 방향)의 각도를 계산
    private static float GetZangleFromTwoPosition(Transform fromTrans, Transform toTrans)
    {
        // Null 검사: 유효하지 않은 Transform이면 0도 반환
        if (fromTrans == null || toTrans == null)
        {
            return 0f;
        }

        // 두 Transform의 X, Y 거리 차이를 계산
        float xDistance = toTrans.position.x - fromTrans.position.x;
        float yDistance = toTrans.position.y - fromTrans.position.y;

        // Mathf.Atan2를 사용하여 탄젠트 각도 계산 (라디안을 각도로 변환)
        float angle = (Mathf.Atan2(yDistance, xDistance) * Mathf.Rad2Deg) - 90f;

        // 각도를 0도~360도로 정규화
        angle = GetNormalizedAngle(angle);

        return angle; // 계산된 각도 반환
    }

    // 두 Transform 사이의 XZ 평면(Y축 방향)의 각도를 계산
    private static float GetYangleFromTwoPosition(Transform fromTrans, Transform toTrans)
    {
        // Null 검사: 유효하지 않은 Transform이면 0도 반환
        if (fromTrans == null || toTrans == null)
        {
            return 0f;
        }

        // 두 Transform의 X, Z 거리 차이를 계산
        float xDistance = toTrans.position.x - fromTrans.position.x;
        float zDistance = toTrans.position.z - fromTrans.position.z;

        // Mathf.Atan2를 사용하여 탄젠트 각도 계산 (라디안을 각도로 변환)
        float angle = (Mathf.Atan2(zDistance, xDistance) * Mathf.Rad2Deg) - 90f;

        // 각도를 0도~360도로 정규화
        angle = GetNormalizedAngle(angle);

        return angle; // 계산된 각도 반환
    }

    // 각도를 0도~360도로 정규화하는 메서드
    public static float GetNormalizedAngle(float angle)
    {
        // 각도가 0보다 작으면 계속 360씩 더함
        while (angle < 0f)
        {
            angle += 360f;
        }

        // 각도가 360보다 크면 계속 360씩 뺌
        while (360f < angle)
        {
            angle -= 360f;
        }

        return angle; // 정규화된 각도 반환
    }
}
