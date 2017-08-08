using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour {
    
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
            Debug.LogError("No more Bubbles");
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

    private void DrawGuideLine()
    {
        // 발사할 방향으로 RayCast
        Ray2D ray = new Ray2D(launchPivot.position, launcher.up);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, settings.MaxGuideDistance, 1 << LayerMask.NameToLayer("GuideWall"));

        float lineLength = settings.MaxGuideDistance;
        float remainLength = 0f;


        lineRenderer.gameObject.SetActive(true);
        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, launchPivot.position);
        if (hit.collider)
        {
            lineRenderer.SetPosition(1, hit.point);

            lineLength = Vector3.Distance(launchPivot.position, hit.point);

            remainLength = settings.MaxGuideDistance - lineLength;
        }
        else
        {
            lineRenderer.SetPosition(1, launchPivot.position + (launchPivot.up * settings.MaxGuideDistance));
        }

        mpb.SetVector("_MainTex_ST", new Vector4(lineLength, 1f, Time.realtimeSinceStartup * -settings.GuideAnimateSpeed, 0f));
        lineRenderer.SetPropertyBlock(mpb);

        if (remainLength > 0f)
        {
            Vector3 refelectVector = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            refelectVector.x *= -1f;
            refelectVector.Normalize();

            refelectVector *= remainLength;

            lineRenderer2.gameObject.SetActive(true);
            lineRenderer2.positionCount = 2;
            lineRenderer2.SetPosition(0, lineRenderer.GetPosition(1));
            lineRenderer2.SetPosition(1, lineRenderer.GetPosition(1) + refelectVector);

            mpb.SetVector("_MainTex_ST", new Vector4(remainLength, 1f, Time.realtimeSinceStartup * -settings.GuideAnimateSpeed, 0f));
            lineRenderer2.SetPropertyBlock(mpb);
        }
        else
        {
            lineRenderer2.gameObject.SetActive(false);
        }

    }

    private void HideGuideLine()
    {
        lineRenderer.gameObject.SetActive(false);
        lineRenderer2.gameObject.SetActive(false);
    }

}
