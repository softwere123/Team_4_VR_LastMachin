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
            return;
        }

        _shooting = true;

        for (int i = 0; i < projectileNum; i++)
        {
            // 탄환을 풀에서 가져오기
            SGProjectile projectile = GetProjectile(transform.position);
            if (projectile == null)
            {
                continue;
            }

            float angle = 0f;

            // 탄환 발사
            ShotProjectile(
                projectile,
                projectileSpeed,
                angle,
                false, // homing
                null,
                0f,
                false,
                0f, 0f, false
            );

            // ✅ 콜백을 여기서 정확히 호출
            if (_shotCtrl != null)
            {
                _shotCtrl.onProjectileFired?.Invoke();
                Debug.Log("💣 탄환 실제 발사됨 - 콜백 호출 성공");
            }
            else
            {
                Debug.LogWarning("❌ _shotCtrl가 null이라 콜백 호출 실패!");
            }
        }

        FiredShot();
        FinishedShot();
    }
}

