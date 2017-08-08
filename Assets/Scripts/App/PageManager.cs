using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PageManager : MonoBehaviour {

	public virtual void OnPageLoaded()
    {
    }

    public virtual void OnPageShow()
    {
    }

    public virtual void OnPageUnload()
    {
    }
}
