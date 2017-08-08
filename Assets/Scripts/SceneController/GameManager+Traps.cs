using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager  {

    [Header("Trap")]
    public GameObject goBettle;

    private bool ActivateBettle;
    private int bettleLifeTIme;

    public void InitializeTrap()
    {
        goBettle.SetActive(false);
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
        bettleLifeTIme = GameConst.TRAP_BETTLE_LIFE;
        goBettle.SetActive(true);
    }

    public void DestryBettle()
    {
        ActivateBettle = false;
        goBettle.SetActive(false);
    }
}
