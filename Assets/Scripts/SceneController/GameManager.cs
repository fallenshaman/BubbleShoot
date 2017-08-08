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
        gameGrid.UpdateGridPosition();
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

        gameGrid.OnBubbleAttached += OnBubbleAttached;
        
        gameGrid.CreateGrid(levelData);
        
        SetMissionIcon(levelData.levelType);
        SetMissionGoal(levelData.goal);

        ShowMissionFailPopup(false);
        ShowMissionSuccessPopup(false);

        IsMissionEnd = false;
        
        Score = 0;
        MissionValue = 0;
        RemainBubbleCount = levelData.bubbleCount;

        InitializeItem();

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
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            RotateLauncher(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            LaunchBubble();
            HideGuideLine();
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
                RotateLauncher(Input.GetTouch(0).position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                LaunchBubble();
                HideGuideLine();
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
