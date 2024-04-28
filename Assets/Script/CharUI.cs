using System.Collections;
using System.Collections.Generic;
using TMPro;
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
		expBar.value = (float)character.exp / (10 + character.Level);
		skillSlider.fillAmount = character.skillDelay / character.status.skillDelay;
	}
}
