using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : PageManager {

    public void OnStartButtonClick()
    {
        App.Instance.ChangePage(new MapPage());
    }

}
