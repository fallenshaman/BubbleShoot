using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class foo : MonoBehaviour {

    public Image img;

	public void fadeIn()
    {
        img.CrossFadeAlpha(0f, 0f, true);
        img.CrossFadeAlpha(1f, 10f, true);
    }

    public void fadeOut()
    {
        img.CrossFadeAlpha(0f, 10f, true);
    }
}
