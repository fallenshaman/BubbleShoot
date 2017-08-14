using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : BubbleBehaviour
{
    public ProjectileBehaviour(Bubble _bubble) : base(_bubble) { }

    private Collider2D collider;

    protected override void Initialize()
    {
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
    }

    public override void OnDisconnected()
    {
        base.OnDisconnected();
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConst.TAG_WALL))
        {
            Vector2 vel = bubble.rigid2D.velocity;
            vel.x *= -1f;

            bubble.rigid2D.velocity = vel;
        }
        else if (collision.gameObject.CompareTag(GameConst.TAG_DESTROY_AREA))
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            bubble.Desapwn();
        }
        else if(collision.gameObject.CompareTag(GameConst.TAG_FLY))
        {
            Fly fly = collision.gameObject.GetComponent<Fly>();
            fly.DestroyFly();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.OnProjectileDestroyed();

            bubble.Desapwn();
        }
    }

    public override void OnCollisionEnter(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConst.TAG_BUBBLE) ||
           collision.gameObject.CompareTag(GameConst.TAG_WALL))
        {
            GamePage page = (GamePage)App.Instance.CurrentPage;
            
            // 일반 버블 상태로 변경 
            bubble.SetNormalBubble();

            bubble.transform.parent = page.manager.gameGrid.transform;

            // 다른 버블과 충돌
            if (collision.gameObject.CompareTag(GameConst.TAG_BUBBLE))
            {
                Bubble hitBubble = collision.gameObject.GetComponent<Bubble>();
                page.manager.gameGrid.AddBubbleToNearCell(hitBubble.cell, bubble);
            }
            else if (collision.gameObject.CompareTag(GameConst.TAG_WALL))
            {
                page.manager.gameGrid.AddBubbleToNearCell(bubble.transform.localPosition, bubble);
            }
        }
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
