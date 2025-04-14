using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapTranformDestroyed : MonoBehaviour
{

    public GameObject MapData;
    public float mapMoveSpeed = 10.0f;

    public float flagPosition = -20.0f;

    public float changeTimer = 0.0f;
    public float changeCheckTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += new Vector3(0.0f, 0.0f, mapMoveSpeed * Time.deltaTime);

        changeTimer += Time.deltaTime;

        if (this.gameObject.transform.position.z <= -20.0f)
        {
            this.gameObject.transform.position += new Vector3(0.0f, 0.0f, 80.0f);

            if (changeCheckTime <= changeTimer)
            {
                GameObject temp = (GameObject)Instantiate(MapData);
                transform.position = this.gameObject.transform.position;
                Destroy(this.gameObject);

            }
        }

       
    }

   

    //삭제시킨후 새로운거 맵 재생성 ??
    
    
    //플레이어 조정하는캐릭터가
    //if (오브젝트 스페이스바)

    //{+ 인스텐스 > 맞으면 > 파괴하거나 > 시간지나면 사라지게 >   <타격시}
    //
}
