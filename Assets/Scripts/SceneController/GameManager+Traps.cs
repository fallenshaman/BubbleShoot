using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager  {

    [Header("Trap Bettle")]
    public GameObject goBettle;
    private bool ActivateBettle;
    private int bettleLifeTime;

    [Header("Trap Ant")]
    public GameObject goLeftPot;
    public GameObject goRightPot;
    private bool ActivateAnt;
    private int antLifeTime;
    
    public void InitializeTrap()
    {
        goBettle.SetActive(false);

        goLeftPot.SetActive(true);
        goRightPot.SetActive(true);
    }

    public void CreateFly()
    {
        Fly fly = flyPool.Spawn().GetComponent<Fly>();

        fly.OnFlyDestroy = OnFlyDestroy;

        fly.StartFlying();

        ShowFlapperButton(true);
    }

    private void OnFlyDestroy(Fly fly)
    {
        fly.OnFlyDestroy = null;
        
        // Despawn
        flyPool.Desapwn(fly.gameObject);

        if(flyPool.SpawnedItems.Count == 0)
            ShowFlapperButton(false);
    }
    
    public void CreateBettle()
    {
        ActivateBettle = true;
        bettleLifeTime = GameConst.TRAP_BETTLE_LIFE;
        goBettle.SetActive(true);
    }

    public void DestryBettle()
    {
        ActivateBettle = false;
        goBettle.SetActive(false);
    }

    public void CreateAnt()
    {
        ActivateAnt = true;
        antLifeTime = GameConst.TRAP_ANT_LIFE;
        goLeftPot.SetActive(false);
        goRightPot.SetActive(false);
    }

    public void DestroyAnt()
    {
        ActivateAnt = false;
        goLeftPot.SetActive(true);
        goRightPot.SetActive(true);
    }
}
