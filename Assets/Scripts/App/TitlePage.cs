using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePage : App.Page {
    
    public TitlePage() : base()
    {
        // 타이틀 씬을 불러 온다.
        SceneLoadManager.LoadScene(GameConst.SCENE_TITLE, false);
    }
    
    public override void OnPageLoaded()
    {
    }
}
