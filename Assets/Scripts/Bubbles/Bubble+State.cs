using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Bubble
{
    private FSM<Bubble.Type> fsm;

    public Bubble.Type GetState()
    {
        return fsm.CurrentStateType();
    }


    private void InitializeStates()
    {
        fsm = new FSM<Type>();

        fsm.AddState(new State<Type>(Type.NORMAL, OnEnterNormal, OnExitNormal));
        fsm.AddState(new State<Type>(Type.PROJECTILE, OnEnterProjectile, OnExitProjectile));
        fsm.AddState(new State<Type>(Type.BEE, OnEnterBee, OnExitBee));
        fsm.AddState(new State<Type>(Type.HIVE, OnEnterHive, OnExitHive));
    }
    
    public void SetState(Bubble.Type type)
    {
        fsm.ChangeState(type);
    }
    
    #region NORMAL
    public void SetNormalBubble()
    {
        fsm.ChangeState(Type.NORMAL);
        SetTrap(Trap.NONE);
    }

    public void SetNormalBubble(Bubble.Color color, Bubble.Trap _trap = Trap.NONE)
    {
        fsm.ChangeState(Type.NORMAL);
        SetColor(color);
        SetTrap(_trap);
    }

    private void OnEnterNormal()
    {
        //type = Type.NORMAL;

        gameObject.tag = GameConst.TAG_BUBBLE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);
        
        // 일반 버블의 행동 정의
        behaviour = new NormalBehaviour(this);
    }

    private void OnExitNormal()
    {
        SetTrap(Trap.NONE);
    }
    
    #endregion

    #region PROJECTILE
    public void SetProjectilBubble()
    {
        fsm.ChangeState(Type.PROJECTILE);
        SetRandomColor();
    }

    private void OnEnterProjectile()
    {
        //type = Type.PROJECTILE;

        gameObject.tag = GameConst.TAG_PROJECTILE;
        gameObject.layer = 0;       // Default;

        rigid2D.bodyType = RigidbodyType2D.Dynamic;
        
        behaviour = new ProjectileBehaviour(this);
    }

    private void OnExitProjectile()
    {
        rigid2D.velocity = Vector2.zero;
        rigid2D.angularVelocity = 0f;

        rigid2D.bodyType = RigidbodyType2D.Static;
    }
    #endregion

    #region BEE
    public void SetBeeBubble()
    {
        fsm.ChangeState(Type.BEE);
    }

    private void OnEnterBee()
    {
        //type = Type.BEE;
        SetColor(Color.PURPLE);
        SetSubImage(App.Instance.setting.bee);

        behaviour = new BeeBehaviour(this);
    }

    private void OnExitBee()
    {
        SetSubImage();
    }

    #endregion

    #region HIVE
    public void SetHiveBubble(Color color)
    {
        fsm.ChangeState(Type.HIVE);

        SetColor(color);
        SetSubImage(App.Instance.setting.hive);
    }

    private void OnEnterHive()
    {
        //type = Type.HIVE;
        behaviour = new HiveBehaviour(this);
    }

    private void OnExitHive()
    {
        SetSubImage();
    }
    #endregion
}
