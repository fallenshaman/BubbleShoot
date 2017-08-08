using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {
    
    [Header("Setting")]
    [SerializeField]
    private float StartYPosition = 6.5f;
    [SerializeField]
    public float verticalSpeed = 2.5f;
    [SerializeField]
    public float horizontalSpeed = 1.5f;

    [Range(1f, 6f)]
    public float moveRange;

    // 이동 방향
    private const float MOVE_LEFT = -1f;
    private const float MOVE_RIGHT = 1f;

    private float direction = MOVE_RIGHT;

    private Coroutine coroutineFlying = null;

    public Action<Fly> OnFlyDestroy = null;

    private void Start()
    {
        StartFlying();
    }
    
    public void StartFlying()
    {
        Initialize();
        coroutineFlying = StartCoroutine(movement());
    }
    
    public void DestroyFly()
    {
        if (coroutineFlying != null)
            StopCoroutine(coroutineFlying);

        // GameManager 쪽에서 파리 목록 정리 및 Despawn을 처리한다.
        if (OnFlyDestroy != null)
            OnFlyDestroy(this);
    }

    IEnumerator movement()
    {
        float x = 0f;
        float elapsedTime = 0f;
        while(true)
        {
            x += Time.deltaTime * horizontalSpeed * direction;

            transform.position = new Vector3(x, 6.5f + Mathf.Sin(elapsedTime) * verticalSpeed, 0f);

            if(x < -moveRange)
            {
                direction = MOVE_RIGHT;
                UpdateLookingDirection();
            }
            if(moveRange < x)
            {
                direction = MOVE_LEFT;
                UpdateLookingDirection();
            }
    
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
    
    // 파리 초기화
    private void Initialize()
    {
        // 방향 설정
        direction = UnityEngine.Random.Range(0, 2) == 0 ? MOVE_LEFT : MOVE_RIGHT;
        
        UpdateLookingDirection();
    }

    // 파리가 바라 보는 방향을 변경한다.
    private void UpdateLookingDirection()
    {
        if (direction == 1f)
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
}
