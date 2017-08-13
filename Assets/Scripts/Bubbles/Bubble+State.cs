using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Bubble
{
    private FSM<Bubble.Type> fsm;

    private void InitializeStates()
    {
        fsm = new FSM<Type>();

        fsm.AddState(new State<Type>(Type.NORMAL, OnEnterNormal));
        fsm.AddState(new State<Type>(Type.TRAP, OnEnterTrap, OnExitTrap));
        fsm.AddState(new State<Type>(Type.PROJECTILE, OnEnterProjectile, OnExitProjectile));
        fsm.AddState(new State<Type>(Type.FALLING, OnEnterFalling, OnExitFalling));
        fsm.AddState(new State<Type>(Type.BEE, OnEnterBee, OnExitBee));
        fsm.AddState(new State<Type>(Type.HIVE, OnEnterHive, OnExitHive));
        fsm.AddState(new State<Type>(Type.FIRE_BALL, OnEnterFireBall, OnExitFireBall));
        
    }
    
    public void SetState(Bubble.Type type)
    {
        fsm.ChangeState(type);
    }
    
    #region NORMAL
    public void SetNormalBubble()
    {
        fsm.ChangeState(Type.NORMAL);
    }

    public void SetNormalBubble(Bubble.Color color)
    {
        fsm.ChangeState(Type.NORMAL);
        SetColor(color);
    }

    private void OnEnterNormal()
    {
        type = Type.NORMAL;

        gameObject.tag = GameConst.TAG_BUBBLE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);

        trapType = Trap.NONE;
    }
    #endregion

    #region TRAP
    public void SetTrapBubble(Bubble.Color color, Bubble.Trap trap)
    {
        fsm.ChangeState(Type.TRAP);
        
        SetColor(color);

        trapType = trap;
        SetSubImage(App.Instance.setting.trapIcons[(int)trapType]);
    }

    private void OnEnterTrap()
    {
        type = Type.TRAP;

        gameObject.tag = GameConst.TAG_BUBBLE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_BUBBLE);
    }

    private void OnExitTrap()
    {
        trapType = Trap.NONE;
        SetSubImage();
    }

    #endregion


    #region PROJECTILE
    public void SetProjectilBubble()
    {
        fsm.ChangeState(Type.PROJECTILE);
    }
    
    private void OnEnterProjectile()
    {
        type = Type.PROJECTILE;
        
        gameObject.tag = GameConst.TAG_PROJECTILE;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_PROJECTILE);

        rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnExitProjectile()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;

        rigidbody.bodyType = RigidbodyType2D.Static;
    }
    #endregion

    #region FALLING
    public void SetFallingBubble()
    {
        fsm.ChangeState(Type.FALLING);
    }

    private void OnEnterFalling()
    {
        type = Type.FALLING;

        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_FALLING_BUBBLE);

        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.gravityScale = 5f;
    }

    private void OnExitFalling()
    {
        rigidbody.gravityScale = 0f;
        rigidbody.bodyType = RigidbodyType2D.Static;
    }
    #endregion

    #region BEE
    public void SetBeeBubble()
    {
        fsm.ChangeState(Type.BEE);
    }
    
    private void OnEnterBee()
    {
        type = Type.BEE;
        SetColor(Color.PURPLE);
        SetSubImage(App.Instance.setting.bee);
    }

    private void OnExitBee()
    {
        SetSubImage();
    }

    #endregion

    #region HIVE
   public void SetHiveBubble()
    {
        fsm.ChangeState(Type.HIVE);
    }

    private void OnEnterHive()
    {
        type = Type.HIVE;
    }

    private void OnExitHive()
    {

    }


    #endregion

    #region FIRE_BALL
    
    public void SetFireBall()
    {
        fsm.ChangeState(Type.FIRE_BALL);
    }

    private void OnEnterFireBall()
    {
        type = Type.FIRE_BALL;

        gameObject.tag = GameConst.TAG_FIRE_BALL;
        gameObject.layer = LayerMask.NameToLayer(GameConst.LAYER_PROJECTILE);

        rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
    
    private void OnExitFireBall()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;

        rigidbody.bodyType = RigidbodyType2D.Static;
    }

    #endregion

}
