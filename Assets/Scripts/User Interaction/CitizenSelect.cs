using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitizenSelect : MonoBehaviour 
{
	
	public bool active = true;
	GameObject selected_citizen;
	Text tooltip;


	// On start, selects the tooltip Text object, for display purpose.
	void Start() {
		tooltip = GameObject.Find ("CitizenTooltip").transform.FindChild ("Text").gameObject.GetComponent<Text> ();
	}

	
	// If left click pressed, calls Fire1Raycast; updates selected citizen's tooltip on every rendered frame.
	void Update () {
		if (Input.GetButtonDown ("Fire1") && active) { Fire1Raycast (); }

		if (active && selected_citizen) {
			updateTooltip ();
		} else {
			tooltip.text = "No citizen selected";
		}
	}


	// Selection raycast on citizen.
	void Fire1Raycast() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (selected_citizen) {
			while (selected_citizen.transform.childCount > 0) {
				DestroyImmediate(selected_citizen.transform.GetChild(0).gameObject);
			}
		}
		selected_citizen = null;
		if (hit.collider && hit.collider.gameObject.GetComponent<Citizen>()) {
			selected_citizen = hit.collider.gameObject;
			GameObject mask = Instantiate(Resources.Load("CitizenSelectMask", typeof(GameObject))) as GameObject;
			mask.transform.SetParent (selected_citizen.transform);
			mask.transform.localPosition = new Vector3 (0f, 0f, -1.5f);
		}
	}


	// Updates the citizen tooltip text
	void updateTooltip() {
		Citizen citizen = selected_citizen.GetComponent<Citizen> ();
		string text = "Citizen n°" + citizen.id.ToString() + "\n";
		text += "Health: " + Mathf.Round (citizen.health * 100f).ToString () + "\tMoney: " + Mathf.RoundToInt(citizen.money).ToString () + "\n";
		text += "Hunger: " + Mathf.Round (citizen.hunger * 100f).ToString () + "\tSleep: " + Mathf.Round (citizen.sleep * 100f).ToString () + "\n";
		text += "Social health: " + Mathf.Round (citizen.social_health * 100f).ToString () + "\tHappieness: " + Mathf.Round (citizen.happieness * 100f).ToString () + "\n";
		text += "Ideal money: " + Mathf.Round (citizen.ideal_money).ToString() + "\n";
		if (citizen.job != null) {
			text += "\nCurrently has a job:\n";
			text += "  Works at " + citizen.job.employer.gameObject.name + "\n";
			text += "  Salary: " + citizen.job.pay_per_session.ToString() + "\n";
		} else {
			text += "\nCurrently has no job\n";
		}
		text += "\n" + citizen.ai.current_status;
		text += "\nInventory:\n";
		foreach (KeyValuePair<Items, int> kvp in citizen.inventory) {
			text += "   " + kvp.Key.ToString () + ": " + kvp.Value.ToString() + "\n";
		}
		text += "\nAdditionnal info:\n" + citizen.ai.additionnal_info;
		print ("Info: " + citizen.ai.additionnal_info);
		tooltip.text = text;
	}


}
