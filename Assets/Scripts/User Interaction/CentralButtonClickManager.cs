using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentralButtonClickManager : MonoBehaviour 
{

	public float enabled_shift;

	public void clickNodes(GameObject sender) {
		GameObject world = GameObject.Find ("World");
		GameObject target = world.transform.FindChild ("Node Display").gameObject;
		CitizenSelect citizen_select = GameObject.FindObjectOfType<CitizenSelect> ();
		GameObject citizen_tooltip = transform.FindChild ("CitizenTooltip").gameObject;
		if (target) {
			citizen_select.active = target.active;
			citizen_tooltip.SetActive(target.active);
			target.SetActive (!target.active);
			float color_value = 1f;
			if (target.active) { color_value -= enabled_shift; }
			sender.GetComponent<Image> ().color = new Color(color_value, color_value, color_value);
		} else {
			print ("No target found");
		}
	}

}
