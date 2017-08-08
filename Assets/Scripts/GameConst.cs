using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임에서 사용하는 상수들을 정의 하는 클래스
public class GameConst
{
    #region SCENE_INDEX
    public static int SCENE_APP = 0;
    public static int SCENE_TITLE = 1;
    public static int SCENE_MAP = 2;
    public static int SCENE_GAME = 3;
    public static int SCENE_LOADING = 4;
    #endregion


    #region SCORE
    public static int SCORE_BEE = 500;
    public static int SCORE_BUBBLE = 20;
#endregion
    
    #region TAG
    public static string TAG_BUBBLE = "Bubble";
    public static string TAG_WALL = "Wall";
    public static string TAG_GUIDE_WALL = "Bubble";
    public static string TAG_FLY = "Fly";
    
    public static string TAG_DESTROY_AREA = "DestroyArea";
    public static string TAG_PROJECTILE = "Projectile";
    public static string TAG_POT_HOLE = "PotHole";
    #endregion

    #region LAYER
    public static string LAYER_BUBBLE = "Bubble";
    public static string LAYER_PROJECTILE = "Projectile";
    public static string LAYER_POT_HOLE = "PotHole";
    #endregion

    #region POOL_NAME
    public static string POOL_BUBBLE = "Bubble";
    public static string POOL_FLY = "Fly";
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

    public static int GRID_VISIBILE_ROW_COUNT = 7;
    public static float GRID_ROW_HIDE_HEIGHT = 1.15f;
    public static float GRID_MIN_Y_POSITION = 16.5f;
    #endregion

    public static int BUBBLE_COLOR_RANGE = 4;
    public static float BUBBLE_COLOR_CHANGE_TIME = 0.2f;
    
}
