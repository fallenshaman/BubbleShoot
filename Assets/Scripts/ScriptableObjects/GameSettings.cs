using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettings : ScriptableObject
{
    [Header("Launcher")]
    public float BubbleSpeed;
    public float MaxGuideDistance;
    public float launcherAngleLimit;
    public float GuideAnimateSpeed;

    [Header("GameBoard")]
    public int GridColumnCount = 12;
    public float GridRowGap = 1.15f;
    public float GridColumnGap = 1.3f;
    public float MaxYPosition = 16.5f;
    public float EvenRowOffsetX = 0.65f;
}