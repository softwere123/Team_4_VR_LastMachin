using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    float timer = 2.0f; // Timer to control the spawn rate

    public GameObject objectToSpawn; // The object to spawn 
    
    // Update is called once per frame
    void Update()
    {
        if(objectToSpawn != null ) // Check if the space key is pressed
        {
            timer -= Time.deltaTime; // Decrease timer by the time passed since last frame
            if (timer <= 0 && objectToSpawn != null) // Check if timer is up and objectToSpawn is not null
            {
                objectToSpawn.SetActive(true); // Activate the object
                timer = 2.0f; // Reset timer
            }
        }   
        
    }
}
