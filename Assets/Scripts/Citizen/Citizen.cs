using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Citizen : MonoBehaviour 
{
	public bool good_to_go = false;

	public int id = 0;
	public CitizenAI ai;
	public float hunger = 1f, 
				 money = 1000f,
				 sleep = 1f,
				 health = 1f,
				 happieness = 1f,
				 social_health = 1f,
				 walk_speed = 3f;
	public Job job;
	public WholeBuilding home;
	Dictionary<Citizen, float> relationships = new Dictionary<Citizen, float> ();
	public float ideal_money = 3000f;

	public float sleep_attrition_rate = 0.001f, 
				 hunger_attrition_rate = 0.002f, 
				 health_attrition_rate = 0.01f, 
				 social_health_attrition_rate = 0.003f;

	List<Action> queued_actions = new List<Action>();
	Action current_action;
	float time_since_action_begining = 0f;
	public Coords2D current_coords;

	public Dictionary<Items, int> inventory = new Dictionary<Items, int>();


	public override int GetHashCode () { return this.id; }

	// Overrides Equals method, compares unique ids of Citizen objects.
	public override bool Equals(object obj) 
	{
		if (obj.GetType() == typeof(Citizen)) { return ((Citizen)obj).id == this.id; }
		return false;
	}


	// Registers the citizen to get a unique ID.
	void Awake() {
		GameObject.FindObjectOfType<CitizenOverseer> ().registerCitizen (this);
		ai = GetComponent<CitizenAI> ();
		if (!ai) {
			gameObject.AddComponent<CitizenAI> ();
			ai = GetComponent<CitizenAI> ();
		}
		queued_actions.Add (new Action (ActionType.Wait, (Vector2)transform.position));
		foreach (Items item in Enum.GetValues(typeof(Items))) {
			inventory.Add (item, 0);
		}
	}


	// Update triggered at every physic frame. Used to update status of the current action.
	void FixedUpdate() {
		if (good_to_go) {
			time_since_action_begining += Time.fixedDeltaTime * Time.timeScale;
			updateActions ();
			applyStatAttrition ();
		}
	}
		
	// Updates current action if there is one, else will get the next plan 
	void updateActions() {
		if (current_action != null) {
			switch (current_action.type) {
			case ActionType.Move:
				updateMoveAction ();
				break;
			case ActionType.Wait:
				updateWaitAction ();
				break;
			case ActionType.Inventory:
				makeInventoryAction ();
				break;
			case ActionType.Spend:
				break;
			}
		} else {
			current_action = getNextAction ();
			if (current_action == null) {
				ai.planNext ();
			} else {
				updateActions ();
			}
		}
	}

	// Update action status if action type is Move
	void updateMoveAction() {
		Vector2 position = new Vector2 (transform.position.x, transform.position.y);
		float distance_walked = walk_speed * Time.fixedDeltaTime * Time.timeScale;
		if (distance_walked < (current_action.destination - position).magnitude) {
			transform.position = transform.position + (Vector3)(current_action.destination - position).normalized * distance_walked;
		} else {
			transform.position = (Vector3)(current_action.destination - position) + transform.position; 
			current_coords = current_action.position_coords;
			current_action = getNextAction ();
			time_since_action_begining = 0f;
			if (current_action == null) { ai.planNext (); }
		}
	}
		
	// Update action status if action type is Wait
	void updateWaitAction() {
		if (time_since_action_begining >= current_action.action_time) {
			getReward ();
			current_action = getNextAction ();
			time_since_action_begining = 0f;
			if (current_action == null) {
				ai.planNext ();
			}
		}
	}

	// Deals with actions of type Inventory
	void makeInventoryAction() {
		inventory [current_action.item] += current_action.inventory_change;
		current_action = getNextAction ();
		updateActions ();
	}

	// Deals with actions of type Inventory
	void makeSpendAction() {
		inventory [current_action.item] += Mathf.Abs(current_action.inventory_change);
		money -= Mathf.Abs(current_action.money_spent);
		current_action = getNextAction ();
		updateActions ();
	}


	// Adds an action
	public void addAction(Action action) {
		addActions (new List<Action> () { action });
	}

	// Adds a range of new actions
	public void addActions(List<Action> actions) {
		queued_actions.AddRange (actions);
	}

	// Return the next action in action thread
	private Action getNextAction() {
		if (queued_actions.Count > 0) {
			Action action = queued_actions [0];
			queued_actions.RemoveAt (0);
			return action;
		}
		return null;
	}


	// Gets the reward from the action
	private void getReward() {
		float amount = current_action.total_reward * Mathf.Max (time_since_action_begining, current_action.action_time) / current_action.action_time;
		switch (current_action.reward) {
		case CitizenStat.Money:
			money += amount;
			break;
		case CitizenStat.Hunger:
			hunger = Mathf.Min (hunger + amount, 1f);
			break;
		case CitizenStat.Sleep:
			sleep = Mathf.Min (sleep + amount, 1f);
			break;
		case CitizenStat.Health:
			health = Mathf.Min (health + amount, 1f);
			break;
		case CitizenStat.SocialHealth:
			social_health = Mathf.Min (social_health + amount, 1f);
			break;
		case CitizenStat.Happiness:
			happieness = Mathf.Min (happieness + amount, 1f);
			break;
		}
	}


	void applyStatAttrition() {
		if (current_action != null) {
			if (current_action.reward != CitizenStat.Sleep) { sleep -= Time.fixedDeltaTime * Time.timeScale * sleep_attrition_rate; }
			if (current_action.reward != CitizenStat.Hunger && current_action.reward != CitizenStat.Health) { 
				hunger -= Time.fixedDeltaTime * Time.timeScale * hunger_attrition_rate;
			}
			if (current_action.reward != CitizenStat.SocialHealth) { social_health -= Time.fixedDeltaTime * Time.timeScale * social_health_attrition_rate; }
		} else {
			sleep -= Time.fixedDeltaTime * Time.timeScale * sleep_attrition_rate;
			hunger -= Time.fixedDeltaTime * Time.timeScale * hunger_attrition_rate;
			social_health -= Time.fixedDeltaTime * Time.timeScale * social_health_attrition_rate;
		}

		sleep = Mathf.Max (sleep, 0f);
		hunger = Mathf.Max (hunger, 0f);
		healthAttrition ();
	}


	void healthAttrition() {
		if (current_action != null) {
			if (current_action.reward != CitizenStat.Health && hunger == 0f) { 
				health -= Time.fixedDeltaTime * Time.timeScale * health_attrition_rate;
			}
		}

		if (health <= 0f) {
			DestroyImmediate (gameObject);
		}
	}

}


public enum CitizenStat
{
	None,
	Money,
	Hunger,
	Sleep,
	Health,
	SocialHealth,
	Happiness
}
