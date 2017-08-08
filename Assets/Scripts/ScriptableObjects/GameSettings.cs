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
    public float MaxYPosition = 16.5f;

    [Header("Sprites")]
    public List<Sprite> bubbleSprites;

    public Sprite bee;
    public Sprite hive;

    public List<Sprite> missionIcons;

    public List<Sprite> trapIcons;
}