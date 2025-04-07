using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발사체(SGProjectile)를 관리하는 클래스
public class SGProjectileManager
{
    private List<SGProjectile> projectileList = new List<SGProjectile>(2000);
    // 발사체들을 순서대로 저장하는 리스트 (최대 용량 초기값 2000)

    private HashSet<SGProjectile> projectileHashSet = new HashSet<SGProjectile>();
    // 발사체 중복 추가 방지를 위한 해시셋

    public int activeprojectileCount { get { return projectileList.Count; } }
    // 현재 활성화된 발사체 수를 반환하는 읽기 전용 프로퍼티

    public void Updateprojectiles(float deltaTime)
    {
        // 발사체 리스트를 역순으로 순회하며 업데이트
        for (int i = projectileList.Count - 1; i >= 0; i--)
        {
            SGProjectile projectile = projectileList[i]; // 현재 발사체 가져오기

            if (projectile == null)
            {
                projectileList.Remove(projectile); // 비활성화된 발사체를 리스트에서 제거
                continue;
            }
            projectile.UpdateMove(deltaTime); // 발사체의 이동 업데이트 호출
        }
    }

    public void Addprojectile(SGProjectile projectile)
    {
        if (projectileHashSet.Contains(projectile))
        {
            return; // 이미 추가된 발사체라면 중복 추가를 방지
        }
        projectileList.Add(projectile); // 리스트에 발사체 추가
        projectileHashSet.Add(projectile); // 해시셋에도 발사체 추가
    }

    public void Removeprojectile(SGProjectile projectile, bool destroy)
    {
        if (projectileHashSet.Contains(projectile) == false)
        {
            // 해시셋에 없는 발사체는 제거 예약 처리
            projectile.reserveReleaseOnShot = true; // 발사체 해제 예약
            projectile.reserveReleaseOnShotIsDestroy = destroy; // 제거 또는 파괴 여부 설정
            return;
        }
        projectileList.Remove(projectile); // 리스트에서 발사체 제거
        projectileHashSet.Remove(projectile); // 해시셋에서 발사체 제거
    }
}
