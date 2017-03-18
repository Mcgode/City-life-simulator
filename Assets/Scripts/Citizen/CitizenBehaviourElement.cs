using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CitizenBehaviourElement {

	public string description;
	public List<CitizenBehaviourElement> canComeFrom;
	public BehaviourType type;
	public object objective;
	public float time;
	public Items required_item;


	public CitizenBehaviourElement(string desc, List<CitizenBehaviourElement> canComeFrom, BehaviourType behaviour_type, object objective, float time = 0f, Items requires = Items.None) {
		description = desc;
		this.canComeFrom = canComeFrom;
		type = behaviour_type;
		this.objective = objective;
		this.time = time;
		required_item = requires;
	}

	public override bool Equals(object obj) {
		if (obj.GetType() == typeof(CitizenBehaviourElement)) {
			return ((CitizenBehaviourElement)obj).description == this.description;
		}
		return false;
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