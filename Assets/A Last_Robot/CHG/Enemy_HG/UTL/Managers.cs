using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { return s_instance; } }

    private static bool isInitialized = false;
    public void Awake()
    {
        Init();
    }
    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go); 
            s_instance = go.GetComponent<Managers>();
            isInitialized = true; // 초기화 완료
        }
    }
    public bool IsInitialized()
    {
        return isInitialized;
    }

    SGProjectileManager _projectileManager = new SGProjectileManager();
    SGShotManager _shotManager = new SGShotManager();
    
    public static SGShotManager ShotManager { get { return Instance?._shotManager; } }
    public static SGProjectileManager projectileManager { get { return Instance?._projectileManager; } }
   

}