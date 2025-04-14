using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyeMove : MonoBehaviour
{
    




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position -= new Vector3(0.0f, 0.0f, 5.0f * Time.deltaTime);

        if (transform.position.z < -50.0f)
        {
            Destroy(this.gameObject);
        }
           
    }
}
