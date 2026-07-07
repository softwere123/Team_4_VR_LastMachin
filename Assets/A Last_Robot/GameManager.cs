using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// restart 기능을 만들자
// 재시작 할 때 image restart를 보이지 않게 하자
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public void OnClickRestart()
    {
        //첫 장면을 가져오게 된다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public GameObject Restart;
    public GameObject Restart2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


}