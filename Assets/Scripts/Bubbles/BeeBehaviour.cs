using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeBehaviour : BubbleBehaviour
{
    public BeeBehaviour(Bubble _bubble) : base(_bubble) { }

    protected override void Initialize()
    {
        bubble.gameObject.tag = GameConst.TAG_BUBBLE;
        bubble.gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);
    }

    public override void OnDestroyed()
    {
        Knockdown();

        base.OnDestroyed();
    }

    public override void OnDisconnected()
    {
        Knockdown();

        bubble.DetachBubbleFromCell();
        bubble.Desapwn();       // 풀로 돌려 보낸다.
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConst.TAG_FIRE_BALL))
        {
            OnDestroyed();

            GamePage page = (GamePage)App.Instance.CurrentPage;
            page.manager.gameGrid.FindDisconnectedBubbles();
        }
    }

    private void Knockdown()
    {
        GamePage page = (GamePage)App.Instance.CurrentPage;
        page.manager.OnBeeKnockdown();
    }
}
