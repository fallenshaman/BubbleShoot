using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePage : App.Page {

    public GameManager manager;
    
    public GamePage(int levelID) : base()
    {
        SceneLoadManager.LoadScene(GameConst.SCENE_GAME);
    }

    public override void OnPageLoaded()
    {
        manager = (GameManager)FindPageManager(GameConst.SCENE_GAME);

        if (manager != null)
            manager.OnPageLoaded();
    }

    public override void OnPageShow()
    {
        manager.OnPageShow();
    }

    public override void OnPageUnload()
    {
        manager.OnPageUnload();
    }
}
