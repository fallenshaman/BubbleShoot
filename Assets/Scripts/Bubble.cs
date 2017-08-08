using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bubble : MonoBehaviour
{
    public enum Color
    {
        ORANGE = 0,
        RED,
        GREEN,
        BLUE,
        PURPLE,
        RAINBOW,
    }

	public enum Trap
	{
		NONE,
		FLY,
		BEETLE,
		ANT,
	}

    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SpriteRenderer subImage;

    private Color colorType;

    public Color color{
        get {
            return colorType;
        }
    }

    public Trap trapType = Trap.NONE;
    public bool isBee = false;

    public Cell cell = null;

    private Coroutine coroutineColorChanging = null;
    
    public void SetProjectilBubble()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        gameObject.tag = GameConst.TAG_PROJECTILE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_PROJECTILE);
    }

    public void SetBubble()
    {
        if(coroutineColorChanging != null)
        {
            StopCoroutine(coroutineColorChanging);
            coroutineColorChanging = null;
        }
        
        if(rigidbody.bodyType != RigidbodyType2D.Static)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
            rigidbody.gravityScale = 0f;

            rigidbody.bodyType = RigidbodyType2D.Static;
        }
        
        gameObject.tag = GameConst.TAG_BUBBLE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);

        isBee = false;
        trapType = Trap.NONE;
        SetSubImage();
    }

    public void DestroyBubble()
    {
        // 함정 처리
        if (trapType != Trap.NONE)
            ActivateTrap();
        
        cell.DetachBubble();
        Desapwn();
    }

    private void ActivateTrap()
    {
        if(trapType == Trap.FLY)
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.CreateFly();
        }
    }

    public void Desapwn()
    {
        PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE).Desapwn(gameObject);
    }

    public void FallingBubble()
    {
        cell.DetachBubble();

        if (isBee)
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnBeeKnockdown();
            Desapwn();
        }
        else
        {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = 5f;
        }
    }

    public void AddForce(Vector3 force)
    {
        rigidbody.AddForce(force);
    }
    
    public void SetTrap(Trap trap)
    {
        trapType = trap;

        if(trapType == Trap.FLY)
        {
            SetSubImage(App.Instance.setting.trapIcons[(int)trapType]);
        }
    }

    // 오렌지, 빨강, 녹색, 파랑  4가지중 랜덤색상 설정
    public void SetRandomColor()
    {
        int colorIndex = Random.Range(0, GameConst.BUBBLE_COLOR_RANGE);
        SetColor((Bubble.Color)colorIndex);
    }

    public void SetColor(Bubble.Color color)
    {
        colorType = color;

        spriteRenderer.sprite = App.Instance.setting.bubbleSprites[(int)colorType];
        
        if(colorType == Color.RAINBOW)
        {
            coroutineColorChanging = StartCoroutine(ChangeBubbleColors());
        }
    }

    IEnumerator ChangeBubbleColors()
    {
        int colorIndex = 0;

        WaitForSeconds wait = new WaitForSeconds(GameConst.BUBBLE_COLOR_CHANGE_TIME);
        while (true)
        {
            spriteRenderer.sprite = App.Instance.setting.bubbleSprites[colorIndex];
            yield return wait;

            colorIndex++;
            if (colorIndex >= GameConst.BUBBLE_COLOR_RANGE)
                colorIndex = 0;
        }
    }
    
    public void SetSubImage(Sprite sprite = null)
    {
        subImage.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConst.TAG_WALL))
        {
            Vector2 vel = rigidbody.velocity;
            vel.x *= -1f;

            rigidbody.velocity = vel;
        }
        else if(collision.gameObject.CompareTag(GameConst.TAG_DESTROY_AREA))
        {
            SetBubble();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE).Desapwn(this.gameObject);
        }
        else if(gameObject.CompareTag(GameConst.TAG_PROJECTILE) && collision.gameObject.CompareTag(GameConst.TAG_FLY))
        {
            Fly fly = collision.gameObject.GetComponent<Fly>();
            fly.DestroyFly();

            SetBubble();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE).Desapwn(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.CompareTag(GameConst.TAG_PROJECTILE))
            return;
        
        if(collision.gameObject.CompareTag(GameConst.TAG_BUBBLE) ||
           collision.gameObject.CompareTag(GameConst.TAG_WALL))
        {
            // 일반 버블 상태로 변경 
            SetBubble();

            GamePage page = (GamePage)App.Instance.CurrentPage;

            transform.parent = page.manager.gameGrid.transform;

            // 다른 버블과 충돌
            if (collision.gameObject.CompareTag(GameConst.TAG_BUBBLE))
            {
                Bubble hitBubble = collision.gameObject.GetComponent<Bubble>();
                page.manager.gameGrid.AddBubbleToNearCell(hitBubble.cell, this);
            }
            else if (collision.gameObject.CompareTag(GameConst.TAG_WALL))
            {
                page.manager.gameGrid.AddBubbleToNearCell(this.transform.localPosition, this);
            }

            // 버블이 존재하는 가장 낮은 행의 값을 갱신
            page.manager.gameGrid.UpdateLowestBubbleRow(this);

            page.manager.gameGrid.DestroySameColorBubbles(this);
        }
    }

    //private void AttachToNearEmptyCell(Cell hitCell)
    //{
    //    // 충돌한 셀 주변의 셀을 구한다.
    //    List<Cell> listAdjacentCells = GameManager.Instance.gameGrid.GetAdjacentCells(hitCell);
        
    //    float minDistance = float.MaxValue;
    //    Cell nearCell = null;

    //    foreach (Cell cell in listAdjacentCells)
    //    {
    //        if (cell == null)
    //            continue;

    //        if (!cell.IsEmpty())
    //            continue;

    //        Vector3 cellPos = cell.GetPositionOnGrid();

    //        float distance = Vector3.Distance(transform.localPosition, cellPos);

    //        Debug.Log(string.Format("[{0},{1}] {2}", cell.row, cell.col, distance));

    //        if (distance < minDistance)
    //        {
    //            minDistance = distance;
    //            nearCell = cell;
    //        }
    //    }
        
    //    nearCell.AttachBubble(this);
    //}

}
