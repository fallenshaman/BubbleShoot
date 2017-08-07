using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePage : App.Page {

    private TitleController controller;
    public TitlePage(App app, TitleController title) : base(app)
    {
        controller = title;
    }
    
    public override void OnPageOpen()
    {
        controller.OnOpen();
    }

    protected override void OnPageClose()
    {
        controller.OnClose();
    }

}
