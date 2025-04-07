using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    // 싱글톤 패턴 구현: s_instance라는 정적 변수를 통해 인스턴스를 관리
    static Managers s_instance;

    // Managers 인스턴스를 쉽게 접근할 수 있도록 하는 속성으로, 읽기 전용
    public static Managers Instance { get { return s_instance; } }

    // Managers가 초기화되었는지 확인하는 플래그
    private static bool isInitialized = false;

    // 객체가 생성될 때 호출되는 Unity 라이프 사이클 메서드
    public void Awake()
    {
        // Managers 초기화
        Init();
    }

    // Managers 초기화 메서드
    public static void Init()
    {
        // 싱글톤 객체가 이미 존재하지 않는 경우에만 초기화 수행
        if (s_instance == null)
        {
            // Hierarchy에서 @Managers라는 이름의 GameObject를 찾음
            GameObject go = GameObject.Find("@Managers");

            // @Managers GameObject가 없으면 새로 생성
            if (go == null)
            {
                go = new GameObject { name = "@Managers" }; // 이름은 "@Managers"
                go.AddComponent<Managers>();              // Managers 컴포넌트를 추가
            }

            // 생성한 Managers 객체가 씬 전환으로 파괴되지 않도록 설정
            DontDestroyOnLoad(go);

            // Managers 인스턴스 저장
            s_instance = go.GetComponent<Managers>();

            // 초기화 완료 플래그 설정
            isInitialized = true;
        }
    }

    // Managers가 초기화되었는지 확인하는 메서드
    public bool IsInitialized()
    {
        return isInitialized;
    }

    // Managers 내부에서 관리할 클래스 인스턴스 (SGProjectileManager와 SGShotManager)
    SGProjectileManager _projectileManager = new SGProjectileManager(); // 총알 관리 클래스
    SGShotManager _shotManager = new SGShotManager();                   // 발사 관리 클래스

    // SGShotManager에 접근할 수 있는 정적 속성
    public static SGShotManager ShotManager { get { return Instance?._shotManager; } }

    // SGProjectileManager에 접근할 수 있는 정적 속성
    public static SGProjectileManager projectileManager { get { return Instance?._projectileManager; } }
}
