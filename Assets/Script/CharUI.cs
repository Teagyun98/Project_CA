using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharUI : MonoBehaviour
{
	[SerializeField] private CharController character;
	[Header("Under UI")]
	[SerializeField] private TextMeshProUGUI level;
	[SerializeField] private Slider hpBar;
	[SerializeField] private Slider expBar;
	[SerializeField] private Image skillSlider;
	[SerializeField] private Button levelUpBtn;
	[SerializeField] private TextMeshProUGUI levelUpBtnText;
	[Header("Follow HpBar")]
	[SerializeField] private Slider followHpBar;

	private void FixedUpdate()
	{
		followHpBar.transform.position = character.transform.position + new Vector3(0,2,0);
	}

	private void Update()
	{
		hpBar.value = character.hp / character.status.hp;
		followHpBar.value = hpBar.value;
		level.text = $"Lv.{character.Level}";
		expBar.value = (float)character.Exp / (10 + character.Level);
		skillSlider.fillAmount = character.skillDelay / character.status.skillDelay;

		if (GameManager.Instance.Gold >= 100 + (character.Level - 1) * 10 && character.Sm.CurState != character.DicState[CharState.Die])
        {
            levelUpBtn.gameObject.SetActive(true);
			levelUpBtnText.text = $"Level Up!<br>{100 + (character.Level - 1) * 10} Gold";

        }
        else
            levelUpBtn.gameObject.SetActive(false);
    }

	public void LevelUp()
	{
		if (GameManager.Instance.Gold < 100 + (character.Level - 1) * 10)
			return;

		GameManager.Instance.SetGold(-(100 + (character.Level - 1) * 10));
		character.SetExp(10 + character.Level);
	}

}
