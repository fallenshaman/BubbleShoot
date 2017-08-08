﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager {

    public Image missionIcon;
    public Text txtMissionValue;
    public Text txtMissonGoal;

    public Text txtScore;
    public Text txtRemainCount;

    public GameObject missionSuccessPopup;
    public GameObject missionFailPopup;

    [Header("Item Rainbow")]
    public Text txtRainbowCount;

    [Header("Item GuideLine")]
    public Text txtGuideLineCount;

    [Header("Item Flapper")]
    public GameObject goFlapperButton;
    public Text txtFlapperCount;

    [Header("Item Hammer")]
    public Text txtHammerCount;
    public GameObject goHammerNotice;

    [Header("Item Fireball")]
    public Text txtFireballCount;

    public void InitializeUI()
    {
        SetMissionIcon(levelData.levelType);
        SetMissionGoal(levelData.goal);

        ShowMissionFailPopup(false);
        ShowMissionSuccessPopup(false);

        ShowHammerNotice(false);
        ShowFlapperButton(false);
    }

    public void OnItemRainbowClick()
    {
        OnUseRainbow();
    }

    public void OnItemGuideLineClick()
    {
        OnUseGuideLine();
    }

    public void OnItemFlapperClick()
    {
        OnUseFallper();
    }

    public void OnItemHammerClick()
    {
        OnUseHammer();
    }

    public void OnItemFireballClick()
    {
        OnUseFireBall();
    }

    public void OnButtonOK()
    {
        App.Instance.ChangePage(new MapPage());
    }

    public void OnButtonRetry()
    {
        App.Instance.ChangePage(new GamePage(levelID));
    }

    public void ShowMissionSuccessPopup(bool bShow)
    {
        missionSuccessPopup.SetActive(bShow);
    }

    public void ShowMissionFailPopup(bool bShow)
    {
        missionFailPopup.SetActive(bShow);
    }

    public void OnSwapButtonClick()
    {
        if(CanSwapBubble)
            SwapBubble(projectile, nextProjectile);
    }

    public void SetMissionIcon(LevelData.LevelType type)
    {
        missionIcon.sprite = App.Instance.setting.missionIcons[(int)type];
    }

    public void SetMissonValue(int value)
    {
        txtMissionValue.text = value.ToString();
    }

    public void SetMissionGoal(int value)
    {
        txtMissonGoal.text = value.ToString();
    }

    public void SetScore(int value)
    {
        txtScore.text = string.Format("{0:n0}", value);
    }

    public void SetRemainCount(int value)
    {
        txtRemainCount.text = value.ToString();
    }
    
    public void ShowHammerNotice(bool bShow)
    {
        goHammerNotice.SetActive(bShow);
    }

    public void ShowFlapperButton(bool bShow)
    {
        goFlapperButton.SetActive(bShow);
    }
}
