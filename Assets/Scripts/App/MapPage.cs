using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPage : App.Page {
    
    private MapManager manager;

    public MapPage() : base()
    {
        // 맵 씬을 불러온다.
        SceneLoadManager.LoadScene(GameConst.SCENE_MAP);
    }

    public override void OnPageLoaded()
    {
        manager = (MapManager)FindPageManager(GameConst.SCENE_MAP);

        if (manager != null)
            manager.OnPageLoaded();
    }

}
