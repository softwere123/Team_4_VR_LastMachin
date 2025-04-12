using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnDelay = 5f;

    private void Start()
    {
        // 일정 시간이 지난 후에 SpawnObject 함수를 호출합니다.
        StartCoroutine(SpawnObjectAfterDelay());
    }

    private IEnumerator SpawnObjectAfterDelay()
    {
        // spawnDelay 초 대기
        yield return new WaitForSeconds(spawnDelay);

        // objectToSpawn 프리팹을 생성합니다.
        Instantiate(objectToSpawn, transform.position, transform.rotation);
    }
}