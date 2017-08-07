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

        bubblePool = PoolManager.Instance.GetPool("Bubble");

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
    }

    public void OnClose()
    {
        Debug.Log("GameClose");
    }
    
    private void HandleInput()
    {
        if (IsMissionEnd)
            return;

        Vector3 touchedPosition;        // 터치된 위치값(픽셀 좌표계)

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            touchedPosition = Input.mousePosition;
            RotateLauncher(touchedPosition);
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

#elif UNITY_ANDROID || UNITY_IOS
        if(Input.touchCount != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                touchedPosition = Input.GetTouch(0).position;
                RotateLauncher(touchedPosition);
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                LaunchBubble();
            }
        }
#endif
    }
}
