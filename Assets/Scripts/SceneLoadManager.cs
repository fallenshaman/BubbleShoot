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

    private static bool useFadeEffect = true;

    private Scene currentScene;

    public ThreadPriority loadingThreadPriority;

    private AsyncOperation operation;
    
    [Header("Timing Settings")]
    public float waitOnLoadEnd = 0.25f;
    public float fadeDuration = 0.25f;

    public AudioListener audioListener;

    // 입력받은 씬을 불러온다.
    public static void LoadScene(int sceneIndex, bool useFade = true)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        targetSceneIndex = sceneIndex;

        useFadeEffect = useFade;

        //// 씬이 제거되기전 호출
        //App.Instance.CurrentPage.OnPageUnload();

        // 우선 로딩 화면의 출력을 위해 로딩 씬을 불러옴
        SceneManager.LoadScene(GameConst.SCENE_LOADING);
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
        if(useFadeEffect)
        {
            // 로딩 UI를 표시한다.
            ShowLoadingUIs();

            yield return null;

            // 검은 화면 페이드 인
            FadeIn();
        }
        
        // 비동기로 새로운 씬을 불러 온다.
        Application.backgroundLoadingPriority = loadingThreadPriority;
        operation = SceneManager.LoadSceneAsync(levelNum, LoadSceneMode.Additive);
        
        while (!operation.isDone)
            yield return null;

        // 로딩 스크린에 있는 AudioListener를 비활성화
        audioListener.enabled = false;

        if(useFadeEffect)
        {
            // 로딩 완료 UI 표시
            ShowLoadCompleteUIs();

            // 로딩 종료후 잠시 대기
            yield return new WaitForSeconds(waitOnLoadEnd);
        }
        
        // 새로 열린 페이지 초기화 단계
        App.Instance.CurrentPage.OnPageLoaded();
        //App.Instance.CurrentPage.OnPageInitialize();

        if(useFadeEffect)
        {
            // 검은 화면 페이드 아웃
            FadeOut();
            // 페이드 아웃 대기
            yield return new WaitForSeconds(fadeDuration);
        }
        
        // 페이지가 표시됨
        App.Instance.CurrentPage.OnPageShow();
        
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