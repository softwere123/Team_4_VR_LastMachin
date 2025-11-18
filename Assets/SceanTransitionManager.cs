using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceanTransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        fadeScreen.Fadeout(); 
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //fade 작업 종료후 새로운 씬 로드
        SceneManager.LoadScene(sceneIndex);



    }
}
