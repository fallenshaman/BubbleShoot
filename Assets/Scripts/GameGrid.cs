using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour {
    
    private Cell[,] grid;
    private bool[,] visitedFlags;

    private Pool bubblePool;

    private int gridRowCount;

    public Action OnBubbleAttached = () => { };

    // 버블이 부탁되어 있는 가장 낮은 행의 번호
    public int LowestBubbleRow { get; private set; }

    // 주변 셀 검색시 사용할 Offset 값들
    // 좌, 좌하, 우하, 우, 우상, 좌상 순서임.
    // 인접 셀 검색시에는 전체 (0 ~ 5) 사용
    // 연결되지 않은  셀 검색 시에는 (0 ~ 3) 사용
    private int[,] ColOffset = new int[,] { 
        { -1, -1, 0, 1, 0, -1 },    // 홀수 행
        { -1, 0, 1, 1, 1, 0 } };    // 짝수 행

    private int[] RowOffset = new int[] { 0, 1, 1, 0, -1, -1 };

    private void Start()
    {
        bubblePool = PoolManager.Instance.GetPool("Bubble");
    }

    // 레벨 데이타를 이용해 게임 그리드 생성
    public void CreateGrid(LevelData levelData)
    {
        // 전체 그리드 높이
        gridRowCount = levelData.listRows.Count + GameConst.GRID_ADDITIONAL_ROW_COUNT;

        // 레벨 데이터가 상의 마지막 행이 가장 마지막 버블이 위치하는 행이다.
        LowestBubbleRow = levelData.listRows.Count - 1; 

        grid = new Cell[gridRowCount, GameConst.GRID_COLUMN_COUNT];
        visitedFlags = new bool[gridRowCount, GameConst.GRID_COLUMN_COUNT];
        
        float posY = 0f;
        
        for (int row = 0; row < gridRowCount; ++row)
        {
            posY = row * GameConst.GRID_ROW_GAP;
            
            // 이번 행에 위치하는 셀의 수 (홀수 행 : 12, 짝수 행 11)
            int columnCount = GameConst.GRID_COLUMN_COUNT;
            float xOffset = 0;

            // 짝수 행
            if (row % 2 == 1)
            {
                columnCount = GameConst.GRID_COLUMN_COUNT - 1;
                xOffset = GameConst.GRID_EVEN_ROW_X_OFFSET;
            }
            
            for (int col = 0; col < columnCount; ++col)
            {
                Cell cell = new Cell(row, col);
                grid[row, col] = cell;

                // 레벨 데이타가 존재하는 행.
                if(row < levelData.listRows.Count)
                {
                    CellInfo info = levelData.listRows[row].cells[col];

                    if (info.type == CellInfo.CellType.EMPTY)
                        continue;

                    GameObject goBubble = bubblePool.Spawn();
                    Bubble bubble = goBubble.GetComponent<Bubble>();
                    bubble.SetBubble();

                    cell.AttachBubble(bubble);

                    if (info.type == CellInfo.CellType.NORMAL)
                    {
                        bubble.SetColor(info.color);
                        bubble.SetTrap(info.trap);
                    }
                    else if(info.type == CellInfo.CellType.BEE)
                    {
                        bubble.isBee = true;
                        bubble.SetColor(Bubble.Color.PURPLE);
                        bubble.SetSubImage(App.Instance.setting.bee);
                    }
                    
                    bubble.transform.parent = this.transform;
                    bubble.transform.localPosition = new Vector3(xOffset + (col * GameConst.GRID_COLUMN_GAP), -posY, 0f);
                }
            }   
        }   // End of row
    }

    public void CreateGrid(int height)
    {
        gridRowCount = height + GameConst.GRID_ADDITIONAL_ROW_COUNT;

        grid = new Cell[gridRowCount, GameConst.GRID_COLUMN_COUNT];
        visitedFlags = new bool[gridRowCount, GameConst.GRID_COLUMN_COUNT];

        float posY = 0f;
        
        for (int row = 0; row < gridRowCount; ++row)
        {
            posY = row * GameConst.GRID_ROW_GAP;

            int colCount = GameConst.GRID_COLUMN_COUNT;
            float xOffset = 0;

            // 짝수 줄
            if (row % 2 == 1)
            {
                colCount = GameConst.GRID_COLUMN_COUNT - 1;
                xOffset = GameConst.GRID_EVEN_ROW_X_OFFSET;
            }
            
            for (int col = 0; col < colCount; ++col)
            {
                Cell cell = new Cell(row, col);

                grid[row, col] = cell;
                
                if(row < height)
                {
                    GameObject goBubble = bubblePool.Spawn();
                    Bubble bubble = goBubble.GetComponent<Bubble>();
                    bubble.SetBubble();

                    cell.AttachBubble(bubble);

                    bubble.SetRandomColor();

                    bubble.transform.parent = this.transform;
                    bubble.transform.localPosition = new Vector3(xOffset + (col * GameConst.GRID_COLUMN_GAP), -posY, 0f);
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
        
        int rowMod = row % 2;       // 0 == 홀수행 , 1 = 짝수행

        //Debug.Log(string.Format("Check [{0},{1}]", row, col));

        // 인접한 셀 6개를 검사한다.
        for (int i =0; i < GameConst.GRID_ADJACENT_CELL_COUNT; ++i)
        {
            int adjRow = row + RowOffset[i];
            int adjCol = col + ColOffset[rowMod, i];
            
            //Debug.Log(string.Format("   Checking [{0},{1}]", adjRow, adjCol));

            if (0 <= adjCol && adjCol < GameConst.GRID_COLUMN_COUNT && 0 <= adjRow && adjRow < gridRowCount)
            {
                if(grid[adjRow, adjCol] != null)
                {
                    listAdjacentCell.Add(grid[adjRow, adjCol]);
                }
            }
        }
        
        return listAdjacentCell;
    }
    
    // 새로운 버블을 기준으로 가장 마지막 버블이 존재하는 행 값을 갱신한다.
    public void UpdateLowestBubbleRow(Bubble newBubble)
    {
        // 새로 버블이 부착된 Cell의 행이 크면 마지막 행값을 갱신
        LowestBubbleRow = Mathf.Max(LowestBubbleRow, newBubble.cell.row);

        UpdateGridPosition();
    }

    public void UpdateLowestBubbleRowBottomUp()
    {
        for(int row = LowestBubbleRow; row >= 0; --row)
        {
            for(int col = 0; col < GameConst.GRID_COLUMN_COUNT; ++col)
            {
                Cell cell = grid[row, col];

                if (cell != null && !cell.IsEmpty())
                {
                    LowestBubbleRow = row;
                    UpdateGridPosition();
                    return;
                }
            }
        }
    }

    private Coroutine coroutineMovement = null;

    // 화면에 보이는 버블의 위치를 조절한다.
    // 총 10 행 출력
    // 나무는 3행
    public void UpdateGridPosition()
    {
        // 총 버블의 행 수
        int totalRowCount = LowestBubbleRow + 1;

        Vector3 pos = transform.position;

        int hideRowCount = totalRowCount - GameConst.GRID_VISIBILE_ROW_COUNT;
        if (hideRowCount < 0)
            hideRowCount = 0;

        pos.y = GameConst.GRID_MIN_Y_POSITION + (hideRowCount * GameConst.GRID_ROW_HIDE_HEIGHT);

        if (coroutineMovement != null)
            StopCoroutine(coroutineMovement);

        coroutineMovement = StartCoroutine(GridMovement(pos));
    }
    
    IEnumerator GridMovement(Vector3 targetPosistion)
    {
        float sec = 1f;
        float elapsedTime = 0f;

        Vector3 startPos = transform.position;

        while(elapsedTime < sec)
        {
            transform.position = Vector3.Lerp(startPos, targetPosistion, elapsedTime / sec);
            
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPosistion;
    }


    // 버블을 목표 셀 주변의 가장 가까운 빈 셀에 추가한다.
    public void AddBubbleToNearCell(Cell targetCell, Bubble bubble)
    {
        // 충돌한 셀 주변의 셀을 구한다.
        List<Cell> listAdjacentCells = GetAdjacentCells(targetCell);

        float minDistance = float.MaxValue;
        Cell nearestCell = null;

        foreach (Cell cell in listAdjacentCells)
        {
            if (cell == null)
                continue;

            if (!cell.IsEmpty())
                continue;

            Vector3 cellPos = cell.GetPositionOnGrid();

            float distance = Vector3.Distance(bubble.transform.localPosition, cellPos);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCell = cell;
            }
        }

        nearestCell.AttachBubble(bubble);

        OnBubbleAttached();
    }

    // 그리드 상의 좌표에서 가장 가까운 빈 셀에 버블을 추가한다.
    public void AddBubbleToNearCell(Vector3 positionOnGrid, Bubble bubble)
    {
        // 좌표에 해당하는 셀을 가져온다.
        Cell cell = GetCell(positionOnGrid);

        // 만약 좌표위의 셀이 비어 있다면 버블을 추가
        if(cell.IsEmpty())
        {
            cell.AttachBubble(bubble);
            OnBubbleAttached();
        }
        else
        {
            AddBubbleToNearCell(cell, bubble);
        }
    }

    private Cell GetCell(Vector3 positionOnGrid)
    {
        int row = (int)((-positionOnGrid.y + (GameConst.GRID_ROW_GAP * 0.5f)) / GameConst.GRID_ROW_GAP);

        float offset = row % 2 == 1 ? GameConst.GRID_EVEN_ROW_X_OFFSET : 0f;

        float posX = positionOnGrid.x - offset;


        int col = (int)((posX + (GameConst.GRID_COLUMN_GAP * 0.5f)) / GameConst.GRID_COLUMN_GAP);

        return grid[row, col];
    }


    // 버블을 기준으로 같은 색의 버블을 파괴한다.
    public void DestroySameColorBubbles(Bubble targetBubble)
    {
        // 파괴할 버블 목록
        List<Bubble> listBubblesToDestroy = new List<Bubble>();

        //목표 버블 기준으로 인접한 버블의 색을 순차적으로 검색
        Queue<Bubble> queue = new Queue<Bubble>();
        queue.Enqueue(targetBubble);

        List<Cell> adjacentCellList = new List<Cell>();

        // 방문 플래그 초기화
        Array.Clear(visitedFlags, 0, gridRowCount * GameConst.GRID_COLUMN_COUNT);

        while (queue.Count > 0)
        {
            Bubble currentBubble = queue.Dequeue();
            Cell currentCell = currentBubble.cell;

            if (visitedFlags[currentCell.row, currentCell.col])
                continue;

            visitedFlags[currentCell.row, currentCell.col] = true;
            
            // 파괴할 버블의 목록에 현재버블 추가
            listBubblesToDestroy.Add(currentBubble);

            // 현재 버블과 인접한 셀들을 구한다.
            adjacentCellList.Clear();
            adjacentCellList = GetAdjacentCells(currentCell);

            foreach (Cell cell in adjacentCellList)
            {
                // 인접 셀이 비어 있음
                if (cell.IsEmpty())
                    continue;

                Bubble adjacentBubble = cell.GetBubble();

                // 인접 셀의 버블이 다른 색임
                if (currentBubble.color == Bubble.Color.RAINBOW ||  // 현재 버블이 무지개면 모든 인접한 버블을 추가
                    currentBubble.color == adjacentBubble.color)
                {
                    // 인접 셀의 버블이 이미 파괴 목록에 있으면 건너 뜀
                    if (listBubblesToDestroy.Contains(adjacentBubble))
                        continue;

                    queue.Enqueue(adjacentBubble);
                }
            }

        }   //End of while

        GamePage page = (GamePage) App.Instance.CurrentPage;

        // 3개 이상부터 파괴!!
        if(listBubblesToDestroy.Count >= 3)
        {
            foreach(Bubble bubble in listBubblesToDestroy)
            {
                bubble.DestroyBubble();
                page.manager.Score += GameConst.SCORE_BUBBLE;
            }

            // 가장 낮은 행부터 위로 검사하여 버블이 존재하는 낮은 행의 값 갱신
            UpdateLowestBubbleRowBottomUp();
            
            FindDisconnectedBubbles();
        }
    }

    // 연결되지 않은 버블들을 찾는다.
    private void FindDisconnectedBubbles()
    {
        // 1. 가장 윗 행의 모든 버블을 추가.

        // 2. 각 버블의 인접한 형제(좌,우) 버블과, 자식버블(좌하, 우하) 을 지속적으로 탐색

        // 3. 연결되어 있는 버블들은 VisitedFlag가 true가 된다.

        // 4. 버블 풀에서 활성화된 버블을 순회하면서 VisitedFlag와 비교했을때 false인 버블들은 연결되지 않은 버블
        // << 예외 : 발사 준비, 다음 버블은 예외 처리 해야함. >>

        Queue<Bubble> queue = new Queue<Bubble>();
        
        // 방문 플래그 초기화
        Array.Clear(visitedFlags, 0, gridRowCount * GameConst.GRID_COLUMN_COUNT);

        for (int i =0; i < GameConst.GRID_COLUMN_COUNT; ++i)
        {
            // 비여 있는 셀은 건너 뜀.
            if (grid[0, i].IsEmpty())
                continue;

            queue.Enqueue(grid[0, i].GetBubble());
        }

        while(queue.Count > 0)
        {
            Bubble currentBubble = queue.Dequeue();
            
            int row = currentBubble.cell.row;
            int col = currentBubble.cell.col;

            if (visitedFlags[row, col])
                continue;

            visitedFlags[row, col] = true;

            int rowMod = row % 2;       // 0 == 홀수행 , 1 = 짝수행
            
            // 형재와 자식 셀을 검사한다.
            for (int i = 0; i < GameConst.GRID_SIBILING_CHILDREN_CELL_COUNT; ++i)
            {
                int adjRow = row + RowOffset[i];
                int adjCol = col + ColOffset[rowMod, i];

                if (0 <= adjCol && adjCol < GameConst.GRID_COLUMN_COUNT && 0 <= adjRow && adjRow < gridRowCount)
                {
                    // 셀이 존재하고, 셀에 버블이 있음.
                    if (grid[adjRow, adjCol] != null && !grid[adjRow, adjCol].IsEmpty())
                    {
                        // 방문하지 않은 셀이면 추가
                        if (!visitedFlags[adjRow, adjCol])
                            queue.Enqueue(grid[adjRow, adjCol].GetBubble());
                    }
                }
            }
        }

        List<Bubble> listDisconnectedBubbles = new List<Bubble>();

        foreach(GameObject goBubble in bubblePool.SpawnedItems)
        {
            Bubble bubble = goBubble.GetComponent<Bubble>();        // 풀을 GameObject가 아닌 Generic으로 했으면...

            // 스폰된 버블중에 cell에 할당되지 않은 버블들은 발사 대기 중인 버블
            if (bubble.cell == null)
                continue;

            if(!visitedFlags[bubble.cell.row, bubble.cell.col])
            {
                listDisconnectedBubbles.Add(bubble);
            }
        }
        
        foreach(Bubble bubble in listDisconnectedBubbles)
        {
           
            bubble.FallingBubble();
        }

        UpdateLowestBubbleRowBottomUp();
    }

}

public class Cell
{
    public int row;
    public int col;

    private Bubble bubble;

    public Cell(int _row, int _col)
    {
        row = _row;
        col = _col;

        bubble = null;
    }

    public bool IsEmpty()
    {
        return bubble == null;
    }

    public void Clear()
    {
        bubble = null;
    }

    public void AttachBubble(Bubble target)
    {
        bubble = target;
        bubble.cell = this;

        bubble.transform.localPosition = GetPositionOnGrid();
    }

    public void DetachBubble()
    {
        if (this.bubble == null)
            return;

        bubble.cell = null;
        bubble = null;
    }

    public Bubble GetBubble()
    {
        return bubble;
    }

    public Vector3 GetPositionOnGrid()
    {
        float x = col * GameConst.GRID_COLUMN_GAP;
        float y = row * GameConst.GRID_ROW_GAP; 

        if (row % 2 == 1)
            x += GameConst.GRID_EVEN_ROW_X_OFFSET;

        return new Vector3(x, -y, 0f);
    }
}
