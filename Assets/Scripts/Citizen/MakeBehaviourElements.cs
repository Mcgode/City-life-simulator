using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBehaviourElements : MonoBehaviour {

	public static List<CitizenBehaviourElement> getBehaviours(Citizen citizen)
	{
		List<CitizenBehaviourElement> behaviour_list = new List<CitizenBehaviourElement> ();

		CitizenBehaviourElement start = new CitizenBehaviourElement (
			"Start",
			new List<CitizenBehaviourElement>(),
			BehaviourType.None,
			0f
		);
		behaviour_list.Add (start);

		CitizenBehaviourElement get_work = new CitizenBehaviourElement (
			"Get a work",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.GetWork,
			0f
		);
		behaviour_list.Add (get_work);

		CitizenBehaviourElement go_to_work = new CitizenBehaviourElement (
			"Go to work",
			new List<CitizenBehaviourElement>() { start, get_work },
			BehaviourType.MoveToSpecific,
			citizen.job
		);
		behaviour_list.Add (go_to_work);

		CitizenBehaviourElement work = new CitizenBehaviourElement (
			"Work",
			new List<CitizenBehaviourElement>() { go_to_work },
			BehaviourType.None,
			0f
		);
		behaviour_list.Add (work);

		CitizenBehaviourElement earn_money = new CitizenBehaviourElement (
			"Earn money",
			new List<CitizenBehaviourElement>() { work },
			BehaviourType.GetStat,
			CitizenStats.Money
		);
		behaviour_list.Add (earn_money);

		CitizenBehaviourElement get_food = new CitizenBehaviourElement (
			"Go get food",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.MoveToNearest,
			BuildingType.SuperMarket
		);
		behaviour_list.Add (get_food);

		CitizenBehaviourElement buy_food = new CitizenBehaviourElement (
			"Buy food",
			new List<CitizenBehaviourElement>() { get_food },
			BehaviourType.SpendMoney,
			new SpendMoneyInfo(10f, "food", 10)
		);
		behaviour_list.Add (buy_food);


		CitizenBehaviourElement go_home = new CitizenBehaviourElement (
			"Go home",
			new List<CitizenBehaviourElement>() { start, buy_food },
			BehaviourType.MoveToSpecific,
			citizen.home
		);
		behaviour_list.Add (go_home);

		CitizenBehaviourElement cook = new CitizenBehaviourElement (
			"Cook",
			new List<CitizenBehaviourElement>() { go_home },
			BehaviourType.None,
			0f
		);
		behaviour_list.Add (cook);

		CitizenBehaviourElement eat = new CitizenBehaviourElement (
			"Eat",
			new List<CitizenBehaviourElement>() { cook },
			BehaviourType.GetStat,
			CitizenStats.Hunger
		);
		behaviour_list.Add (eat);

		CitizenBehaviourElement go_to_bed = new CitizenBehaviourElement (
			"Go to bed",
			new List<CitizenBehaviourElement>() { go_home },
			BehaviourType.None,
			0f
		);
		behaviour_list.Add (go_to_bed);

		CitizenBehaviourElement sleep = new CitizenBehaviourElement (
			"Sleep",
			new List<CitizenBehaviourElement>() { go_to_bed },
			BehaviourType.GetStat,
			CitizenStats.Sleep
		);
		behaviour_list.Add (sleep);

		CitizenBehaviourElement idle = new CitizenBehaviourElement (
			"Stay idle",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.None,
			10000f
		);
		behaviour_list.Add (idle);

		CitizenBehaviourElement be_idle = new CitizenBehaviourElement (
			"Be idle",
			new List<CitizenBehaviourElement>() { idle },
			BehaviourType.GetStat,
			CitizenStats.None
		);
		behaviour_list.Add (be_idle);

		return behaviour_list;
	}

}
