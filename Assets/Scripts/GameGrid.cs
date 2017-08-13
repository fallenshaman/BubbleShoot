using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 행의 인덱스는 밑에서 위로 증가
public class GameGrid : MonoBehaviour {

    //private Cell[,] grid;
    private List<Cell[]> grid = new List<Cell[]>();

    private List<bool[]> visitedFlags = new List<bool[]>();

    [SerializeField]
    private Transform roof;

    private Pool bubblePool;

    private int totalRowCount;

    //private int rowBottomBubble;
    private int rowTopBubble;
    // 버블이 부착되어 있는 가장 낮은 행의 번호
    public int rowBottomBubble { get; private set; }

    public Action OnBubbleAttached = () => { };

    public float scrollSpeed;

    // 주변 셀 검색시 사용할 Offset 값들
    // 좌, 좌하, 우하, 우, 우상, 좌상 순서임.
    // 인접 셀 검색시에는 전체 (0 ~ 5) 사용
    // 연결되지 않은  셀 검색 시에는 (0 ~ 3) 사용
    private int[,] ColOffset = new int[,] { 
        { -1, 0, 1, 1, 1, 0 },      // 짝수 행
        { -1, -1, 0, 1, 0, -1 }     // 홀수 행
    };

    private LevelData levelData = null;

    private int[] RowOffset = new int[] { 0, -1, -1, 0, 1, 1 };

    private void Start()
    {
        bubblePool = PoolManager.Instance.GetPool("Bubble");
    }
    
    public void CreateGridFromLevel(LevelData data)
    {
        levelData = data;
        int levelRowCount = levelData.listRows.Count;
        int bubbleCount = levelData.bubbleCount;

        // 전체 그리드 높이
        totalRowCount = levelRowCount + bubbleCount;

        // 그리드에 맞게 빈 셀 생성
        for(int i =0; i < totalRowCount; ++i)
        {
            grid.Add(new Cell[GameConst.GRID_COLUMN_COUNT]);
            visitedFlags.Add(new bool[GameConst.GRID_COLUMN_COUNT]);
        }

        //grid = new Cell[totalRowCount, GameConst.GRID_COLUMN_COUNT];

        // 방문 플래그 생성
        //visitedFlags = new bool[totalRowCount, GameConst.GRID_COLUMN_COUNT];

        float posY = 0f;

        rowBottomBubble = bubbleCount;
        rowTopBubble = totalRowCount - 1;

        for (int row = 0; row < totalRowCount; ++row)
        {
            int columnCount = GameConst.GRID_COLUMN_COUNT;
            float xOffset = 0;

            posY = row * GameConst.NEW_GRID_ROW_GAP;

            if (CheckOddEvenRow(row) == 0)   // 짝수 행
            {
                columnCount = GameConst.GRID_COLUMN_COUNT - 1;
                xOffset = GameConst.GRID_EVEN_ROW_X_OFFSET;
            }

            for (int col = 0; col < columnCount; ++col)
            {
                Vector3 positionOnGrid = new Vector3(xOffset + (col * GameConst.GRID_COLUMN_GAP), posY, 0f);

                Cell cell = new Cell(row, col, positionOnGrid);
                grid[row][col] = cell;
                //grid[row, col] = cell;
                
                // 실제 레벨 데이타 시작 구간
                if (row >= bubbleCount)
                {
                    int levelRow = row - bubbleCount;

                    CellInfo info = levelData.listRows[levelRowCount - 1 - levelRow].cells[col];

                    if (info.type == CellInfo.CellType.EMPTY)
                        continue;

                    GameObject goBubble = bubblePool.Spawn();
                    Bubble bubble = goBubble.GetComponent<Bubble>();
                    
                    goBubble.name = string.Format("[{0}][{1}]", row, col);

                    cell.AttachBubble(bubble);

                    if (info.type == CellInfo.CellType.NORMAL)
                    {
                        if (info.trap == Bubble.Trap.NONE)
                            bubble.SetNormalBubble(info.color);
                        else
                            bubble.SetTrapBubble(info.color, info.trap);
                    }
                    else if (info.type == CellInfo.CellType.BEE)
                    {
                        bubble.SetBeeBubble();
                    }
                    else if (info.type == CellInfo.CellType.HIVE)
                    {
                        bubble.SetState(Bubble.Type.HIVE);
                        bubble.type = Bubble.Type.HIVE;
                        bubble.SetColor(info.color);
                        bubble.SetSubImage(App.Instance.setting.hive);
                    }

                    bubble.transform.parent = this.transform;
                    bubble.transform.localPosition = positionOnGrid;
                }
            }

            Vector3 gridPos = transform.localPosition;

            if (levelData.levelType == LevelData.LevelType.INTO_POT)
            {
                // 무한 반복
                gridPos.y = -(totalRowCount - GameConst.NEW_GRID_ROW_VISIBILE_COUNT) * GameConst.NEW_GRID_ROW_GAP;

                roof.gameObject.SetActive(false);
            }
            else
            {
                gridPos.y = -(totalRowCount - GameConst.NEW_GRID_ROW_VISIBILE_COUNT_ROOF) * GameConst.NEW_GRID_ROW_GAP;

                roof.gameObject.SetActive(true);
                roof.transform.localPosition = new Vector3(7.15f, posY + 0.5f, 0f);
            }

            transform.localPosition = gridPos;
        }
    }

    private void AdditionalLoad()
    {
        // 레벫 데이터에서 가장 밑 행에서 위로 읽어들인다.
        float posY;
        
        int prevTopRow = totalRowCount;
        totalRowCount += levelData.listRows.Count;

        int row = prevTopRow;

        for (int rowLevel = 0; rowLevel < levelData.listRows.Count; ++rowLevel)
        {
            row = prevTopRow + rowLevel;
            
            int columnCount = GameConst.GRID_COLUMN_COUNT;
            float xOffset = 0;

            posY = row * GameConst.NEW_GRID_ROW_GAP;

            if (CheckOddEvenRow(row) == 0)   // 짝수 행
            {
                columnCount = GameConst.GRID_COLUMN_COUNT - 1;
                xOffset = GameConst.GRID_EVEN_ROW_X_OFFSET;
            }

            grid.Add(new Cell[GameConst.GRID_COLUMN_COUNT]);
            visitedFlags.Add(new bool[GameConst.GRID_COLUMN_COUNT]);

            for (int col = 0; col < columnCount; ++col)
            {
                Vector3 positionOnGrid = new Vector3(xOffset + (col * GameConst.GRID_COLUMN_GAP), posY, 0f);

                Cell cell = new Cell(row, col, positionOnGrid);
                grid[row][col] = cell;
                
                CellInfo info = levelData.listRows[levelData.listRows.Count - 1 - rowLevel].cells[col];

                if (info.type == CellInfo.CellType.EMPTY)
                    continue;

                GameObject goBubble = bubblePool.Spawn();
                Bubble bubble = goBubble.GetComponent<Bubble>();

                goBubble.name = string.Format("[{0}][{1}]", row, col);

                cell.AttachBubble(bubble);

                if (info.type == CellInfo.CellType.NORMAL)
                {
                    if (info.trap == Bubble.Trap.NONE)
                        bubble.SetNormalBubble(info.color);
                    else
                        bubble.SetTrapBubble(info.color, info.trap);
                }
                else if (info.type == CellInfo.CellType.BEE)
                {
                    bubble.SetBeeBubble();
                }
                else if (info.type == CellInfo.CellType.HIVE)
                {
                    bubble.SetState(Bubble.Type.HIVE);
                    bubble.type = Bubble.Type.HIVE;
                    bubble.SetColor(info.color);
                    bubble.SetSubImage(App.Instance.setting.hive);
                }

                bubble.transform.parent = this.transform;
                bubble.transform.localPosition = positionOnGrid;
            }
        }

        rowTopBubble = row;

    }

    // 행이 홀수 행인지 짝수행인지 반환
    // 0 : 짝수
    // 1 : 홀수
    private int CheckOddEvenRow(int row)
    {
        if(row > totalRowCount)
        {
            Debug.LogError("Row out of range!!");
            return 0;
        }
        
        return (totalRowCount - row) % 2;
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
        
        int rowMod = CheckOddEvenRow(row);     // 0 : 짝수 , 1 : 홀수
        
        // 인접한 셀 6개를 검사한다.
        for (int i =0; i < GameConst.GRID_ADJACENT_CELL_COUNT; ++i)
        {
            int adjRow = row + RowOffset[i];
            int adjCol = col + ColOffset[rowMod, i];
        
            if (0 <= adjCol && adjCol < GameConst.GRID_COLUMN_COUNT && 0 <= adjRow && adjRow < totalRowCount)
            {
                if(grid[adjRow][adjCol] != null)
                {
                    listAdjacentCell.Add(grid[adjRow][adjCol]);
                }
            }
        }
        
        return listAdjacentCell;
    }
    
    // 새로운 버블을 기준으로 가장 마지막 버블이 존재하는 행 값을 갱신한다.
    public void UpdateLowestBubbleRow(Bubble newBubble)
    {
        // 새로 버블이 부착된 Cell의 행이 작으면 마지막 행값을 갱신
        rowBottomBubble = Mathf.Min(rowBottomBubble, newBubble.cell.row);

        ScrollToRow(rowBottomBubble);
    }


    // 현재 가장 낮은 버블의 위치에서 위로 검색하면서 새로운 가장 낮은 버블의 위치를 찾는다.
    public void UpdateLowestBubbleRowBottomUp()
    {
        for(int row = rowBottomBubble; row < totalRowCount; ++row)
        {
            for(int col = 0; col < GameConst.GRID_COLUMN_COUNT; ++col)
            {
                Cell cell = grid[row][col];

                if (cell != null && !cell.IsEmpty())
                {
                    rowBottomBubble = row;
                    ScrollToRow(rowBottomBubble);
                    return;
                }
            }
        }
    }

    private Coroutine coroutineMovement = null;

    public void ScroolToBottomRow()
    {
        ScrollToRow(rowBottomBubble);
    }

    public void ScrollToRow(int targetRow)
    {
        if(coroutineMovement != null)
        {
            StopCoroutine(coroutineMovement);
            coroutineMovement = null;
        }

        if (totalRowCount - rowBottomBubble < GameConst.NEW_GRID_ROW_VISIBILE_COUNT_ROOF)
            targetRow = totalRowCount - GameConst.NEW_GRID_ROW_VISIBILE_COUNT_ROOF;
        
        coroutineMovement = StartCoroutine(Scrolling(targetRow));
    }


    IEnumerator Scrolling(int row)
    {
        float targetPosY = -GameConst.NEW_GRID_ROW_GAP * row;

        float moveDirection = 1f;
        if (transform.localPosition.y > targetPosY)
            moveDirection = -1f;

        float remainLength = Mathf.Abs(transform.localPosition.y - targetPosY);

        Vector3 moveVector = new Vector3(0f, 0f, 0f);
        while (remainLength > 0f)
        {
            float distance = Time.deltaTime * scrollSpeed;

            // 이동할 거리가 남은 거리보다 크면 남은 거리 만큼 이동
            if (distance > remainLength)
                distance = remainLength;

            moveVector.y = distance * moveDirection;

            transform.localPosition += moveVector;

            remainLength -= distance;

            yield return null;
        }
    }
    
    // 버블을 목표 셀 주변의 가장 가까운 빈 셀에 추가한다.
    public void AddBubbleToNearCell(Cell targetCell, Bubble bubble)
    {
        if(targetCell == null)
        {
            Debug.LogError("AddBubbleToNearCell - target cell is null!!");
        }

        // 충돌한 셀 주변의 셀을 구한다.
        List<Cell> listAdjacentCells = GetAdjacentCells(targetCell);

        Debug.Log("listAdjacentCells Count : " + listAdjacentCells.Count);
        
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
        int row = (int)((positionOnGrid.y + (GameConst.NEW_GRID_ROW_GAP * 0.5f)) / GameConst.NEW_GRID_ROW_GAP);
        
        float offset = CheckOddEvenRow(row) == 0 ? GameConst.GRID_EVEN_ROW_X_OFFSET : 0f;

        float posX = positionOnGrid.x - offset;
        
        int col = (int)((posX + (GameConst.GRID_COLUMN_GAP * 0.5f)) / GameConst.GRID_COLUMN_GAP);

        return grid[row][col];
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
        //Array.Clear(visitedFlags, 0, totalRowCount * GameConst.GRID_COLUMN_COUNT);
        for(int i =0; i < visitedFlags.Count; ++i)
        {
            for (int j = 0; j < GameConst.GRID_COLUMN_COUNT; ++j)
                visitedFlags[i][j] = false;
        }


        while (queue.Count > 0)
        {
            Bubble currentBubble = queue.Dequeue();
            Cell currentCell = currentBubble.cell;

            if (visitedFlags[currentCell.row][currentCell.col])
                continue;

            visitedFlags[currentCell.row][currentCell.col] = true;
            
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
        
        // 3개 이상부터 파괴!!
        if(listBubblesToDestroy.Count >= 3)
        {
            DestroyBubbles(listBubblesToDestroy);
        }
    }
    

    // 리스트로 전달받은 버블들을 파괴한다.
    private void DestroyBubbles(List<Bubble> listBubbles)
    {
        GamePage page = (GamePage)App.Instance.CurrentPage;

        foreach (Bubble bubble in listBubbles)
        {
            bubble.OnDestroyed();
            page.manager.Score += GameConst.SCORE_BUBBLE;
        }

        // 가장 낮은 행부터 위로 검사하여 버블이 존재하는 낮은 행의 값 갱신
        UpdateLowestBubbleRowBottomUp();

        FindDisconnectedBubbles();
    }

    // 연결되지 않은 버블들을 찾는다.
    public void FindDisconnectedBubbles()
    {
        // 1. 가장 윗 행의 모든 버블을 추가.

        // 2. 각 버블의 인접한 형제(좌,우) 버블과, 자식버블(좌하, 우하) 을 지속적으로 탐색

        // 3. 연결되어 있는 버블들은 VisitedFlag가 true가 된다.

        // 4. 버블 풀에서 활성화된 버블을 순회하면서 VisitedFlag와 비교했을때 false인 버블들은 연결되지 않은 버블
        // << 예외 : 발사 준비, 다음 버블은 예외 처리 해야함. >>

        Queue<Bubble> queue = new Queue<Bubble>();

        // 방문 플래그 초기화
        //Array.Clear(visitedFlags, 0, totalRowCount * GameConst.GRID_COLUMN_COUNT);
        for (int i = 0; i < visitedFlags.Count; ++i)
        {
            for (int j = 0; j < GameConst.GRID_COLUMN_COUNT; ++j)
                visitedFlags[i][j] = false;
        }

        int topRow = totalRowCount - 1;
        for (int i =0; i < GameConst.GRID_COLUMN_COUNT; ++i)
        {
            // 비여 있는 셀은 건너 뜀.
            if (grid[topRow][i].IsEmpty())
                continue;

            queue.Enqueue(grid[topRow][i].GetBubble());
        }

        while(queue.Count > 0)
        {
            Bubble currentBubble = queue.Dequeue();
            
            int row = currentBubble.cell.row;
            int col = currentBubble.cell.col;

            if (visitedFlags[row][col])
                continue;

            visitedFlags[row][col] = true;

            int rowMod = CheckOddEvenRow(row);       // 0 == 짝수행 , 1 = 홀수행
            
            // 형재와 자식 셀을 검사한다.
            for (int i = 0; i < GameConst.GRID_SIBILING_CHILDREN_CELL_COUNT; ++i)
            {
                int adjRow = row + RowOffset[i];
                int adjCol = col + ColOffset[rowMod, i];

                if (0 <= adjCol && adjCol < GameConst.GRID_COLUMN_COUNT && 0 <= adjRow && adjRow < totalRowCount)
                {
                    // 셀이 존재하고, 셀에 버블이 있음.
                    if (grid[adjRow][adjCol] != null && !grid[adjRow][adjCol].IsEmpty())
                    {
                        // 방문하지 않은 셀이면 추가
                        if (!visitedFlags[adjRow][adjCol])
                            queue.Enqueue(grid[adjRow][adjCol].GetBubble());
                    }
                }
            }
        }

        List<Bubble> listDisconnectedBubbles = new List<Bubble>();

        foreach(GameObject goBubble in bubblePool.SpawnedItems)
        {
            Bubble bubble = goBubble.GetComponent<Bubble>();        // 풀을 GameObject가 아닌 Generic으로 했으면...

            // 스폰된 버블중에 cell에 할당되지 않은 버블들은 발사 대기 중인 버블
            if (!bubble.IsAttached())
                continue;

            if(!visitedFlags[bubble.cell.row][bubble.cell.col])
            {
                listDisconnectedBubbles.Add(bubble);
            }
        }
        
        foreach(Bubble bubble in listDisconnectedBubbles)
        {  
            bubble.OnDisconnected();
        }

        UpdateLowestBubbleRowBottomUp();

        if(levelData.levelType == LevelData.LevelType.INTO_POT)
        {
            // 남은 버블 행 수가 레벨의 행보다 작으면 
            if (rowTopBubble - rowBottomBubble < levelData.listRows.Count - 1)
            {
                Debug.Log(string.Format("{0} - {1} < {2}", rowTopBubble, rowBottomBubble, levelData.listRows.Count));

                AdditionalLoad();
            }
        }
    }

    // 입력된 버블을 중심으로 주변 버블을 파괴한다.
    public void Hammering(Bubble bubble)
    {
        var adjacentCells = GetAdjacentCells(bubble.cell);

        List<Bubble> listBubbles = new List<Bubble>();
        listBubbles.Add(bubble);

        foreach(Cell cell in adjacentCells)
        {
            if (cell.IsEmpty())
                continue;

            listBubbles.Add(cell.GetBubble());
        }

        // 중심과 주변의 버블들을 파괴한다.
        DestroyBubbles(listBubbles);
    }

}
