using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CitizenBehaviourElement {

	public string description;
	public List<CitizenBehaviourElement> canComeFrom;
	public BehaviourType type;
	public object objective;
	public float time;
	public Items required_item;

	/* The description serves as a string ID of the element. 
	 * You have then the behaviour type, with an objective.
	 * The objective depends on the the behaviour type:
	 * 		• MoveToSpecific: takes a Wholebuilding object, the destination building
	 * 		• MoveToNearest: takes a BuildingType item, the nearest type of building you want the citizen to go to
	 * 		• SpendMoney: takes a SpendMoneyInfo object, will buy the given amount of the specified object, at the given price
	 * 		• None: takes a float value, giving a constant "weight" on this element in the behaviour tree
	 * 		• GetStat: takes a CitizenStat item, replenishes a fixed amount, not configurable here
	 * 		• GetWork: doesn't take any argument, leads to a custom action
	 * 
	 * You then have the time it takes (1f = 1s at timescale = 1.)
	 * The action the behaviour element will induce may require the use of a given item to happen (ex: cooking requires a food item)
	 */

	public CitizenBehaviourElement(string desc, List<CitizenBehaviourElement> canComeFrom, BehaviourType behaviour_type, object objective, float time = 0f) {
		description = desc;
		this.canComeFrom = canComeFrom;
		type = behaviour_type;
		this.objective = objective;
		this.time = time;
		required_item = Items.None;
	}

	public CitizenBehaviourElement(string desc, List<CitizenBehaviourElement> canComeFrom, BehaviourType behaviour_type, object objective, Items requires, float time = 0f) {
		description = desc;
		this.canComeFrom = canComeFrom;
		type = behaviour_type;
		this.objective = objective;
		this.time = time;
		required_item = requires;
	}


	public override bool Equals(object obj) {
		if (obj == null) {
			return false;
		}
		if (obj.GetType() == typeof(CitizenBehaviourElement)) {
			return ((CitizenBehaviourElement)obj).description == this.description;
		}
		return false;
	}

	public override int GetHashCode() {
		return description.GetHashCode ();
	}


	public static bool operator == (CitizenBehaviourElement obj1, CitizenBehaviourElement obj2) {
		return obj1.Equals (obj2);
	}

	public static bool operator != (CitizenBehaviourElement obj1, CitizenBehaviourElement obj2) {
		return !obj1.Equals (obj2);
	}

}


public enum BehaviourType {
	MoveToSpecific,
	MoveToNearest,
	SpendMoney,
	None,
	GetStat,
	GetWork
}


public struct SpendMoneyInfo {

	public float money_to_spend;
	public Items good_to_buy;
	public int amount;

	public SpendMoneyInfo(float price, Items article, int amount) {
		money_to_spend = price;
		good_to_buy = article;
		this.amount = amount;
	}

}