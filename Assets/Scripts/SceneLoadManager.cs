using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public Text txtLoading;
    public Text txtLoadComplete;
    public Image fadeOverlay;
    
    // 로딩 타겟 씬 인덱스
    public static int targetSceneIndex = -1;

    // 로딩 화면의 씬 인덱스
    private static int loadingSceneIndex = 3;

    private Scene currentScene;

    //public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    public ThreadPriority loadingThreadPriority;

    private AsyncOperation operation;
    
    [Header("Timing Settings")]
    public float waitOnLoadEnd = 0.25f;
    public float fadeDuration = 0.25f;

    public AudioListener audioListener;

    // 입력받은 씬을 불러온다.
    public static void LoadScene(int sceneIndex)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        targetSceneIndex = sceneIndex;

        // 씬이 제거되기전 호출
        App.Instance.CurrentPage.OnPageRelease();

        // 우선 로딩 화면의 출력을 위해 로딩 씬을 불러옴
        SceneManager.LoadScene(loadingSceneIndex);
    }

    void Start()
    {
        if (targetSceneIndex < 0)
            return;

        fadeOverlay.gameObject.SetActive(true);
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneAsync(targetSceneIndex));
    }

    private IEnumerator LoadSceneAsync(int levelNum)
    {
        ShowLoadingUIs();

        yield return null;

        FadeIn();
        
        Application.backgroundLoadingPriority = loadingThreadPriority;
        operation = SceneManager.LoadSceneAsync(levelNum, LoadSceneMode.Additive);
        
        while (!operation.isDone)
            yield return null;

        // 로딩 스크린에 있는 AudioListener를 비활성화
        audioListener.enabled = false;

        ShowLoadCompleteUIs();

        // 로딩 종료후 잠시 대기
        yield return new WaitForSeconds(waitOnLoadEnd);
        
        // 새로 열인 페이지 초기화 단계
        App.Instance.CurrentPage.OnPageInitialize();

        FadeOut();

        // 페이드 아웃 대기
        yield return new WaitForSeconds(fadeDuration);

        App.Instance.CurrentPage.OnPageOpen();

        // 로딩 씬 제거
        SceneManager.UnloadSceneAsync(currentScene);
    }
    
    void FadeIn()
    {
        fadeOverlay.CrossFadeAlpha(0, 0f, true);
        fadeOverlay.CrossFadeAlpha(1, 0.1f, true);
    }

    void FadeOut()
    {
        fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
    }

    void ShowLoadingUIs()
    {
        txtLoading.gameObject.SetActive(true);
        txtLoadComplete.gameObject.SetActive(false);
    }

    void ShowLoadCompleteUIs()
    {
        txtLoading.gameObject.SetActive(false);
        txtLoadComplete.gameObject.SetActive(true);
    }

}