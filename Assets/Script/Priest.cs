public class Priest : CharController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        status = GameManager.Instance.characterStatusList[3];
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
