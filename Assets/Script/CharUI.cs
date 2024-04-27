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
	[SerializeField] private Image skillSlider;

	private void Update()
	{
		hpBar.value = character.hp / character.status.hp;
		skillSlider.fillAmount = character.skillDelay / character.status.skillDelay;
	}
}
