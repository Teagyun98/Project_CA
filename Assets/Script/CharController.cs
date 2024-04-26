using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CharState
{
    Idle,
    Walk,
    Casting,
    Attack,
    Hurt,
    Die,
    Victory,
}

public interface CharState<T>
{
    void OperateEnter(T sender);
    void OperateUpdate(T sender);
    void OperateFixedUpdate(T sender);
    void OperateExit(T sender);
}

public class CharIdle : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
	{
        controller = sender;

        controller.Animator.SetBool("Idle", true);
	}

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) 
    {
        controller.Animator.SetBool("Idle", false);
    }
}

public class CharWalk : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Walk", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) 
    {
        if (controller.target == null)
        {
            Vector2 targetPos = GameManager.Instance.FirstChar().transform.position;

            controller.ScaleInversion(controller.transform.position.x < targetPos.x ? true : false);

            controller.transform.position = Vector2.MoveTowards(controller.transform.position, targetPos, 2 * Time.fixedDeltaTime);
        }
        else
        {
            controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x ? true : false);

            controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.target.transform.position, 2 * Time.fixedDeltaTime);

            if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.status.attackDistance)
                controller.Sm.SetState(controller.DicState[CharState.Casting]);
        }
    }

    public void OperateExit(CharController sender) 
    {
        controller.Animator.SetBool("Walk", false);
    }
}

public class CharCasting : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Casting", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Casting", false);
    }
}

public class CharAttack : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Attack", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Attack", false);
    }
}

public class CharHurt : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Hurt", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Hurt", false);
    }
}

public class CharDie : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Die", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Die", false);
    }
}

public class CharVictory : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Victory", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Animator.SetBool("Victory", false);
    }
}

public class StateMachine<T>
{
    private T m_sender;

    public CharState<T> CurState { get; set; }

    public StateMachine(T sender, CharState<T> state)
    {
        m_sender = sender;
        SetState(state);
    }

    public void SetState(CharState<T> state)
    {
        if (m_sender == null)
            return;

        if (CurState == state)
            return;

        if (CurState != null)
            CurState.OperateExit(m_sender);

        CurState = state;

        if (CurState != null)
            CurState.OperateEnter(m_sender);
    }

    public void DoOperateUpdate()
    {
        if (m_sender == null)
            return;

        CurState.OperateUpdate(m_sender);
    }

    public void DoOperateFixedUpdate()
    {
        if (m_sender == null)
            return;

        CurState.OperateFixedUpdate(m_sender);
    }
}

public enum CharJob
{
    Knight,
    Thief,
    Archer,
    Priest,
}

public class CharController : MonoBehaviour
{
    // 직업을 무기로 생각하고 스트래티지 패턴으로 구현
    // 직업 변경시 Set에 프로퍼티를 사용하여 GameManager에서 직업에 맞는 스텟으로 초기화 시켜주기
    // 플레이어는 State Machine 패턴으로 각각 구분해서 구현

    public Dictionary<CharState, CharState<CharController>> DicState { get; private set; }
    public StateMachine<CharController> Sm { get; private set; }

    public CharacterStatus status;
    public Animator Animator { get; private set; }
    public MonsterController target;

    public virtual void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public virtual void Start()
	{
        CharState<CharController> idle = new CharIdle();
        CharState<CharController> walk = new CharWalk();
        CharState<CharController> casting = new CharCasting();
        CharState<CharController> attack = new CharAttack();
        CharState<CharController> hurt = new CharHurt();
        CharState<CharController> die = new CharDie();
        CharState<CharController> victory = new CharVictory();

        DicState = new Dictionary<CharState, CharState<CharController>>
		{
			{ CharState.Idle, idle },
			{ CharState.Walk, walk },
			{ CharState.Casting, casting },
			{ CharState.Attack, attack },
			{ CharState.Hurt, hurt },
			{ CharState.Die, die },
			{ CharState.Victory, victory }
		};

        Sm = new StateMachine<CharController>(this, DicState[CharState.Idle]);
    }

    public virtual void Update()
	{
        CharController firstChar = GameManager.Instance.FirstChar();

        target = GameManager.Instance.NearMonster(transform.position, firstChar == this ? 0 : status.attackDistance);

        if (((firstChar == this && target != null) || (firstChar != this && firstChar.Sm.CurState != firstChar.DicState[CharState.Idle])) && Sm.CurState == DicState[CharState.Idle])
            Sm.SetState(DicState[CharState.Walk]);

        Sm.DoOperateUpdate();
	}

    public virtual void FixedUpdate()
	{
        Sm.DoOperateFixedUpdate();
	}

    public void CastingEnd()
    {
        Sm.SetState(DicState[CharState.Attack]);
    }

    public void AttackEnd()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < status.attackDistance)
            target.GetDamage(status.attackPower);
    }

    public void ScaleInversion(bool check)
    {
        if ((check && transform.localScale.x > 0) || (!check && transform.localScale.x < 0))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
