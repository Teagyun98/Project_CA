using System.Collections.Generic;
using UnityEngine;

// 몬스터 상태 Enum
public enum MonsterState
{
    Idle,
    Run,
    Attack,
    Death,
}

// 캐릭터에서 사용된 인터페이스를 가져와 사용
// 몬스터 Idle 클래스
public class MonsterIdle : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender) 
    {
        controller = sender;
        controller.Animator.SetBool("Idle", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Idle", false); }
    public void OperateUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 없는 경우 반환
        if (controller.target == null)
            return;

        // 목표 캐릭터가 공격 범위보다 멀리 있는 경우 Run 상태로 변경
        if (Vector2.Distance(controller.transform.position, controller.target.transform.position) > controller.AttackDistance)
            controller.Sm.SetState(controller.DicState[MonsterState.Run]);
        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        else if (Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[controller.AttackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }

    }
    public void OperateFixedUpdate(MonsterController sender) { }
}

// 몬스터 Run 클래스
public class MonsterRun : CharState<MonsterController>
{
    MonsterController controller;

    public void OperateEnter(MonsterController sender)
    {
        controller = sender;

        controller.Animator.SetBool("Run", true);
    }
    public void OperateExit(MonsterController sender) { controller.Animator.SetBool("Run", false); }
    public void OperateUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        if (controller.target != null && Vector2.Distance(controller.transform.position, controller.target.transform.position) < controller.AttackDistance)
        {
            controller.Sm.SetState(controller.DicState[controller.AttackDelay <= 0 ? MonsterState.Attack : MonsterState.Idle]);
        }
    }
    public void OperateFixedUpdate(MonsterController sender) 
    {
        // 목표 캐릭터가 없으면 반환
        if (controller.target == null)
            return;

        // 이동 방향과 맞게 몬스터 스케일 변경
        controller.ScaleInversion(controller.transform.position.x < controller.target.transform.position.x);

        // 목표로 1의 속도로 이동
        controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.target.transform.position, 1 * Time.fixedDeltaTime);
    }
}

// 몬스토 Attack 클래스
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

// 몬스터 Death 클래스
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
    // 몬스터 상태들을 Dictionary 변수로 선언
    public Dictionary<MonsterState, CharState<MonsterController>> DicState { get; private set; }
    // StateMachine 변수
    public StateMachine<MonsterController> Sm { get; private set; }
    // 애니메이터 변수
    public Animator Animator { get; private set; }

    // 최대 HP
    public int MaxHp { get; private set; }
    // 현재 HP
    private float hp;
    // 공격력
    public int AttackPower { get; private set; }
    // 공격 딜레이
    public float AttackDelay { get; private set; }
    // 공격 사거리
    public float AttackDistance { get; private set; }

    // 목표 캐릭터
    public CharController target;

    private float stun;

    private void Awake()
    {
        // 애니메이터 초기화
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stun = 0;

        // 몬스터 수치들 가져오기
        MaxHp = GameManager.Instance.monsterHp;
        hp = MaxHp;
        AttackPower = GameManager.Instance.monsterAttackPower;
        AttackDelay = GameManager.Instance.monsterAttackDelay;
        AttackDistance = GameManager.Instance.mosterAttackDistance;

        // 몬스터 상태들 생성
        CharState<MonsterController> idle = new MonsterIdle();
        CharState<MonsterController> run = new MonsterRun();
        CharState<MonsterController> attack = new MonsterAttack();
        CharState<MonsterController> death = new MonsterDeath();

        // 몬스터 상태들 저장
        DicState = new Dictionary<MonsterState, CharState<MonsterController>>
        {
            { MonsterState.Idle, idle},
            { MonsterState.Run, run},
            { MonsterState.Attack, attack},
            { MonsterState.Death, death}
        };

        // StateMachine 초기화
        Sm = new StateMachine<MonsterController>(this, DicState[MonsterState.Idle]);
    }

    private void Update()
    {
        if (GameManager.Instance.GameOver)
            return;

        // 상태이상 중에는 반환
        if (stun > 0)
		{
            stun -= Time.deltaTime;
            return;
        }

        // 공격 딜레이
        if (AttackDelay > 0)
            AttackDelay -= Time.deltaTime;

        // 목표 캐릭터 가져오기
        target = GameManager.Instance.NearChar(transform.position);

        // 목표 캐릭터가 없고 Idle 상태나 Death 상태가 아니라면 Idle상태로 변환
        if (target == null && Sm.CurState != DicState[MonsterState.Idle] && Sm.CurState != DicState[MonsterState.Death])
            Sm.SetState(DicState[MonsterState.Idle]);

        // StateMachine Update문
        Sm.DoOperateUpdate();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameOver)
            return;

        // 상태이상 중에는 반환
        if (stun > 0)
            return;

        // StateMachine FixedUpdate문
        Sm.DoOperateFixedUpdate();
    }

    // 공격 애니메이션 이벤트 함수
    public void AttackEnd()
    {
        // Idle 상태로 변환
        Sm.SetState(DicState[MonsterState.Idle]);
        // 공격 딜레이 초기화
        AttackDelay = GameManager.Instance.monsterAttackDelay;

        // 목표 캐릭터가 공격 범위에 있고 공격 딜레이가 0보다 작으면 Attack 아니면 Idle 상태로 변경
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < AttackDistance)
            target.GetDamage(AttackPower);
    }

    // 몬스터 스케일 변환 함수
    public void ScaleInversion(bool check)
	{
        if ((check && transform.localScale.x < 0) || (!check && transform.localScale.x > 0))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

    // 몬스터가 공격 받을 때 사용되는 함수
    public void GetDamage(float damage, CharController charController)
    {
        hp -= damage;

        // 데미지를 받고 hp가 0보다 적어지면 Death 상태로 변환
        if (hp <= 0)
		{
            Sm.SetState(DicState[MonsterState.Death]);
            charController.SetExp(1);
        }
    }

    // 몬스터 부활 함수
    public void Resurrection()
    {
        hp = MaxHp;
        Sm.SetState(DicState[MonsterState.Idle]);
    }

    public void GetStun(float time)
	{
        stun += time;
	}
}
