using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Image fill;

    private MonsterController monster;

    private void FixedUpdate()
    {
        if(monster)
            transform.position = monster.transform.position + new Vector3(0, 0.5f, 0);
    }

    public void SetMonster(MonsterController controller)
    {
        monster = controller;
    }

    public void SetSlider(float value)
    {
        hpBar.value = value;
    }

    public void SetColor(bool stun)
    {
        if (stun)
            fill.color = new Color32(255, 255, 0, 255);
        else
            fill.color = new Color32(255, 0, 0, 255);
    }
}
