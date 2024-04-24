using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharWalk : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharCasting : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharAttack : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharHurt : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharDie : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
}

public class CharVictory : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) { }
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

public class CharController : MonoBehaviour
{
    // 직업을 무기로 생각하고 스트래티지 패턴으로 구현
    // 직업 변경시 Set에 프로퍼티를 사용하여 GameManager에서 직업에 맞는 스텟으로 초기화 시켜주기
    // 플레이어는 State Machine 패턴으로 각각 구분해서 구현

    public Dictionary<CharState, CharState<CharController>> dicState { get; private set; }
    public StateMachine<CharController> sm { get; private set; }

	private void Start()
	{
        CharState<CharController> idle = new CharIdle();
        CharState<CharController> walk = new CharWalk();
        CharState<CharController> casting = new CharCasting();
        CharState<CharController> attack = new CharAttack();
        CharState<CharController> hurt = new CharHurt();
        CharState<CharController> die = new CharDie();
        CharState<CharController> victory = new CharVictory();

		dicState = new Dictionary<CharState, CharState<CharController>>
		{
			{ CharState.Idle, idle },
			{ CharState.Walk, walk },
			{ CharState.Casting, casting },
			{ CharState.Attack, attack },
			{ CharState.Hurt, hurt },
			{ CharState.Die, die },
			{ CharState.Victory, victory }
		};

		sm = new StateMachine<CharController>(this, dicState[CharState.Idle]);
	}

	private void Update()
	{
        sm.DoOperateUpdate();
	}

	private void FixedUpdate()
	{
        sm.DoOperateFixedUpdate();
	}
}
