using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameManager : PageManager
{
    public GameGrid gameGrid;
    public GameSettings settings;
    public LevelData levelData;

    [SerializeField]
    private Camera mainCam;
    
    private MaterialPropertyBlock mpb;

    private Pool bubblePool;
    private Pool flyPool;

    public int levelID;

    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            SetScore(score);
        }
    }

    private int remainBubbleCount;
    public int RemainBubbleCount
    {
        get
        {
            return remainBubbleCount;
        }
        set
        {
            remainBubbleCount = value;
            SetRemainCount(remainBubbleCount);
        }
    }
    
    void Update()
    {
        HandleInput();
    }

    public override void OnPageLoaded()
    {
        // 레벨 불러오기
        levelData = App.Instance.levelTable.listLevels[levelID];

        Initialize();
    }

    public override void OnPageShow()
    {
        gameGrid.ScroolToBottomRow();
    }

    public override void OnPageUnload()
    {
        bubblePool.DesapwnAll();
        flyPool.DesapwnAll();
    }
    
    // 초기화
    public void Initialize()
    {
        mpb = new MaterialPropertyBlock();
        launcherScreenPos = mainCam.WorldToScreenPoint(launcher.transform.position);

        bubblePool = PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE);
        flyPool = PoolManager.Instance.GetPool(GameConst.POOL_FLY);

        gameGrid.OnBubbleAttached = OnBubbleAttached;
        
        gameGrid.CreateGridFromLevel(levelData);
        
        IsMissionEnd = false;

        InitializeUI();

        Score = 0;
        MissionValue = 0;
        RemainBubbleCount = levelData.bubbleCount;

        InitializeItem();
        InitializeTrap();

        LoadProjectile();
    }
    
    private void HandleInput()
    {
        if (IsMissionEnd)
            return;
        
#if UNITY_EDITOR
        InputOnEditor();
#elif UNITY_ANDROID || UNITY_IOS
        InputOnMobile();
#endif
    }

    private void InputOnEditor()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() || ActivateHammer)
                return;
            
            RotateLauncher(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (ActivateHammer)
                CheckBubbleIsTouched(Input.mousePosition);
            else
                LaunchBubble();
        }

        if (Input.GetMouseButtonUp(1))
        {
            App.Instance.ChangePage(new MapPage());
        }
    }

    private void InputOnMobile()
    {
        if (Input.touchCount > 0)
        {
            if (IsPointerOverUIObject())
                return;

            Touch touch = Input.GetTouch(0);
           
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                if(!ActivateHammer)
                    RotateLauncher(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!ActivateHammer)
                    LaunchBubble();
                else
                    CheckBubbleIsTouched(touch.position);
            }
        }
    }

    private void CheckBubbleIsTouched(Vector3 touchPosition)
    {
        Vector2 touchPosInWorldPos = Camera.main.ScreenToWorldPoint(touchPosition);

        RaycastHit2D hit = Physics2D.Raycast(touchPosInWorldPos, mainCam.transform.forward);

        if (hit.collider != null)
        {
            Bubble bubble = hit.collider.gameObject.GetComponent<Bubble>();
            
            if(bubble != null)
            {
                // 그리드에 부착된 버블을 터치함
                if(bubble.IsAttached() && bubble.GetState() == Bubble.Type.NORMAL)
                {
                    gameGrid.Hammering(bubble);
                    ActivateHammer = false;
                }
            }
        }

    }

    // 안드로이드 빌드에서 UGUI와 Input의 처리를 위해.
    bool IsPointerOverUIObject()
    {
        // Referencing this code for GraphicRaycaster 
        // https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
