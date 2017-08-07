using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private static Color[] CellColors = new Color[6]
    {
        new Color(1f, 0.64f, 0f),   // Orange
        Color.red,   // Red
        Color.green,   // Green
        Color.cyan,   // Blue
        Color.magenta,   // Purple
        Color.white    // WHITE
        };
    
    private LevelData _target;
    

    public void OnEnable()
    {
        _target = (LevelData)target;
    }

    public override void OnInspectorGUI()
    {
        bool isChange = false;
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("레벨 타입 : ", GUILayout.Width(90.0f));
        LevelData.LevelType newType = (LevelData.LevelType)EditorGUILayout.EnumPopup(_target.levelType);
        if(newType != _target.levelType)
        {
            isChange = true;
            _target.levelType = newType;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("목표 수 : ", GUILayout.Width(90.0f));
        int goal = EditorGUILayout.IntField(_target.goal, GUILayout.Width(50.0f));
        if (_target.goal != goal)
        {
            isChange = true;
            _target.goal = goal;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("버블 수 : ", GUILayout.Width(90.0f));
        int bubbleCount = EditorGUILayout.IntField(_target.bubbleCount, GUILayout.Width(50.0f));
        if (_target.bubbleCount != bubbleCount)
        {
            isChange = true;
            _target.bubbleCount = bubbleCount;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("행 수 : " + _target.listRows.Count);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("행 추가"))
        {
            _target.listRows.Add(new RowCells());
            isChange = true;
        }

        if (GUILayout.Button("비어있는 행 삭제"))
        {
            for(int i= _target.listRows.Count -1; i >=0; --i)
            {
                if (_target.listRows[i].IsEmpty())
                {
                    _target.listRows.RemoveAt(i);
                    isChange = true;
                }
                else
                {
                    break;
                }
            }
        }

        GUILayout.EndHorizontal();

        

        for (int row = 0; row < _target.listRows.Count; ++row)
        {
            DrawRow(row, _target.listRows[row], ref isChange);
        }

        if (isChange)
        {
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(_target);
        }

    }

    private void DrawRow(int row, RowCells rowData, ref bool isChange)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("행 삭제"))
        {
            _target.listRows.Remove(rowData);
            return;
        }


        bool isEvenRow = row % 2 == 1;

        if(isEvenRow)
            GUILayout.Space(45f);

        for (int i = 0; i < rowData.cells.Length; ++i)
        {
            if (isEvenRow && i == GameConst.GRID_COLUMN_COUNT - 1)
            {
                GUILayout.Space(45f);
                continue;
            }
                
            CellInfo cell = rowData.cells[i];

            if(cell.type == CellInfo.CellType.NORMAL || cell.type == CellInfo.CellType.HIVE)
            {
                GUI.backgroundColor = CellColors[(int)cell.color];
            }
            else
                GUI.backgroundColor = Color.white;
            
            GUILayout.BeginVertical("Box", GUILayout.Width(80.0f), GUILayout.Height(80.0f));
            GUILayout.Label(string.Format("[{0},{1}]", row, i));
            CellInfo.CellType newType = (CellInfo.CellType)EditorGUILayout.EnumPopup(cell.type);

            if (newType != cell.type)
            {
                isChange = true;
                cell.type = newType;
            }

            if (cell.type == CellInfo.CellType.NORMAL)
            {
                DrawCellColor(cell, ref isChange);
                DrawCellTrap(cell, ref isChange);
            }
            else if (cell.type == CellInfo.CellType.HIVE)
            {
                DrawCellColor(cell, ref isChange);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawCellColor(CellInfo cell, ref bool isChange)
    {
        Bubble.Color newColor = (Bubble.Color)EditorGUILayout.EnumPopup(cell.color);
        if (newColor != cell.color)
        {
            isChange = true;
            cell.color = newColor;
        }
    }

    private void DrawCellTrap(CellInfo cell, ref bool isChange)
    {
        Bubble.Trap newTrap = (Bubble.Trap)EditorGUILayout.EnumPopup(cell.trap);
        if (newTrap != cell.trap)
        {
            isChange = true;
            cell.trap = newTrap;
        }
    }
}
