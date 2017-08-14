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
        PROJECTILE,
        BEE,
        HIVE,
    }
    
    [SerializeField]
    public Rigidbody2D rigid2D;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SpriteRenderer subImage;

    [SerializeField]
    private Animator animator;
    
    private Color colorType;

    public Color color{
        get {
            return colorType;
        }
    }

    private BubbleBehaviour behaviour;

    public Trap trapType = Trap.NONE;
    
    public Cell cell = null;
    
    private Coroutine coroutineColorChanging = null;

    private void Awake()
    {
        InitializeStates();

        SetNormalBubble();
    }

    public bool IsAttached()
    {
        // 부작된 셀이 존재하는가?
        return cell != null;
    }

    // 셀에서 버블을 분리시킨다.
    public void DetachBubbleFromCell()
    {
        cell.DetachBubble();
    }
    
    // 버블이 파괴 되었을때
    public void OnDestroyed()
    {
        behaviour.OnDestroyed();    
    }

    // 다른 버블과의 연결이 끊어짐
    public void OnDisconnected()
    {
        behaviour.OnDisconnected();
    }

    public void PlayBouncingAnim()
    {
        animator.SetTrigger(GameConst.BUBBLE_BOUNCING_ANIM_TRIGGER);
    }
    
    public void ActivateTrap()
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
    
    public void AddForce(Vector3 force)
    {
        rigid2D.AddForce(force);
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        behaviour.OnTriggerEnter(collider);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        behaviour.OnCollisionEnter(collision);
    }

    public void SetColor(Bubble.Color color)
    {
        colorType = color;

        spriteRenderer.sprite = App.Instance.setting.bubbleSprites[(int)colorType];

        if (colorType == Color.RAINBOW)
        {
            coroutineColorChanging = StartCoroutine(ChangeBubbleColors());
        }
    }

    // 오렌지, 빨강, 녹색, 파랑  4가지중 랜덤색상 설정
    public void SetRandomColor()
    {
        int colorIndex = Random.Range(0, GameConst.BUBBLE_COLOR_RANGE);
        SetColor((Bubble.Color)colorIndex);
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

    public void SetTrap(Trap trap)
    {
        trapType = trap;
        SetSubImage(App.Instance.setting.trapIcons[(int)trapType]);
    }

    public void SetSubImage(Sprite sprite = null)
    {
        subImage.sprite = sprite;
    }
}
