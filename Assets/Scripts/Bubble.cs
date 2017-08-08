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
        MAX_COUNT,
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
    
    public void SetProjectilBubble()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        gameObject.tag = GameConst.TAG_PROJECTILE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_PROJECTILE);
    }

    public void SetBubble()
    {
        rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.gravityScale = 0f;
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;

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
            page.Manager.CreateFly();
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
            page.Manager.OnBeeKnockdown();
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

    public void SetRandomColor()
    {
        int colorIndex = Random.Range(0, (int)Color.MAX_COUNT - 1);
        SetColor(colorIndex);
    }

    public void SetColor(Bubble.Color color)
    {
        SetColor((int)color);
    }

    public void SetColor(int colorIndex)
    {
        if (colorIndex < 0 || (int)Color.MAX_COUNT <= colorIndex)
            colorIndex = 0;

        colorType = (Bubble.Color)colorIndex;

        GamePage page = (GamePage)App.Instance.CurrentPage;
        spriteRenderer.sprite = page.Manager.listBubbleSprite[colorIndex];
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
            page.Manager.OnProjectileDestroyed();

            PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE).Desapwn(this.gameObject);
        }
        else if(collision.gameObject.CompareTag(GameConst.TAG_FLY))
        {
            Fly fly = collision.gameObject.GetComponent<Fly>();
            fly.DestroyFly();

            SetBubble();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.Manager.OnProjectileDestroyed();

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

            transform.parent = page.Manager.gameGrid.transform;

            // 다른 버블과 충돌
            if (collision.gameObject.CompareTag(GameConst.TAG_BUBBLE))
            {
                Bubble hitBubble = collision.gameObject.GetComponent<Bubble>();
                page.Manager.gameGrid.AddBubbleToNearCell(hitBubble.cell, this);
            }
            else if (collision.gameObject.CompareTag(GameConst.TAG_WALL))
            {
                page.Manager.gameGrid.AddBubbleToNearCell(this.transform.localPosition, this);
            }

            // 버블이 존재하는 가장 낮은 행의 값을 갱신
            page.Manager.gameGrid.UpdateLowestBubbleRow(this);

            page.Manager.gameGrid.DestroySameColorBubbles(this);
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
