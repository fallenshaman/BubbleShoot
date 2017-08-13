using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Bubble : MonoBehaviour
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

    public enum Type
    {
        NORMAL = 0,
        TRAP,
        PROJECTILE,
        FALLING,
        BEE,
        HIVE,
        FIRE_BALL,
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
    public Type type = Type.NORMAL;

    public Cell cell = null;

    private Coroutine coroutineColorChanging = null;

    private void Awake()
    {
        InitializeStates();

        SetNormalBubble(Color.ORANGE);
    }

    public bool IsAttached()
    {
        // 부작된 셀이 존재하는가?
        return cell != null;
    }
  
    // 버블이 파괴 되었을때
    public void OnDestroyed()
    {
        if (type == Type.HIVE)
        {
            CollectHive();

            cell.DetachBubble();
            Desapwn();
        }
        else if(type == Type.TRAP)
        {
            // 함정 처리
            if (trapType != Trap.NONE)
                ActivateTrap();

            cell.DetachBubble();
            Desapwn();
        }
        else if(type == Type.NORMAL)
        {
            cell.DetachBubble();
            Desapwn();
        }
        else if (type == Type.BEE)
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnBeeKnockdown();
            cell.DetachBubble();
            Desapwn();
        }
    }

    // 다른 버블과의 연결이 끊어짐
    public void OnDisconnected()
    {
        if (type == Type.BEE)
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnBeeKnockdown();
            cell.DetachBubble();
            Desapwn();
        }
        else if (type == Type.HIVE)
        {
            CollectHive();
            cell.DetachBubble();
            Desapwn();
        }
        else
        {
            cell.DetachBubble();
            SetFallingBubble();
        }
    }


    private void ActivateTrap()
    {
        GamePage page = (GamePage)App.Instance.CurrentPage;
        if (trapType == Trap.FLY)
        {
            page.manager.CreateFly();
        }
        else if(trapType == Trap.BEETLE)
        {
            page.manager.CreateBettle();
        }
        else if(trapType == Trap.ANT)
        {
            page.manager.CreateAnt();
        }
    }

    public void Desapwn()
    {
        SetNormalBubble();

        PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE).Desapwn(gameObject);
    }

    

    private void CollectHive()
    {
        GamePage page = (GamePage)App.Instance.CurrentPage;
        page.manager.OnHiveCollect();
    }

    public void AddForce(Vector3 force)
    {
        rigidbody.AddForce(force);
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
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            Desapwn();
        }
        else if(gameObject.CompareTag(GameConst.TAG_PROJECTILE) && collision.gameObject.CompareTag(GameConst.TAG_FLY))
        {
            Fly fly = collision.gameObject.GetComponent<Fly>();
            fly.DestroyFly();
            
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            Desapwn();
        }
        else if(collision.gameObject.CompareTag(GameConst.TAG_FIRE_BALL))
        {
            Debug.Log("Triggered with fireball : " + gameObject.name);

            OnDestroyed();
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.Score += GameConst.SCORE_BUBBLE;

            page.manager.gameGrid.FindDisconnectedBubbles();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.CompareTag(GameConst.TAG_PROJECTILE))
            return;
        
        if(collision.gameObject.CompareTag(GameConst.TAG_BUBBLE) ||
           collision.gameObject.CompareTag(GameConst.TAG_WALL))
        {
            Debug.Log("Collision with bubble " + gameObject.name);

            // 일반 버블 상태로 변경 
            SetNormalBubble();

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
}
