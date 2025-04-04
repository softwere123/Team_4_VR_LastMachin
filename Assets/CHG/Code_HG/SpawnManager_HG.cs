using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private Vector3 defaultSpawn = new Vector3(-14, 2, -13); // 기본 스폰 위치

    // 씬별 스폰 위치 저장
    private Dictionary<string, Vector3> spawnPositions = new Dictionary<string, Vector3>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬별 스폰 위치 저장
    public void SetSpawnPosition(string sceneName, Vector3 position)
    {
        if (spawnPositions.ContainsKey(sceneName))
        {
            spawnPositions[sceneName] = position;
        }
        else
        {
            spawnPositions.Add(sceneName, position);
        }
    }

    // 현재 씬의 스폰 위치 가져오기
    public Vector3 GetSpawnPosition(string sceneName)
    {
        if (spawnPositions.ContainsKey(sceneName))
        {
            return spawnPositions[sceneName];
        }
        return defaultSpawn; // 기본 위치 반환
    }
}
