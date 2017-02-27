using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

	public ActionType type;
	public float action_time;
	public Vector2 destination;
	public CitizenStats reward;
	public float total_reward;

	public Action(ActionType action_type, float time, Vector2 at, CitizenStats reward_type = CitizenStats.None, float reward_amount = 0f) {
		type = action_type;
		action_time = time;
		destination = at;
		reward = reward_type;
		total_reward = reward_amount;
	}

}


public enum ActionType
{
	Move,
	Wait
}
