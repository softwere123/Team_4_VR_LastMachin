using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    //3초 이후에 삭제

    public float SpawnTime = 3.0f;
    public float SpawnCheckTime = 0.0f;
    void Start()
    {
        SpawnCheckTime = SpawnTime;
    }
  
    
    public GameObject Prefabs;
    // Start is called before the first frame update
    void Update()
    {
        for (int i = 1; i <= 1; i++)
        {
            SpawnCheckTime -= Time.deltaTime;
            if (SpawnCheckTime <= 0.0f)
            {
                SpawnCheckTime = SpawnTime;

                GameObject Temp = (GameObject)Instantiate(Prefabs);
                Temp.transform.position = this.gameObject.transform.position;
            }

        }
       
    }

    // Update is called once per frame
    
}
