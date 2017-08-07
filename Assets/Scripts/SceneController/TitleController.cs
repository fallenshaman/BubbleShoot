using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

    [Header("SceneLoad")]
    public int MapSceneIndex;

    private TitlePage page;
    
	// Use this for initialization
	void Start () {
        page = new TitlePage(App.Instance, this);  
	}

    public void OnOpen()
    {
        Debug.Log("Title Open");
    }

    public void OnClose()
    {
        Debug.Log("Title Close");
    }

    public void LoadMapScene()
    {
        if (MapSceneIndex < 0 || SceneManager.sceneCountInBuildSettings <= MapSceneIndex)
        {
            Debug.LogError("Scene index is out of range! " + MapSceneIndex);
            return;
        }

        SceneLoadManager.LoadScene(MapSceneIndex);
    }

}
