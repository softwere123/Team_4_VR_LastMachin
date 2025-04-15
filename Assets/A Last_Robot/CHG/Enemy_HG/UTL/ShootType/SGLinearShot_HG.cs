using UnityEngine;

/// <summary>
/// 일직선으로 탄환을 발사하는 기본 탄막 샷 클래스
/// </summary>
public class SGLinearShot_HG : SGBaseShot
{
    public override void Shot()
    {
        if (projectileNum <= 0 || projectilePrefab == null)
        {
            return; // 설정이 잘못됨
        }

        _shooting = true;

        for (int i = 0; i < projectileNum; i++)
        {
            // 탄환을 풀에서 가져오기
            SGProjectile projectile = GetProjectile(transform.position);
            if (projectile == null)
            {
                continue; // 풀에서 가져오기 실패
            }

            // 각도 없이 직선 발사
            float angle = 0f;

            // 탄환 발사 (호밍/사인웨이브 등 옵션은 모두 비활성화)
            ShotProjectile(
                projectile,
                projectileSpeed,
                angle,
                false, // homing
                null,
                0f,    // homingAngleSpeed
                false, // sinWave
                0f, 0f, false // sinWave params
            );
        }

        // 발사 완료 처리
        FiredShot();
        FinishedShot();
    }
}
