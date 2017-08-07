﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임에서 사용하는 상수들을 정의 하는 클래스
public class GameConst
{
    #region TAG
    public static string TAG_BUBBLE = "Bubble";
    public static string TAG_WALL = "Wall";
    public static string TAG_GUIDE_WALL = "Bubble";

    public static string TAG_DESTROY_AREA = "DestroyArea";
    public static string TAG_PROJECTILE = "Projectile";

    #endregion

    #region LAYER
    public static string LAYER_BUBBLE = "Bubble";
    public static string LAYER_PROJECTILE = "Projectile";
    

#endregion

    #region POOL_NAME
    public static string POOL_BUBBLE = "Bubble";
    #endregion

    #region GRID
    // 게임 그리드의 가로 셀 수
    public static int GRID_COLUMN_COUNT = 12;

    // 게임 그리드의 열간 간격
    public static float GRID_COLUMN_GAP = 1.3f;

    // 게임 그리드의 행간 간격
    public static float GRID_ROW_GAP = 1.15f;

    // 짝수 열의 X좌표 Offset
    public static float GRID_EVEN_ROW_X_OFFSET = 0.65f;

    public static int GRID_ADDITIONAL_ROW_COUNT = 10;

    public static int GRID_ADJACENT_CELL_COUNT = 6;

    public static int GRID_SIBILING_CHILDREN_CELL_COUNT = 4;

#endregion
}
