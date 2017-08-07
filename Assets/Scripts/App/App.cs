using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 게임의 씬 하나를 나타내는 Page 클래스
    public abstract class Page
    {
        public readonly App app;

        public virtual void OnPageInitialize() { }
        public virtual void OnPageRelease() { }
        public virtual void OnPageOpen() { }
        protected virtual void OnPageClose() { }
        
        public Page(App gameApp)
        {
            this.app = gameApp;
            
            if (app.CurrentPage != null)
                app.CurrentPage.OnPageClose();
            
            app.CurrentPage = this;
        }
    }    
}
