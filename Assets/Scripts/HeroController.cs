using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using GlobalEnums;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public ActorStates hero_state;
    public ActorStates prev_hero_state;

    public bool acceptingInput = true;

    public float move_input;

    public float RUN_SPEED = 5f;

    private Rigidbody2D rb2d;
    private BoxCollider2D col2d;
    private GameManager gm;
    private InputHandler inputHandler;
    public HeroControllerStates cState;
    private HeroAnimationController animCtrl;

    private static HeroController _instance;

    public static HeroController instance
    {
	get
	{
            if (_instance == null)
                _instance = FindObjectOfType<HeroController>();
            if(_instance && Application.isPlaying)
	    {
                DontDestroyOnLoad(_instance.gameObject);
	    }
            return _instance;
	}
    }

    private void Awake()
    {
        if(_instance == null)
	{
            _instance = this;
            DontDestroyOnLoad(this);
	}
        else if(this != _instance)
	{
            Destroy(gameObject);
            return;
	}
        SetupGameRefs();
    }

    private void SetupGameRefs()
    {
        if (cState == null)
            cState = new HeroControllerStates();
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<BoxCollider2D>();
        animCtrl = GetComponent<HeroAnimationController>();
        gm = GameManager.instance;
        inputHandler = gm.GetComponent<InputHandler>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        orig_Update();
    }

    private void orig_Update()
    {
        if (hero_state == ActorStates.no_input)
	{

	}
        else if(hero_state != ActorStates.no_input)
	{
            LookForInput();
	}
    }

    private void FixedUpdate()
    {
        if (hero_state != ActorStates.no_input)
	{
            Move(move_input);
            if(move_input > 0f && !cState.facingRight )
	    {
                FlipSprite();
	    }
            else if(move_input < 0f && cState.facingRight)
	    {
                FlipSprite();
	    }
	}
    }

    private void Move(float move_direction)
    {
        if (cState.onGround)
        {
            SetState(ActorStates.grounded);
        }
        if(acceptingInput)
	{
            rb2d.velocity = new Vector2(move_direction * RUN_SPEED, rb2d.velocity.y);
	}
    }

    public void FlipSprite()
    {
        cState.facingRight = !cState.facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void LookForInput()
    {
        if (acceptingInput)
        {
            move_input = inputHandler.inputActions.moveVector.Vector.x;
        }
    }
    /// <summary>
    /// 设置玩家的ActorState的新类型
    /// </summary>
    /// <param name="newState"></param>
    private void SetState(ActorStates newState)
    {
        if(newState == ActorStates.grounded)
	{
            if(Mathf.Abs(move_input) > Mathf.Epsilon)
	    {
                newState  = ActorStates.running;
	    }
	    else
	    {
                newState = ActorStates.idle;
            }
	}
        else if(newState == ActorStates.previous)
	{
            newState = prev_hero_state;
	}
        if(newState != hero_state)
	{
            prev_hero_state = hero_state;
            hero_state = newState;
            animCtrl.UpdateState(newState);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
	{
            cState.onGround = true;
	}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            cState.onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            cState.onGround = false;
        }
    }
}

[Serializable]
public class HeroControllerStates
{
    public bool facingRight;
    public bool onGround;

    public HeroControllerStates()
    {
        facingRight = false;
        onGround = false;
    }
}
