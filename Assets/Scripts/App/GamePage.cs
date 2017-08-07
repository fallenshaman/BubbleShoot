using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePage : App.Page {
    
    public GameManager Manager  { get; private set; }


    public GamePage(App app, GameManager game) : base(app)
    {
        Manager = game;
    }

    public override void OnPageInitialize()
    {
        Manager.OnInitialize();
    }

    public override void OnPageRelease()
    {
        Manager.OnRelease();
    }

    public override void OnPageOpen()
    {
        Manager.OnOpen();
    }

    protected override void OnPageClose()
    {
        Manager.OnClose();
    }

}
