using UnityEngine;
using System.Collections;//버튼의 상호작용을 위해 가져온다
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro.EditorUtilities; // 주석 19에 있는 함수를 쓰기 위해 가져온다

public class GameMenuManager : MonoBehaviour
{
    public GameObject menu; //게임 오브젝트를 메뉴로 선언
    public InputActionProperty showButton; // 버튼을 누르면 메뉴가 나오게 하기 위해 선언
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame()) // 선언한것을 검사하는 if문
        {
            menu.SetActive(!menu.activeSelf);
             
        }
       
    }
}
