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
		if (target) {
			target.SetActive (!target.active);
			float color_value = 1f;
			if (target.active) { color_value -= enabled_shift; }
			sender.GetComponent<Image> ().color = new Color(color_value, color_value, color_value);
		} else {
			print ("No target found");
		}
	}

}
