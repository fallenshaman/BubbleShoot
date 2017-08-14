using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BubbleBehaviour {
    protected Bubble bubble;

    public BubbleBehaviour(Bubble _bubble)
    {
        bubble = _bubble;

        Initialize();
    }

    // 초기화
    protected virtual void Initialize()
    {

    }

    // 버블을 파괴한다
    public virtual void OnDestroyed()
    {
        bubble.DetachBubbleFromCell();        // 셀에서 분리 시킨다.
        bubble.Desapwn();       // 풀로 돌려 보낸다.
    }
	
    public virtual void OnDisconnected()
    {
        bubble.DetachBubbleFromCell();
    }

    public virtual void OnTriggerEnter(Collider2D collision)
    {

    }

    public virtual void OnCollisionEnter(Collision2D collision)
    {

    }

}
