using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager {

    private bool ActivateRainbow = false;
    private bool ActivateUnlimitGuideLine = false;
    private bool _activateHammer;
    private bool ActivateHammer
    {
        get
        {
            return _activateHammer;
        }
        set
        {
            _activateHammer = value;
            ShowHammerNotice(_activateHammer);
        }
    }
    private bool ActivateFireball = false;
    public Transform fireball;
    
    private int rainbowCount;
    public int RainbowCount
    {
        get
        {
            return rainbowCount;
        }
        set
        {
            rainbowCount = value;
            txtRainbowCount.text = rainbowCount.ToString();
        }
    }

    private int guideLineCount;
    public int GuideLineCount
    {
        get
        {
            return guideLineCount;
        }
        set
        {
            guideLineCount = value;
            txtGuideLineCount.text = guideLineCount.ToString();
        }
    }

    private int hammerCount;
    public int HammerCount
    {
        get
        {
            return hammerCount;
        }
        set
        {
            hammerCount = value;
            txtHammerCount.text = hammerCount.ToString();
        }
    }

    private int flapperCount;
    public int FlapperCount
    {
        get
        {
            return flapperCount;
        }
        set
        {
            flapperCount = value;
            txtFlapperCount.text = flapperCount.ToString();
        }
    }

    private int fireballCount;
    public int FireballCount
    {
        get
        {
            return fireballCount;
        }
        set
        {
            fireballCount = value;
            txtFireballCount.text = fireballCount.ToString();
        }
    }

    private void InitializeItem()
    {
        RainbowCount = 5;
        GuideLineCount = 5;
        HammerCount = 5;
        FlapperCount = 5;
        FireballCount = 5;

        ActivateRainbow = false;
        ActivateUnlimitGuideLine = false;
        ActivateHammer = false;
        ActivateFireball = false;
    }

    public void OnUseRainbow()
    {
        if (RainbowCount <= 0)
            return;

        if (ActivateRainbow)
            return;

        if (projectile == null)
            return;

        // 무지개 아이템 사용 활성화
        ActivateRainbow = true;

        // 발사 준비중인 버블의 색을 무지개로 바꿈
        projectile.SetColor(Bubble.Color.RAINBOW);

        // 아이템 수 차감
        RainbowCount--;
    }

    public void OnUseGuideLine()
    {
        if (GuideLineCount <= 0)
            return;

        if (ActivateUnlimitGuideLine)
            return;

        // 가이드라인 아이템 사용 활성화
        ActivateUnlimitGuideLine = true;

        GuideLineCount--;
    }

    public void OnUseHammer()
    {
        if (HammerCount <= 0)
            return;

        if (ActivateHammer)
            return;

        ActivateHammer = true;
        
        HammerCount--;
    }

    public void OnUseFallper()
    {
        if (FlapperCount <= 0)
            return;

        List<GameObject> listFlies = flyPool.SpawnedItems;

        for(int i=listFlies.Count -1; i >=0; --i)
        {
            Fly fly = listFlies[i].GetComponent<Fly>();
            fly.DestroyFly();
        }
        
        FlapperCount--;
    }

    public void OnUseFireBall()
    {
        if (FireballCount <= 0)
            return;
        
        if (ActivateFireball)
            return;

        if (projectile == null)
            return;

        // 파이어볼 활성화
        ActivateFireball = true;

        // 발사 준비중인 버블을 파이어볼로 변경
        projectile.SetFireball();

        fireball.parent = projectile.transform;
        fireball.localPosition = new Vector3(0f, 0f, -1f);
        fireball.gameObject.SetActive(true);

        // 아이템 수 차감
        FireballCount--;
    }

}
