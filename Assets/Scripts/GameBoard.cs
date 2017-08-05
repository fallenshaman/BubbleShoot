using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    private Cell[,] board;

    private Pool bubblePool;

    private int gridHeight;

    private void Start()
    {
        bubblePool = PoolManager.Instance.GetPool("Bubble");
    }

    public void CreateBoard(int height)
    {
        gridHeight = height + 10;

        board = new Cell[gridHeight, 12];

        float posY = 0f;
        
        for (int row = 0; row < gridHeight; ++row)
        {
            posY = row * GameManager.Instance.settings.GridRowGap;

            int colCount = GameManager.Instance.settings.GridColumnCount;
            float xOffset = 0;

            // 짝수 줄
            if (row % 2 == 1)
            {
                colCount = GameManager.Instance.settings.GridColumnCount - 1;
                xOffset = GameManager.Instance.settings.EvenRowOffsetX;
            }
            
            for (int col = 0; col < colCount; ++col)
            {
                Cell cell = new Cell(row, col);

                board[row, col] = cell;
                
                if(row < height)
                {
                    GameObject goBubble = bubblePool.Spawn();
                    Bubble bubble = goBubble.GetComponent<Bubble>();

                    cell.AttachBubble(bubble);

                    bubble.ChangeColor();

                    bubble.transform.parent = this.transform;
                    bubble.transform.localPosition = new Vector3(xOffset + (col * GameManager.Instance.settings.GridColumnGap), -posY, 0f);
                }
            }
        }
    }

    public List<Cell> GetAdjacentCells(Cell cell)
    {
        return GetAdjacentCells(cell.row, cell.col);
    }

    // 입력한 위치 주변의 인접 Cell 목록을 반환한다.
    public List<Cell> GetAdjacentCells(int row, int col)
    {
        List<Cell> listAdjacentCell = new List<Cell>();

        // 입력 받은 위치의 행이 홀수일 때와 짝수일때 인접 Cell의 X 인덱스 구하는 방식이 다름
        int[,] ColOffset = new int[,] { { -1, 0, 1, 0, -1, -1 }, { 0, 1, 1, 1, 0, -1 } };
        int[] RowOffset = new int[] { -1, -1, 0, 1, 1, 0 };

        int rowMod = row % 2;       // 1 == 홀수행 , 0 = 짝수행

        Debug.Log(string.Format("Check [{0},{1}]", row, col));

        // 인접한 셀 6개를 검사한다.
        for (int i =0; i < 6; ++i)
        {
            int adjRow = row + RowOffset[i];
            int adjCol = col + ColOffset[rowMod, i];
            
            Debug.Log(string.Format("   Checking [{0},{1}]", adjRow, adjCol));

            if (0 <= adjCol && adjCol < GameManager.Instance.settings.GridColumnCount && 0 <= adjRow && adjRow < gridHeight)
            {
                if(board[adjRow, adjCol] != null)
                {
                    listAdjacentCell.Add(board[adjRow, adjCol]);
                }
            }
        }
        
        return listAdjacentCell;
    }
}

public class Cell
{
    public int row;
    public int col;

    private Bubble attachedBubble;

    public Cell(int _row, int _col)
    {
        row = _row;
        col = _col;

        attachedBubble = null;
    }

    public bool IsEmpty()
    {
        return attachedBubble == null;
    }


    public void AttachBubble(Bubble bubble)
    {
        attachedBubble = bubble;
        bubble.attachedCell = this;

        bubble.transform.localPosition = GetPositionOnGrid();
    }

    public Vector3 GetPositionOnGrid()
    {
        float x = col * GameManager.Instance.settings.GridColumnGap;
        float y = row * GameManager.Instance.settings.GridRowGap; 

        if (row % 2 == 1)
            x += GameManager.Instance.settings.EvenRowOffsetX;

        return new Vector3(x, -y, 0f);
    }
}
