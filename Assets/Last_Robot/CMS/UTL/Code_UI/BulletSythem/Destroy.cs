using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float destroyDelay = 5.0f; // 몇 초 후에 오브젝트를 제거할지 설정할 수 있습니다.

    void Start()
    {
        // destroyDelay 초 후에 현재 스크립트가 연결된 게임 오브젝트를 제거합니다.
        Destroy(gameObject, destroyDelay);
    }
}