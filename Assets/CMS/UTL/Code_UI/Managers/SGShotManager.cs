using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발사(SGShotCtrl) 관리 클래스
public class SGShotManager
{
    private List<SGShotCtrl> m_shotList = new List<SGShotCtrl>(2048);
    // 발사체 컨트롤러(SGShotCtrl)를 저장하는 리스트 (초기 용량 2048)

    private HashSet<SGShotCtrl> m_shotHashSet = new HashSet<SGShotCtrl>();
    // 중복 추가를 방지하기 위한 해시셋 구조

    public int activeShotCount { get { return m_shotList.Count; } }
    // 현재 활성화된 발사체 컨트롤러 수를 반환하는 프로퍼티

    public void UpdateShots(float deltaTime)
    {
        // Shot 리스트를 순회하며 업데이트 실행
        for (int i = m_shotList.Count - 1; i >= 0; i--)
        {
            SGShotCtrl shotCtrl = m_shotList[i]; // 현재 발사체 컨트롤러 가져오기

            if (shotCtrl == null)
            {
                m_shotList.Remove(shotCtrl); // Null 오브젝트 제거
                continue;
            }

            shotCtrl.UpdateShot(deltaTime); // 발사체 컨트롤러의 업데이트 호출
        }
    }

    public void AddShot(SGShotCtrl shotCtrl)
    {
        // 이미 추가된 발사체인지 확인
        if (m_shotHashSet.Contains(shotCtrl))
        {
            return; // 이미 추가된 경우 다시 추가하지 않음
        }

        m_shotList.Add(shotCtrl); // 리스트에 추가
        m_shotHashSet.Add(shotCtrl); // 해시셋에 추가
    }

    public void RemoveShot(SGShotCtrl shotCtrl)
    {
        // 발사체가 해시셋에 포함되어 있지 않은 경우
        if (m_shotHashSet.Contains(shotCtrl) == false)
        {
            return; // 아무 동작도 하지 않음
        }

        m_shotList.Remove(shotCtrl); // 리스트에서 발사체 제거
        m_shotHashSet.Remove(shotCtrl); // 해시셋에서도 제거
    }
}
