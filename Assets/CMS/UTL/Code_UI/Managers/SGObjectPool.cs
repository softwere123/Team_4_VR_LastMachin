using System;
using System.Collections.Generic;
using UnityEngine;

// SGObjectPool: 발사체 등 오브젝트를 풀링 방식으로 관리하는 클래스
// 동일한 객체를 반복적으로 생성/파괴하지 않고 오브젝트를 재사용하여 성능을 최적화함
public class SGObjectPool : MonoBehaviour
{
    // 싱글톤 패턴: 오브젝트 풀링 시스템의 유일한 인스턴스
    static SGObjectPool s_instance;

    // SGObjectPool의 전역(instance) 접근 포인트
    public static SGObjectPool Instance { get { return s_instance; } }

    // 오브젝트 풀 초기화 (싱글톤 구현 및 부모 GameObject 설정)
    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@ObjectPool"); // @ObjectPool이라는 GameObject를 검색
            if (go == null)
            {
                go = new GameObject { name = "@ObjectPool" }; // 없을 경우 새로 생성
                go.AddComponent<SGObjectPool>(); // SGObjectPool 컴포넌트 추가
            }

            // 오브젝트가 씬 전환 시 파괴되지 않도록 설정
            DontDestroyOnLoad(go);

            // 싱글톤 인스턴스 할당
            s_instance = go.GetComponent<SGObjectPool>();
        }
    }

    // 초기 풀 생성 정보를 담는 클래스
    [Serializable]
    private class InitializePool
    {
        public GameObject projectilePrefab = null; // 저장할 발사체 프리팹
        public int initialPoolNum = 0;             // 초기 생성할 발사체 수
    }

    [SerializeField]
    private List<InitializePool> _initializePoolList = null; // 초기화할 풀 리스트

    // 오브젝트 풀링에 필요한 데이터를 담는 로컬 클래스
    private class PoolingParam
    {
        public List<SGProjectile> projectileList = new List<SGProjectile>(1024); // 발사체 리스트
        public int searchStartIndex = 0; // 비활성 발사체를 찾기 위한 시작 인덱스
    }

    // 발사체를 키 값으로 관리하는 딕셔너리 (각 프리팹의 고유 ID로 구분)
    private Dictionary<int, PoolingParam> pooledprojectileDic = new Dictionary<int, PoolingParam>(256);

    // Awake: 오브젝트 풀 초기화 및 초기 발사체 생성
    public void Awake()
    {
        Init();

        // 초기화 리스트를 순회하면서 풀을 생성
        if (_initializePoolList != null && _initializePoolList.Count > 0)
        {
            for (int i = 0; i < _initializePoolList.Count; i++)
            {
                CreatePool(_initializePoolList[i].projectilePrefab, _initializePoolList[i].initialPoolNum);
            }
        }
    }

    // 특정 발사체 프리팹으로 풀 생성
    public void CreatePool(GameObject goPrefab, int createNum)
    {
        for (int i = 0; i < createNum; i++)
        {
            // 풀에서 오브젝트를 가져오며 강제로 인스턴스화
            SGProjectile projectile = Getprojectile(goPrefab, SGUtil.VECTOR3_ZERO, true);

            // 발사체가 생성되지 않으면 초기 생성 중단
            if (projectile == null)
            {
                break;
            }

            // 생성된 발사체를 비활성화 상태로 반환
            ReleaseProjectile(projectile);
        }
    }

    // 발사체를 풀에서 가져오거나 새로 생성
    public SGProjectile Getprojectile(GameObject goPrefab, Vector3 position, bool forceInstantiate = false)
    {
        if (goPrefab == null)
        {
            return null; // 프리팹이 설정되지 않은 경우
        }

        SGProjectile projectile = null;
        int key = goPrefab.GetInstanceID(); // 프리팹의 고유 ID를 키로 사용

        // 발사체 키가 딕셔너리에 없으면 생성
        if (!pooledprojectileDic.ContainsKey(key))
        {
            pooledprojectileDic.Add(key, new PoolingParam());
        }

        PoolingParam poolParam = pooledprojectileDic[key]; // 키에 해당하는 풀 정보 가져오기

        // 풀에서 비활성화된 오브젝트 검색 (forceInstantiate가 false일 경우)
        if (!forceInstantiate && poolParam.projectileList.Count > 0)
        {
            if (poolParam.searchStartIndex < 0 || poolParam.searchStartIndex >= poolParam.projectileList.Count)
            {
                poolParam.searchStartIndex = poolParam.projectileList.Count - 1;
            }

            // 리스트를 역순으로 탐색 (비활성 오브젝트 찾기)
            for (int i = poolParam.searchStartIndex; i >= 0; i--)
            {
                if (poolParam.projectileList[i] == null || poolParam.projectileList[i].gameObject == null)
                {
                    poolParam.projectileList.RemoveAt(i); // 존재하지 않는 오브젝트 제거
                    continue;
                }
                if (!poolParam.projectileList[i].isActive)
                {
                    poolParam.searchStartIndex = i - 1; // 다음 검색 시작 인덱스 설정
                    projectile = poolParam.projectileList[i];
                    break;
                }
            }

            // 추가 탐색 수행 (남은 리스트 확인)
            if (projectile == null)
            {
                for (int i = poolParam.projectileList.Count - 1; i > poolParam.searchStartIndex; i--)
                {
                    if (poolParam.projectileList[i] == null || poolParam.projectileList[i].gameObject == null)
                    {
                        poolParam.projectileList.RemoveAt(i);
                        continue;
                    }
                    if (!poolParam.projectileList[i].isActive)
                    {
                        poolParam.searchStartIndex = i - 1;
                        projectile = poolParam.projectileList[i];
                        break;
                    }
                }
            }
        }

        // 풀에서 사용 가능한 발사체가 없으면 새로 생성
        if (projectile == null)
        {
            GameObject go = Instantiate(goPrefab, transform);
            projectile = go.GetComponent<SGProjectile>();

            // SGProjectile 컴포넌트가 존재하지 않으면 추가
            if (projectile == null)
            {
                projectile = go.AddComponent<SGProjectile>();
            }

            // 풀에 추가
            poolParam.projectileList.Add(projectile);
            poolParam.searchStartIndex = poolParam.projectileList.Count - 1;
        }

        // 발사체 초기화
        projectile.transform.SetPositionAndRotation(position, SGUtil.QUATERNION_IDENTITY);
        projectile.SetActive(true); // 활성화

        // 발사체를 전역 관리 시스템에 등록
        Managers.projectileManager.Addprojectile(projectile);

        return projectile;
    }

    // 발사체를 풀로 반환하거나 파괴
    public void ReleaseProjectile(SGProjectile projectile, bool destroy = false)
    {
        if (projectile == null || projectile.gameObject == null)
        {
            return; // 유효하지 않은 경우 처리 중단
        }

        // 발사 완료 처리
        projectile.OnFinishedShot();

        // 발사체를 전역 관리자에서 제거
        Managers.projectileManager.Removeprojectile(projectile, destroy);

        // 파괴 설정이 활성화된 경우
        if (destroy)
        {
            Destroy(projectile.gameObject); // 오브젝트 제거
            Destroy(projectile);           // SGProjectile 제거
            projectile = null;
            return;
        }

        // 비활성 상태로 풀에 반환
        if (projectile != null)
            projectile.SetActive(false);
    }

}
