using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

	public ActionType type;
	public float action_time;
	public Vector2 destination;
	public CitizenStat reward;
	public float total_reward;
	public Coords2D position_coords;
	public Items item;
	public int inventory_change;
	public float money_spent;

	public Action(ActionType action_type, Vector2 at) {
		type = action_type;
		action_time = 0f;
		destination = at;
		reward = CitizenStat.None;
		total_reward = 0f;
		position_coords = new Coords2D (Mathf.RoundToInt (at.x), Mathf.RoundToInt (at.y));
	}

	public Action(ActionType action_type, Vector2 at, Coords2D force_position_coords) {
		type = action_type;
		action_time = 0f;
		destination = at;
		reward = CitizenStat.None;
		total_reward = 0f;
		position_coords = force_position_coords;
	}

	public Action(ActionType action_type, float time, CitizenStat reward_type = CitizenStat.None, float reward_amount = 0f) {
		type = action_type;
		action_time = time;
		destination = new Vector2();
		reward = reward_type;
		total_reward = reward_amount;
		position_coords = new Coords2D (0, 0);
	}

	public Action(ActionType action_type, Items inventory_item, int change) {
		type = action_type;
		item = inventory_item;
		inventory_change = change;
	}

	public Action(ActionType action_type, Items inventory_item, float money_spent, int change) {
		type = action_type;
		item = inventory_item;
		this.money_spent = money_spent; 
		inventory_change = change;
	}

	public Action(ActionType action_type, CitizenStat affected_behaviour, float amount = 0.2f) {
		type = action_type;
		total_reward = amount;
		reward = affected_behaviour;
	}

}


public enum ActionType
{
	Move,
	Wait,
	Inventory,
	Spend,
	ReduceEfficiency
}
