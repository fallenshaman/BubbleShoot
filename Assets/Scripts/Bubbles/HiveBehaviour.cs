using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBehaviour : BubbleBehaviour
{
    public HiveBehaviour(Bubble _bubble) : base(_bubble) { }

    protected override void Initialize()
    {
        bubble.gameObject.tag = GameConst.TAG_BUBBLE;
        bubble.gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);
    }

    public override void OnDestroyed()
    {
        CollectHive();

        base.OnDestroyed();
    }

    public override void OnDisconnected()
    {
        CollectHive();

        base.OnDisconnected();
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
    
    private void CollectHive()
    {
        GamePage page = (GamePage)App.Instance.CurrentPage;
        page.manager.OnHiveCollect();
    }
}
