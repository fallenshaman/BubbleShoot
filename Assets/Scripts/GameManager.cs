using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public static GameManager Instance
    {
        get
        {
            return (GameManager)sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    public GameBoard gameBoard;

    public GameSettings settings;

    [SerializeField]
    private Camera mainCam;
    
    public Transform launcher;
    public Transform launchPivot;

    private Vector3 launcherScreenPos;      // 발사대의 화면 좌표

    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;

    private MaterialPropertyBlock mpb;

    private Pool bubblePool;

    public List<Sprite> listBubbleSprite = new List<Sprite>();

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
        launcherScreenPos = mainCam.WorldToScreenPoint(launcher.transform.position);

        bubblePool = PoolManager.Instance.GetPool("Bubble");

        gameBoard.CreateBoard(6);
    }
    
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        Vector3 touchedPosition;        // 터치된 위치값(픽셀 좌표계)

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            touchedPosition = Input.mousePosition;
            RotateLauncher(touchedPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            LaunchBubble();
        }
#elif UNITY_ANDROID || UNITY_IOS
        if(Input.touchCount != 0)
        {
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

    private void LaunchBubble()
    {
        Bubble projectile = bubblePool.Spawn().GetComponent<Bubble>();
        projectile.SetProjectilBubble();
        projectile.ChangeColor();
        projectile.transform.position = launchPivot.position;

        projectile.AddForce(launchPivot.up.normalized * settings.BubbleSpeed);
    }
    
    private void RotateLauncher(Vector3 touchedPosition)
    {
        // 발사대와 터치한 위치의 각도를 구한다.
        Vector3 LauncherToTouchedPosition = touchedPosition - launcherScreenPos;
        
        float angle = Vector3.Angle(Vector3.up, LauncherToTouchedPosition);

        angle = Mathf.Min(angle, settings.launcherAngleLimit);

        if (LauncherToTouchedPosition.x > 0)
            angle *= -1;
        
        launcher.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        DrawLine();

    }
    
    private void DrawLine()
    {
        // 발사할 방향으로 RayCast
        Ray2D ray = new Ray2D(launchPivot.position, launcher.up);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, settings.MaxGuideDistance, 1 << LayerMask.NameToLayer("GuideWall"));

        float lineLength = settings.MaxGuideDistance;
        float remainLength = 0f;


        if(hit.collider)
        {
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, launchPivot.position);
            lineRenderer.SetPosition(1, hit.point);
            
            lineLength = Vector3.Distance(launchPivot.position, hit.point);

            remainLength = settings.MaxGuideDistance - lineLength;
        }
        else
        {
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, launchPivot.position);
            lineRenderer.SetPosition(1, launchPivot.position + (launchPivot.up * settings.MaxGuideDistance));
        }
        
        mpb.SetVector("_MainTex_ST", new Vector4(lineLength, 1f, Time.realtimeSinceStartup * -settings.GuideAnimateSpeed , 0f));
        lineRenderer.SetPropertyBlock(mpb);

        if (remainLength > 0f)
        {
            Vector3 refelectVector = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            refelectVector.x *= -1f;
            refelectVector.Normalize();

            refelectVector *= remainLength;

            lineRenderer2.positionCount = 2;
            lineRenderer2.SetPosition(0, lineRenderer.GetPosition(1));
            lineRenderer2.SetPosition(1, lineRenderer.GetPosition(1) + refelectVector);

            mpb.SetVector("_MainTex_ST", new Vector4(remainLength, 1f, Time.realtimeSinceStartup * -settings.GuideAnimateSpeed, 0f));
            lineRenderer2.SetPropertyBlock(mpb);
        }

    }
}
