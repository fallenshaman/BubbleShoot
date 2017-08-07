using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour {

    public GameManager manager;
    public int score;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(GameConst.TAG_BUBBLE))
        {
            // 항아리에 들어온 버블을 제거 한다.
            Bubble bubble = collision.gameObject.GetComponent<Bubble>();
            bubble.Desapwn();

            manager.OnBubbleInThePot(score);
        }
    }
}
