using UnityEngine;

public class Thief : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        status = GameManager.Instance.characterStatusList[1];
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
