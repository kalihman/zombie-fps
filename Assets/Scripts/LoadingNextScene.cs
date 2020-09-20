using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingNextScene : MonoBehaviour
{
    // 다음 씬을 비동기 방식으로 로드하고 싶다
    // 또한 현재 씬에는 로딩 진행률을 시각적으로 표현하고 싶다

    // 진행할 씬 번호
    public int sceneNumber = 2;

    // 로딩 슬라이더 바
    public Slider loadingBar;

    // 로딩 진행 텍스트
    public Text loadingText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TransitionNextScene(sceneNumber));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TransitionNextScene(int num)
    {
        // 지정된 씬을 비동기 형식으로 로드
        AsyncOperation ao = SceneManager.LoadSceneAsync(num);
        
        // 로드되는 씬이 보이지 않게 함
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            loadingBar.value = ao.progress;
            loadingText.text = (ao.progress * 100f).ToString() + "%";

            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
