using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPage : App.Page {
    
    private MapController controller;

    public MapPage(App app, MapController map) : base(app)
    {
        controller = map;
    }

    public override void OnPageInitialize()
    {
        controller.OnInitialize();
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
