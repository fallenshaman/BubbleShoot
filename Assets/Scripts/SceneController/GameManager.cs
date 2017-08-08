using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameManager : MonoBehaviour
{
    public GameGrid gameGrid;
    public GameSettings settings;
    public LevelData levelData;

    [SerializeField]
    private Camera mainCam;
    
    private MaterialPropertyBlock mpb;

    private Pool bubblePool;
    private Pool flyPool;

    public List<Sprite> listBubbleSprite = new List<Sprite>();

    private GamePage page;
    
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
    
    private void Start()
    {
        page = new GamePage(App.Instance, this);

        mpb = new MaterialPropertyBlock();
        launcherScreenPos = mainCam.WorldToScreenPoint(launcher.transform.position);

        bubblePool = PoolManager.Instance.GetPool(GameConst.POOL_BUBBLE);
        flyPool = PoolManager.Instance.GetPool(GameConst.POOL_FLY);

        gameGrid.OnBubbleAttached += OnBubbleAttached;
    }
    
    void Update()
    {
        HandleInput();
    }

    // 초기화
    public void OnInitialize()
    {
        gameGrid.CreateGrid(levelData);

        gameGrid.UpdateGridPosition();
        
        SetMissionIcon(levelData.levelType);
        SetMissionGoal(levelData.goal);

        ShowMissionFailPopup(false);
        ShowMissionSuccessPopup(false);

        IsMissionEnd = false;

        Score = 0;
        MissionValue = 0;
        RemainBubbleCount = levelData.bubbleCount;

        LoadProjectile();
    }

    public void OnOpen()
    {
        //Debug.Log(gameGrid.LowestBubbleRow);
    }
    
    public void OnRelease()
    {
        bubblePool.DesapwnAll();
        flyPool.DesapwnAll();
    }

    public void OnClose()
    {
        Debug.Log("GameClose");
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
            SceneLoadManager.LoadScene(1);
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
