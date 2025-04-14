using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanwer : MonoBehaviour
{
    public GameObject MapData; // 생성할 미니맵 데이터 게임 오브젝트
    public float mapMoveSpeed = 10.0f; // 미니맵 이동 속도

    public float changeTimer = 0.0f; // 미니맵 이동 타이머
    public float changeCheckTime = 10.0f; // 미니맵 생성 및 교체 간격


    public int myStation = 0;
    public List<float> StationTime = new List<float>();
    public List<GameObject> ProjectTileList = new List<GameObject>();

    public float StationCheckTimer = 5.0f;

    // Update is called once per frame
    public Vector3 spawnPosition = new Vector3(0.0f, 0.0f, 0.0f);

    private void Start()
    {
      

    }
    void Update()
    {

        StationCheckTimer -= Time.deltaTime;

        if(StationCheckTimer <= 0 )
        {
            StationCheckTimer = StationTime[myStation];
            GameObject temp = Instantiate(ProjectTileList[myStation], spawnPosition, Quaternion.identity);
            Destroy(temp, StationTime[myStation]);
            myStation++;
        }



        //// 매 프레임마다 미니맵을 위로 이동
        //this.gameObject.transform.position += new Vector3(0.0f, 0.0f, mapMoveSpeed * Time.deltaTime);

        //// 시간을 추적하는 타이머를 증가
        //changeTimer += Time.deltaTime;

        //// 일정 시간 간격(changeCheckTime)으로 새로운 미니맵 데이터 생성 및 현재 미니맵 파괴
        //if (changeCheckTime <= changeTimer)
        //{

        //    // 새로운 미니맵 데이터 생성
        //    GameObject temp = Instantiate(MapData, spawnPosition, Quaternion.identity);

        //    // 새로운 미니맵 데이터의 위치를 현재 미니맵과 동일하게 설정
        //    //Destroy(this.gameObject);



        //    // 타이머 초기화
        //    changeTimer = 0.0f;
        //}

    }
}
