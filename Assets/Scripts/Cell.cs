using UnityEngine;

public class Cell
{
    public int row;     // 행
    public int col;     // 열

    private Vector3 positionOnGrid;     // 그리드 안에서의 셀의 위치 (그리드의 localposition)
    
    private Bubble bubble;              // 셀에 부착된 버블

    public Cell(int _row, int _col, Vector3 pos)
    {
        row = _row;
        col = _col;

        positionOnGrid = pos;

        bubble = null;
    }

    public bool IsEmpty()
    {
        return bubble == null;
    }
    
    public void AttachBubble(Bubble target)
    {
        bubble = target;
        bubble.cell = this;

        bubble.transform.localPosition = positionOnGrid;
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
        return positionOnGrid;
    }
}
