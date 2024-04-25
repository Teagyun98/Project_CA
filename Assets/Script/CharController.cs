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

        controller.charJob.animator.SetBool("Idle", true);
	}

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) 
    {
        controller.charJob.animator.SetBool("Idle", false);
    }
}

public class CharWalk : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Walk", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) 
    {
        controller.charJob.animator.SetBool("Walk", false);
    }
}

public class CharCasting : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Casting", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.charJob.animator.SetBool("Casting", false);
    }
}

public class CharAttack : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Attack", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.charJob.animator.SetBool("Attack", false);
    }
}

public class CharHurt : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Hurt", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.charJob.animator.SetBool("Hurt", false);
    }
}

public class CharDie : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Die", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.charJob.animator.SetBool("Die", false);
    }
}

public class CharVictory : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.charJob.animator.SetBool("Victory", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.charJob.animator.SetBool("Victory", false);
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
[Serializable]
public class ICharJob
{
    public string name;
    public GameObject gameObject;
    public Animator animator;

    public virtual void Attack()
    {

    }
}

public class Knight : ICharJob
{
    public override void Attack()
    {
        Debug.Log("탱커");
    }
}

public class Thief : ICharJob
{
    public override void Attack()
    {
        Debug.Log("근거리 딜러");
    }
}

public class Archer : ICharJob
{
    [SerializeField] GameObject arrow;
    public override void Attack()
    {
        Debug.Log("원거리 딜러");
    }
}

public class Priest : ICharJob
{
    public override void Attack()
    {
        Debug.Log("힐러");
    }
}


public class CharController : MonoBehaviour
{
    // 직업을 무기로 생각하고 스트래티지 패턴으로 구현
    // 직업 변경시 Set에 프로퍼티를 사용하여 GameManager에서 직업에 맞는 스텟으로 초기화 시켜주기
    // 플레이어는 State Machine 패턴으로 각각 구분해서 구현

    public Dictionary<CharState, CharState<CharController>> DicState { get; private set; }
    public StateMachine<CharController> Sm { get; private set; }

    public CharJob job;

    public List<ICharJob> jobList;
    public ICharJob charJob { get; private set; }

    private CharacterStatus status;

    private void Start()
	{
        // 초기 직업 설정
        charJob = jobList[(int)job];
        status = GameManager.Instance.characterStatusList[(int)job];
        charJob.gameObject.SetActive(true);
        charJob.Attack();

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

	private void Update()
	{
        Sm.DoOperateUpdate();
	}

	private void FixedUpdate()
	{
        Sm.DoOperateFixedUpdate();
	}

    public void CastingEnd()
    {
        Sm.SetState(DicState[CharState.Attack]);
    }

    [ContextMenu("리스트 초기화")]
    public void SetList()
    {
        ICharJob knight = new Knight();
        ICharJob thief = new Thief();
        ICharJob archer = new Archer();
        ICharJob priest = new Priest();

        jobList = new List<ICharJob>
        {
            knight,
            thief,
            archer,
            priest
        };
    }
}
