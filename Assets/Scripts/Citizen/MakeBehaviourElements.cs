using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBehaviourElements : MonoBehaviour 
{

	// Here we declare the different behaviour elements, to make the behaviour tree.
	// Note that, even though the "Start" behaviour is necessary and must be the first in the list, the design should allow you to make the final behaviour tree however
	// you want it to be.

	// See the CitizenBehaviourElement struct for more info on those objects.
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
			"Get a job",
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
			CitizenStat.Money
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
			new SpendMoneyInfo(10f, Items.Food, 10),
			2f
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
			1,
			Items.Food,
			5f
		);
		behaviour_list.Add (cook);

		CitizenBehaviourElement eat = new CitizenBehaviourElement (
			"Eat",
			new List<CitizenBehaviourElement>() { cook },
			BehaviourType.GetStat,
			CitizenStat.Hunger
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
			CitizenStat.Sleep
		);
		behaviour_list.Add (sleep);

		CitizenBehaviourElement go_to_hospital = new CitizenBehaviourElement (
			"Go to hospital",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.MoveToNearest,
			BuildingType.Hospital
		);
		behaviour_list.Add (go_to_hospital);

		CitizenBehaviourElement heal = new CitizenBehaviourElement (
			"Be healthy",
			new List<CitizenBehaviourElement>() { go_to_hospital },
			BehaviourType.GetStat,
			CitizenStat.Health
		);
		behaviour_list.Add (heal);

		CitizenBehaviourElement go_to_bar = new CitizenBehaviourElement (
			"Go to bar",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.MoveToNearest,
			BuildingType.Bar
		);
		behaviour_list.Add (go_to_bar);

		CitizenBehaviourElement get_drink = new CitizenBehaviourElement (
			"Get a drink",
			new List<CitizenBehaviourElement>() { go_to_bar },
			BehaviourType.SpendMoney,
			new SpendMoneyInfo(10f, Items.Drink, 1)
		);
		behaviour_list.Add (get_drink);

		CitizenBehaviourElement drink = new CitizenBehaviourElement (
			"Drink",
			new List<CitizenBehaviourElement>() { get_drink },
			BehaviourType.None,
			0f,
			Items.Drink,
			5f
		);
		behaviour_list.Add (drink);

		CitizenBehaviourElement go_to_park = new CitizenBehaviourElement (
			"Go to the park",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.MoveToNearest,
			BuildingType.Park
		);
		behaviour_list.Add (go_to_park);

		CitizenBehaviourElement have_a_walk = new CitizenBehaviourElement (
			"Have a walk",
			new List<CitizenBehaviourElement>() { go_to_park },
			BehaviourType.None,
			2000f,
			10f
		);
		behaviour_list.Add (have_a_walk);

		CitizenBehaviourElement discuss = new CitizenBehaviourElement (
			"Discuss",
			new List<CitizenBehaviourElement>() { get_drink, have_a_walk },
			BehaviourType.None,
			0f,
			10f
		);
		behaviour_list.Add (discuss);

		CitizenBehaviourElement be_social = new CitizenBehaviourElement (
			"Be social",
			new List<CitizenBehaviourElement>() { discuss },
			BehaviourType.GetStat,
			CitizenStat.SocialHealth
		);
		behaviour_list.Add (be_social);

		CitizenBehaviourElement go_to_church = new CitizenBehaviourElement (
			"Go to church",
			new List<CitizenBehaviourElement>() { start },
			BehaviourType.MoveToNearest,
			BuildingType.Church
		);
		behaviour_list.Add (go_to_church);

		CitizenBehaviourElement pray = new CitizenBehaviourElement (
			"Pray",
			new List<CitizenBehaviourElement>() { go_to_church },
			BehaviourType.None,
			2000f,
			10f
		);
		behaviour_list.Add (pray);

		CitizenBehaviourElement be_happy = new CitizenBehaviourElement (
			"Be happie",
			new List<CitizenBehaviourElement>() { get_drink, have_a_walk, pray },
			BehaviourType.GetStat,
			CitizenStat.Happiness
		);
		behaviour_list.Add (be_happy);

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
			CitizenStat.None
		);
		behaviour_list.Add (be_idle);

		return behaviour_list;
	}

}
