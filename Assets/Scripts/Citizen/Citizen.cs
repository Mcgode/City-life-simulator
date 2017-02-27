using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour 
{
	public int id = 0;
	CitizenAI ai;
	float hunger = 1f;
	float money = 1000f;
	float sleep = 1f;
	float health = 1f;
	float happieness = 1f;
	float social_health = 1f;
	float walk_speed = 3f;
	Job job;
	Dictionary<Citizen, float> relationships;

	List<Action> queued_actions = new List<Action>();
	Action current_action;
	float time_since_action_begining = 0f;


	public override int GetHashCode ()
	{
		return this.id;
	}

	// Overrides Equals method, compares unique ids of Citizen objects.
	public override bool Equals(object obj) {
		if (obj.GetType() == typeof(Citizen)) {
			return ((Citizen)obj).id == this.id;
		}
		return false;
	}


	// Registers the citizen to get a unique ID.
	void Awake() {
		GameObject.FindObjectOfType<CitizenOverseer> ().registerCitizen (this);
	}

	// AI init and other stuff. 
	void Start() {
		ai = GetComponent<CitizenAI> ();
		if (!ai) {
			gameObject.AddComponent<CitizenAI> ();
			ai = GetComponent<CitizenAI> ();
		}
	}


	// Update triggered at every physic frame. Used to update status of the current action.
	void FixedUpdate() {
		time_since_action_begining += Time.fixedDeltaTime * Time.timeScale;
		if (current_action != null) {
			if (current_action.type == ActionType.Move) {
				updateMoveAction ();
			} else {
				updateWaitAction ();
			}
		} else {
			ai.planNext ();
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
			getReward ();
			current_action = getNextAction ();
			time_since_action_begining = 0f;
			if (current_action == null) { ai.planNext (); }
		}
	}
		
	// Update action status if action type is Wait
	void updateWaitAction() {
		Vector2 position = new Vector2 (transform.position.x, transform.position.y);
		if ((position - current_action.destination).magnitude < 0.5f) {
			if (time_since_action_begining >= current_action.action_time) {
				getReward ();
				current_action = getNextAction ();
				time_since_action_begining = 0f;
				if (current_action == null) { ai.planNext (); }
			}
		} else {
			getReward ();
			ai.planNext();
		}
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
		case RewardType.Money:
			money += amount;
			break;
		case RewardType.Hunger:
			hunger = Mathf.Min (hunger + amount, 1f);
			break;
		case RewardType.Sleep:
			sleep = Mathf.Min (sleep + amount, 1f);
			break;
		case RewardType.Health:
			health = Mathf.Min (health + amount, 1f);
			break;
		case RewardType.SocialHealth:
			social_health = Mathf.Min (social_health + amount, 1f);
			break;
		case RewardType.Happiness:
			happieness = Mathf.Min (happieness + amount, 1f);
			break;
		}
	}

}
