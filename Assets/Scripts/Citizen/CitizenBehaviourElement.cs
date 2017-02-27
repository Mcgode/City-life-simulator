using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CitizenBehaviourElement {

	public string description;
	public List<CitizenBehaviourElement> canComeFrom;
	public BehaviourType type;
	public object objective;


	public CitizenBehaviourElement(string desc, List<CitizenBehaviourElement> canComeFrom, BehaviourType behaviour_type, object objective) {
		description = desc;
		this.canComeFrom = canComeFrom;
		type = behaviour_type;
		this.objective = objective;
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

	float money_to_spend;
	string good_to_buy;
	int amount;

	public SpendMoneyInfo(float price, string article, int amount) {
		money_to_spend = price;
		good_to_buy = article;
		this.amount = amount;
	}

}