using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : PageManager {
    public override void OnPageLoaded()
    {
        Debug.Log("MapManager OnPageLoaded");

        Initialize();
    }

    // 초기화
    public void Initialize()
    {
        PoolManager.Instance.Initiallize();
    }
    
    public void PlayGame(int levelID)
    {
        App.Instance.ChangePage(new GamePage(levelID));
    }

}
