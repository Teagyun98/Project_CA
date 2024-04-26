using UnityEngine;

public class Knight : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        // 초기 직업 설정
        status = GameManager.Instance.characterStatusList[0];
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void AttackEnd()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < status.attackDistance)
        {
            target.GetDamage(status.attackPower);
        }

        Sm.SetState(DicState[CharState.Idle]);
    }
}
