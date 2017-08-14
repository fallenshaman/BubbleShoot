using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBehaviour : BubbleBehaviour
{
    public NormalBehaviour(Bubble _bubble) : base(_bubble) { }

    protected override void Initialize()
    {
    }

    public override void OnDestroyed()
    {
        if (bubble.trapType != Bubble.Trap.NONE)
            bubble.ActivateTrap();

        GamePage page = (GamePage)App.Instance.CurrentPage;
        page.manager.Score += GameConst.SCORE_BUBBLE;

        base.OnDestroyed();
    }

    public override void OnDisconnected()
    {
        base.OnDisconnected();

        // 낙하 처리
        StartFalling();
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        // 게임 하단의 파괴 영역에 닿음
        // 항아리가 사라졌을때만 발생 한다.
        if(collision.gameObject.CompareTag(GameConst.TAG_DESTROY_AREA))
        {
            EndFalling();
            bubble.Desapwn();
        }
        else if(collision.gameObject.CompareTag(GameConst.TAG_POT))
        {
            EndFalling();

            Pot pot = collision.gameObject.GetComponent<Pot>();
            
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnBubbleInThePot(pot.score);
            
            bubble.Desapwn();
        }
        else if (collision.gameObject.CompareTag(GameConst.TAG_FIRE_BALL))
        {
            OnDestroyed();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.Score += GameConst.SCORE_BUBBLE;

            page.manager.gameGrid.FindDisconnectedBubbles();
        }
    }

    public override void OnCollisionEnter(Collision2D collision)
    {
        
    }


    private void StartFalling()
    {
        bubble.gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_FALLING_BUBBLE);

        bubble.rigid2D.bodyType = RigidbodyType2D.Dynamic;
        bubble.rigid2D.gravityScale = GameConst.BUBBLE_FALLING_GRAVITY;
    }

    private void EndFalling()
    {
        bubble.rigid2D.gravityScale = 0f;
        bubble.rigid2D.bodyType = RigidbodyType2D.Static;
    }
}
