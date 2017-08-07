using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData : ScriptableObject
{
    public enum LevelType
    {
        KNOCK_DOWN,
        COLLECT_BEEHIVE,
        INTO_POT
    }
    
    public LevelType levelType;
    public int goal;
    public int bubbleCount;

    public List<RowCells> listRows = new List<RowCells>();
}

[Serializable]
public class RowCells       // 하나의 행 정보
{
    public CellInfo[] cells = new CellInfo[GameConst.GRID_COLUMN_COUNT];

    public RowCells()
    {
        for (int i = 0; i < GameConst.GRID_COLUMN_COUNT; ++i)
            cells[i] = new CellInfo();
    }

    public bool IsEmpty()
    {
        for(int i =0; i < GameConst.GRID_COLUMN_COUNT; ++i)
        {
            if (cells[i].type != CellInfo.CellType.EMPTY)
                return false;
        }
        return true;
    }
}


[Serializable]
public class CellInfo   // Struct로 할 경우 ValueType이므로 커스텀 에디터에서 변경시 바로 적용이 되지 않음
{
    public enum CellType
    {
        EMPTY = 0,
        NORMAL,
        HIVE,
        BEE,
    }

    public CellType type;
    public Bubble.Color color;
    public Bubble.Trap trap;

    public CellInfo()
    {
        type = CellType.EMPTY;
        color = Bubble.Color.ORANGE;
        trap = Bubble.Trap.NONE;
    }
}