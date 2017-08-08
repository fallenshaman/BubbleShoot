using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameManager  {
    
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
}
