using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager  {
    
    [Header("Launcher")]
    public Transform launcher;
    public Transform launchPivot;
    
    private Vector3 launcherScreenPos;      // 발사대의 화면 좌표

    public Transform ProjectilePosition;
    public Transform NextProjectilePosition;

    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;

    private Bubble projectile = null;
    private Bubble nextProjectile = null;

    private bool IsLaunchable = false;

    public void LoadProjectile()
    {
        if (IsMissionEnd)
            return;

        // 더이상 발사할 버블이 없다!
        if(RemainBubbleCount == 0)
        {
            IsLaunchable = false;

            OnMissionFailed();
            return;
        }

        if (projectile == null)
        {
            if (nextProjectile != null)
            {
                projectile = nextProjectile;
                nextProjectile = null;
            }
            else
                projectile = CreateProjectileBubble();

            projectile.transform.position = ProjectilePosition.position;

            IsLaunchable = true;
        }
            
        if(nextProjectile == null)
        {
            nextProjectile = CreateProjectileBubble();
            nextProjectile.transform.position = NextProjectilePosition.position;
        }
    }

    // 발사 버블과 다음 버블을 교체한다.
    public void SwapBubble(Bubble current, Bubble next)
    {
        // 무지개 사용중일때는 버블 교체를 막는다.
        if (activateRainbow)
            return;

        projectile = next;
        projectile.transform.position = ProjectilePosition.position;

        nextProjectile = current;
        nextProjectile.transform.position = NextProjectilePosition.position;
    }

    private Bubble CreateProjectileBubble()
    {
        Bubble bubble = bubblePool.Spawn().GetComponent<Bubble>();
        bubble.SetBubble();
        bubble.SetRandomColor();

        return bubble;
    }

    // 화면 밖으로 벗어나 발사체가 파괴됨.
    public void OnProjectileDestroyed()
    {
        OnLaunchComplete();
    }

    //그리드에 버블이 부착 되었을때 호출
    private void OnBubbleAttached()
    {
        OnLaunchComplete();
    }


    // 발사체가 부착, 또는 화면 밖으로 나가 파괴되고 호출
    private void OnLaunchComplete()
    {
        if (activateRainbow)
            activateRainbow = false;

        // 새로운 발사체를 장전
        LoadProjectile();
    }


    // 버블을 발사한다.
    private void LaunchBubble()
    {

        if (!IsLaunchable)
            return;

        projectile.transform.position = launchPivot.position;
        projectile.SetProjectilBubble();
        projectile.AddForce(launchPivot.up.normalized * settings.BubbleSpeed);

        projectile = null;
        IsLaunchable = false;

        RemainBubbleCount--;
    }

    // 발사대를 회전 시킨다
    private void RotateLauncher(Vector3 touchedPosition)
    {
        if (!IsLaunchable)
            return;

        // 발사대와 터치한 위치의 각도를 구한다.
        Vector3 LauncherToTouchedPosition = touchedPosition - launcherScreenPos;

        float angle = Vector3.Angle(Vector3.up, LauncherToTouchedPosition);

        angle = Mathf.Min(angle, settings.launcherAngleLimit);

        if (LauncherToTouchedPosition.x > 0)
            angle *= -1;

        launcher.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        DrawGuideLine();
    }
    
    private float DrawLine(LineRenderer line, Vector3 from, Vector3 to)
    {
        line.gameObject.SetActive(true);
        line.positionCount = 2;

        line.SetPosition(0, from);
        line.SetPosition(1, to);

        float lineLength = Vector3.Distance(from, to);

        mpb.SetVector("_MainTex_ST", new Vector4(lineLength, 1f, Time.realtimeSinceStartup * -settings.GuideAnimateSpeed, 0f));
        line.SetPropertyBlock(mpb);

        return lineLength;
    }

    private void DrawGuideLine()
    {
        // 발사할 방향으로 RayCast
        Ray2D ray = new Ray2D(launchPivot.position, launcher.up);
        
        // 벽과 닿는지 확인한다.
        RaycastHit2D hitGuideWall = Physics2D.Raycast(ray.origin, ray.direction, settings.MaxGuideDistance, 1 << LayerMask.NameToLayer(GameConst.LAYER_GUIDE_WALL));    
        if (hitGuideWall.collider)
        {
            // 벽 까지 거리
            float lineLength = DrawLine(lineRenderer, launchPivot.position, hitGuideWall.point);

            // 남은 거리
            float remainLength = settings.MaxGuideDistance - lineLength;
            
            if (remainLength > 0f)
            {
                Vector3 refelectVector = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
                refelectVector.x *= -1f;
                refelectVector.Normalize();

                lineRenderer2.gameObject.SetActive(true);

                // 벽에서 반사된 방향으로 RayCast
                Ray2D rayFromWall = new Ray2D(hitGuideWall.point, refelectVector);

                // 버블과 직접적으로 닿는지 확인한다.
                RaycastHit2D hitSecondBubble = Physics2D.Raycast(rayFromWall.origin, rayFromWall.direction, remainLength, 1 << LayerMask.NameToLayer(GameConst.LAYER_BUBBLE));

                if(hitSecondBubble.collider)
                {
                    DrawLine(lineRenderer2, hitGuideWall.point, hitSecondBubble.point);
                }
                else
                {
                    DrawLine(lineRenderer2, hitGuideWall.point, (Vector3)hitGuideWall.point + (refelectVector * remainLength));
                }
            }
        }
        else
        {
            // 두번째 라인은 그리지 않는다.
            lineRenderer2.gameObject.SetActive(false);

            // 버블과 직접적으로 닿는지 확인한다.
            RaycastHit2D hitBubble = Physics2D.Raycast(ray.origin, ray.direction, settings.MaxGuideDistance, 1 << LayerMask.NameToLayer(GameConst.LAYER_BUBBLE));
            if (hitBubble.collider)
            {
                DrawLine(lineRenderer, launchPivot.position, hitBubble.point);
            }
            else
            {
                // 허공
                DrawLine(lineRenderer, launchPivot.position, launchPivot.position + (launcher.up * settings.MaxGuideDistance));
            }
        }
    }

    private void HideGuideLine()
    {
        lineRenderer.gameObject.SetActive(false);
        lineRenderer2.gameObject.SetActive(false);
    }

}
