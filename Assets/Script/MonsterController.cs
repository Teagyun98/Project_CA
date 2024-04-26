using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;


public enum MonsterState
{
    Idle,
    Run,
    Attack,
    Death,
}

public class MonsterIdle : CharState<MonsterController>
{
    MonsterController controller;

    private float attackDelay;

    public void OperateEnter(MonsterController sender) 
    {
        controller = sender;
        controller.Animator.SetBool("Idle", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Idle", false); }
    public void OperateUpdate(MonsterController sender) 
    {
        if (controller.target == null)
            return;

        if (Vector2.Distance(controller.transform.position, controller.target.transform.position) > controller.AttackDistance)
            controller.Sm.SetState(controller.DicState[MonsterState.Run]);
        else
            controller.Sm.SetState(controller.DicState[MonsterState.Attack]);

        if (controller.target != null && Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[attackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }

    }
    public void OperateFixedUpdate(MonsterController sender) { }
}

public class MonsterRun : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Run", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Run", false); }
    public void OperateUpdate(MonsterController sender) { }
    public void OperateFixedUpdate(MonsterController sender) 
    {
        if (controller.target == null)
            return;

        controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x);

        controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.target.transform.position, 1 * Time.fixedDeltaTime);

        if (controller.target != null && Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[controller.AttackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }
    }
}

public class MonsterAttack : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Attack", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Attack", false); }
    public void OperateUpdate(MonsterController sender) { }
    public void OperateFixedUpdate(MonsterController sender) { }
}

public class MonsterDeath : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Death", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Death", false); }
    public void OperateUpdate(MonsterController sender) { }
    public void OperateFixedUpdate(MonsterController sender) { }
}

public class MonsterController : MonoBehaviour
{
    public Dictionary<MonsterState, CharState<MonsterController>> DicState { get; private set; }
    public StateMachine<MonsterController> Sm { get; private set; }

    public Animator Animator { get; private set; }

    public int MaxHp { get; private set; }
    private int hp;
    public int AttackPower { get; private set; }
    public float AttackDelay { get; private set; }
    public float AttackDistance { get; private set; }

    public CharController target;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MaxHp = GameManager.Instance.monsterHp;
        hp = MaxHp;
        AttackPower = GameManager.Instance.monsterAttackPower;
        AttackDelay = GameManager.Instance.monsterAttackDelay;
        AttackDistance = GameManager.Instance.mosterAttackDistance;

        CharState<MonsterController> idle = new MonsterIdle();
        CharState<MonsterController> run = new MonsterRun();
        CharState<MonsterController> attack = new MonsterAttack();
        CharState<MonsterController> death = new MonsterDeath();

        DicState = new Dictionary<MonsterState, CharState<MonsterController>>
        {
            { MonsterState.Idle, idle},
            { MonsterState.Run, run},
            { MonsterState.Attack, attack},
            { MonsterState.Death, death}
        };

        Sm = new StateMachine<MonsterController>(this, DicState[MonsterState.Idle]);
    }

    private void Update()
    {
        if(AttackDelay > 0)
            AttackDelay -= Time.deltaTime;

        target = GameManager.Instance.NearChar(transform.position);

        if (target == null && Sm.CurState != DicState[MonsterState.Idle] && Sm.CurState != DicState[MonsterState.Death])
            Sm.SetState(DicState[MonsterState.Idle]);

        Sm.DoOperateUpdate();
    }

    private void FixedUpdate()
    {
        Sm.DoOperateFixedUpdate();
    }

    public void AttackEnd()
    {
        Sm.SetState(DicState[MonsterState.Idle]);
        AttackDelay = GameManager.Instance.monsterAttackDelay;
    }

    public void ScaleInversion(bool check)
	{
        if ((check && transform.localScale.x < 0) || (!check && transform.localScale.x > 0))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

    public void GetDamage(int damage)
    {
        hp -= damage;

        if (hp < 0)
            Sm.SetState(DicState[MonsterState.Death]);
    }

    public void Resurrection()
    {
        hp = MaxHp;
        Sm.SetState(DicState[MonsterState.Idle]);
    }
}
