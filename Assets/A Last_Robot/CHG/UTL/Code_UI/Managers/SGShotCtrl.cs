using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SGBaseShot을 통해 다양한 탄막을 발사하는 컨트롤러
/// SGUtil.AXIS 기반으로 총알 발사 방향을 지정할 수 있음
/// </summary>
public class SGShotCtrl : MonoBehaviour
{
    [Header("🔫 발사할 샷 리스트 (SGBaseShot 상속)")]
    public List<SGBaseShot> shotList; // 다양한 탄막들을 리스트로 연결

    [Header("🕒 연속 발사 간격")]
    public float interval = 0.1f; // 각 샷 사이의 간격 (초 단위)

    [Header("⚙️ 자동 발사 여부")]
    public bool autoStart = false; // true면 Start 시 자동 발사 실행

    [Header("🔀 발사 방향 및 회전 설정")]
    public SGUtil.AXIS axisMove = SGUtil.AXIS.X_AND_Z; // SGUtil에 정의된 발사 방향 축
    public bool inheritAngle = false;                  // 부모의 회전을 상속할지 여부

    void Start()
    {
        // 자동 시작 설정이 되어 있다면 시작하자마자 발사 루틴 실행
        if (autoStart)
        {
            StartShotRoutine();
        }
    }

    /// <summary>
    /// 외부에서 직접 호출해서 탄막 발사 시작 (애니메이션 이벤트 등)
    /// </summary>
    public void StartShotRoutine()
    {
        StartCoroutine(ShotRoutine());
    }

    /// <summary>
    /// 등록된 모든 샷을 interval 간격으로 순차 실행
    /// </summary>
    private IEnumerator ShotRoutine()
    {
        for (int i = 0; i < shotList.Count; i++)
        {
            if (shotList[i] != null)
            {
                shotList[i].Shot(); // 실제 샷 실행
            }

            yield return new WaitForSeconds(interval); // 다음 발사까지 대기
        }
    }

    /// <summary>
    /// SGShotManager에서 매 프레임 호출하는 업데이트 함수
    /// </summary>
    public void UpdateShot(float deltaTime)
    {
        foreach (var shot in shotList)
        {
            if (shot != null && shot.shooting)
            {
                shot.UpdateShot(deltaTime); // 각 샷 타입의 프레임별 동작 처리 (예: 추적, 파동)
            }
        }
    }
}
