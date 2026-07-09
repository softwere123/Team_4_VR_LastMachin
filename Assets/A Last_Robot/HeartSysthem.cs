using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //ui용 코디
public class HeartSysthem : MonoBehaviour
{
    public Image targetImage; // 이미지 컴포넌트를 가진 게임 오브젝트 대상
    public BulletDestroyed BD;

    // 예제에서 사용할 색상 값
    public Color newColor = new Color(1.0f, 0.0f, 0.0f, 1.0f); // 빨간색

    public void ChangeImageColor()
    {

    }

    public void Start()
    {
        targetImage = this.gameObject.GetComponent<Image>();
    }

    public void Update()
    {
        if (targetImage == null || BD == null || BD.PlayerMax <= 0)
            return;

        targetImage.fillAmount = (float)BD.PlayerHP / BD.PlayerMax;
    }

}
