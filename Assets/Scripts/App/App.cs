using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 앱 실행 라이프 사이클 동안 유지 되는 클래스
public class App : Singleton<App>
{
    public static App Instance
    {
        get
        {
            return (App)sInstance;
        }
        set
        {
            sInstance = value;
        }
    }
    

    public Page CurrentPage { get; private set; }

    public GameSettings setting;

    public LevelTable levelTable;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1080, 1920, true);
    }

    private void Start()
    {
        App.Instance.ChangePage(new TitlePage());
    }

    public void ChangePage(Page newPage)
    {
        if(CurrentPage != null)
        {
            CurrentPage.OnPageUnload();
        }

        CurrentPage = newPage;
    }
    
    // 게임의 씬 하나를 나타내는 Page 클래스
    public abstract class Page
    {
        //public virtual void OnPageInitialize() { }
        //public virtual void OnPageRelease() { }
        //public virtual void OnPageOpen() { }
        //protected virtual void OnPageClose() { }

        protected PageManager FindPageManager(int sceneIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            
            if(scene.isLoaded)
            {
                var roots = scene.GetRootGameObjects();

                foreach (GameObject go in roots)
                {
                    if (go.GetComponent<PageManager>() != null)
                    {
                        return go.GetComponent<PageManager>();
                    }
                }
            }
            
            return null;
        }
        
        public virtual void OnPageLoaded() { }  // 페이지(씬)이 로드 된 후 호출
        
        public virtual void OnPageShow() { }    // 로딩 스크린이 사라진 후 호출됨
        
        public virtual void OnPageUnload() { }  // 페이지(씬)이 언로드 되기전에 호출

        //public Page()
        //{
        //    //Debug.Log("Page Constructor");

        //    //if (App.Instance.CurrentPage != null)
        //    //    App.Instance.CurrentPage.OnPageUnload();

        //    //Debug.LogError("Assigne new Page");
        //    //App.Instance.CurrentPage = this;
        //    //Debug.LogError("Assigne new Page Complete");
        //}
    }    
}
