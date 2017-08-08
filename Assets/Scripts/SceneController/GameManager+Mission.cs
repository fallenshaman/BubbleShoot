using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager  {

    // true : 미션이 성공/실패 여부에 없이 끝남
    private bool IsMissionEnd;

    private int missionValue;
    public int MissionValue
    {
        get
        {
            return missionValue;
        }
        set
        {
            missionValue = value;
            SetMissonValue(missionValue);
        }
    }

    // 미션 완료!
    private void OnMissionComplete()
    {
        if (IsMissionEnd)
            return;

        IsMissionEnd = true;
        // 추가 발사를 막는다.
        IsLaunchable = false;

        ShowMissionSuccessPopup(true);
    }


    // 미션 실패!!
    private void OnMissionFailed()
    {
        if (IsMissionEnd)
            return;

        IsMissionEnd = true;
        IsLaunchable = false;

        ShowMissionFailPopup(true);
    }

    // 미션값 증가
    private void IncreseMissionValue()
    {
        MissionValue++;
        if (MissionValue == levelData.goal)
        {
            OnMissionComplete();
        }
            
    }

#region KNOCKDOWN
    public void OnBeeKnockdown()
    {
        if (levelData.levelType == LevelData.LevelType.KNOCK_DOWN)
        {
            IncreseMissionValue();
        }

        Score += GameConst.SCORE_BEE;
    }

    #endregion


    #region POT
    // 항아리에 버블이 들어갔다.
    public void OnBubbleInThePot(int score)
    {
        if (levelData.levelType == LevelData.LevelType.INTO_POT)
        {
            IncreseMissionValue();
        }

        Score += score;
    }

#endregion
}
