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

        controller.Anim.SetBool("Idle", true);
	}

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) 
    {
        controller.Anim.SetBool("Idle", false);
    }
}

public class CharWalk : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Walk", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender) 
    {
        controller.Anim.SetBool("Walk", false);
    }
}

public class CharCasting : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Casting", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Anim.SetBool("Casting", false);
    }
}

public class CharAttack : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Attack", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Anim.SetBool("Attack", false);
    }
}

public class CharHurt : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Hurt", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Anim.SetBool("Hurt", false);
    }
}

public class CharDie : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Die", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Anim.SetBool("Die", false);
    }
}

public class CharVictory : CharState<CharController>
{
    private CharController controller;

    public void OperateEnter(CharController sender)
    {
        controller = sender;

        controller.Anim.SetBool("Victory", true);
    }

    public void OperateUpdate(CharController sender) { }
    public void OperateFixedUpdate(CharController sender) { }

    public void OperateExit(CharController sender)
    {
        controller.Anim.SetBool("Victory", false);
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

public interface ICharJob
{
    void Attack();
}

public class Knight : ICharJob
{
    public void Attack()
    {

    }
}

public class Thief : ICharJob
{
    public void Attack()
    {

    }
}

public class Archer : ICharJob
{
    public void Attack()
    {

    }
}

public class Priest : ICharJob
{
    public void Attack()
    {

    }
}


public class CharController : MonoBehaviour
{
    // 직업을 무기로 생각하고 스트래티지 패턴으로 구현
    // 직업 변경시 Set에 프로퍼티를 사용하여 GameManager에서 직업에 맞는 스텟으로 초기화 시켜주기
    // 플레이어는 State Machine 패턴으로 각각 구분해서 구현

    public Dictionary<CharState, CharState<CharController>> DicState { get; private set; }
    public StateMachine<CharController> Sm { get; private set; }

    public Animator Anim { get; private set; }

    private CharJob job;
    public CharJob Job 
    {
        get { return job; }
        set 
        {
            job = value;

            charJob = DicJob[job];
            status = GameManager.Instance.characterStatusList[(int)job];
        } 
    }

    public Dictionary<CharJob, ICharJob> DicJob { get; private set; }
    private ICharJob charJob;

    private CharacterStatus status;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    private void Start()
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

        ICharJob knight = new Knight();
        ICharJob thief = new Thief();
        ICharJob archer = new Archer();
        ICharJob priest = new Priest();

        DicJob = new Dictionary<CharJob, ICharJob>
        {
            { CharJob.Knight, knight},
            { CharJob.Thief, thief },
            { CharJob.Archer, archer },
            { CharJob.Priest, priest }
        };

        // 초기 직업 설정
        charJob = DicJob[CharJob.Knight];
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
}
