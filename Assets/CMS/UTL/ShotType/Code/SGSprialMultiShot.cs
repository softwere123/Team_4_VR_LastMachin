using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGSprialMultiShot : SGBaseShot
{
    // 총알이 한 번에 발사될 방향의 수
    public int spiralWayNum = 4;

    // 시작 각도
    public float startAngle = 180f;

    // 총알을 회전하면서 발사할 때의 각도 변화량
    public float shiftAngle = 5f;

    // 총알 발사 사이의 지연 시간
    public float betweenDealy = 0.2f;

    // 현재 발사된 총알의 인덱스
    private int nowIndex;

    // 발사 지연 타이머
    private float delayTimer;

    // SGBaseShot 클래스를 상속받아 Shot() 메서드를 필수적으로 구현
    public override void Shot()
    {
        // 발사 설정값을 검사 (유효하지 않은 값일 경우 발사하지 않음)
        if (projectileNum <= 0 || projectileSpeed <= 0f || spiralWayNum <= 0)
        {
            return; // 발사 중지
        }

        // 이미 발사 중인지 체크
        if (_shooting)
        {
            return; // 이미 발사 중이라면 추가 발사를 방지
        }

        // 발사 시작 플래그 설정
        _shooting = true;
        nowIndex = 0;            // 발사된 총알 인덱스 초기화
        delayTimer = 0;          // 지연 타이머 초기화
    }

    // 매 프레임마다 호출하여 상태를 업데이트
    protected virtual void Update()
    {
        // 발사 중이 아닌 경우는 업데이트하지 않음
        if (_shooting == false)
        {
            return;
        }

        // 타이머를 감소시킴
        delayTimer -= SGTimer.Instance.deltaTime;

        // 지연 타이머가 0 이하가 되었을 때 (총알을 발사할 시간)
        while (delayTimer <= 0)
        {
            // 각 방향으로 총알을 발사하기 위해 방향마다 회전 각도를 계산
            float spiralWayShiftAngle = 360f / spiralWayNum;

            // 각 방향으로 총알을 발사
            for (int i = 0; i < spiralWayNum; i++)
            {
                // 총알 오브젝트를 가져옴 (풀링 시스템을 사용)
                SGProjectile projectile = GetProjectile(transform.position);

                // 만약 사용할 수 있는 총알이 없다면 발사를 중지
                if (projectile == null)
                {
                    break;
                }

                // 발사 각도를 계산 (시작 각도 + 방향 별 각도 + 회전 변동 각도)
                float angle = startAngle + (spiralWayShiftAngle * i) + (shiftAngle * Mathf.Floor(nowIndex / spiralWayNum));

                // 계산된 방향으로 총알을 발사
                ShotProjectile(projectile, projectileSpeed, angle);

                // 총알의 이동 업데이트 (정확한 발사를 위해 타이머에 맞춰 이동 계산)
                projectile.UpdateMove(-delayTimer);

                // 발사된 총알 인덱스를 증가
                nowIndex++;

                // 아직 설정된 총알 수(projectileNum)의 발사를 완료했다면 발사 중지
                if (nowIndex >= projectileNum)
                {
                    break;
                }
            }

            // 발사가 완료되었음을 시스템에 알림
            FiredShot();

            // 설정된 총알 수를 모두 발사했으면 종료
            if (nowIndex >= projectileNum)
            {
                FinishedShot(); // 발사 종료 처리
                return;         // 업데이트 종료
            }

            // 다음 발사 시간을 계산
            delayTimer += betweenDealy;
        }
    }
}
