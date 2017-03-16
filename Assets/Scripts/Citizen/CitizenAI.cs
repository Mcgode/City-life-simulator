using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class CitizenAI : MonoBehaviour 
{

	World world;
	BuildingManager building_manager;
	public List<CitizenBehaviourElement> behaviour_list = new List<CitizenBehaviourElement>();
	List<int> get_stat_behaviours = new List<int> ();
	List<string> descriptions = new List<string>();
	List<List<KeyValuePair<int, float>>> links;
	public Citizen citizen;
	bool currently_planning = false;

	public string current_status = "";


	// Get all important data initialized
	private void Awake() {
		world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
		building_manager = GameObject.FindGameObjectWithTag("World").GetComponent<BuildingManager>();
		citizen = GetComponent<Citizen> ();
		if (!citizen) { DestroyImmediate (this); }
		behaviour_list = MakeBehaviourElements.getBehaviours (citizen);
		for (int i = 0; i < behaviour_list.Count; i++) {
			if (behaviour_list [i].type == BehaviourType.GetStat) {
				get_stat_behaviours.Add (i);
			}
			descriptions.Add (behaviour_list [i].description);
		}
	}


	// The AI plans the next thread of actions
	public void planNext() {
		if (!currently_planning) {
			makeLinks ();
			currently_planning = true;
			List<int> plan = GraphTools.getShortestDistanceToNodeList (links, 0, get_stat_behaviours);
			current_status = "Current plan:\n";
			foreach (int e in plan) {
				current_status += "   " + e.ToString() + ": " + behaviour_list[e].description + "\n";
			}
			citizen.addActions (getActionsFromData (plan));
			current_status += "Priorities at last planning:\n" ;
			foreach (int index in get_stat_behaviours) {
				current_status += "   " + behaviour_list [index].description + " (";
				current_status += Mathf.Round (GraphTools.getShortestDistanceToNodeListKVP (links, 0, new List<int> () { index }).Key).ToString () + ")\n";
			}
			currently_planning = false;
		}
	}


	// Makes the links between the behaviour nodes, mainly aimed at getting the priority values.
	private void makeLinks() {
		links = new List<List<KeyValuePair<int, float>>> ();
		for (int i = 0; i < behaviour_list.Count; i++) {
			links.Add(new List<KeyValuePair<int, float>>());
			foreach (CitizenBehaviourElement element in behaviour_list[i].canComeFrom) {
				int index_of_last = descriptions.IndexOf (element.description);
				links [index_of_last].Add (new KeyValuePair<int, float> (i, getLength (behaviour_list[i], behaviour_list[index_of_last])));
			}
		}
	}


	// Returns a float value to represent the priority of the behaviour
	private float getLength(CitizenBehaviourElement behaviour_element, CitizenBehaviourElement last_element) {
		float value = 20000f;

		switch (behaviour_element.type) {
		case BehaviourType.None:
			value = (float)(behaviour_element.objective);
			break;
		case BehaviourType.MoveToNearest:
			value = 10f * getPathDataToCloserBuildingType ((BuildingType)(behaviour_element.objective), citizen.current_coords).Key;
			break;
		case BehaviourType.MoveToSpecific:
			if (behaviour_element.objective != null) { value = 10f * getPathDataToSpecific (behaviour_element.objective).Key; }
			if (behaviour_element.description == "Go to work" && last_element.description == "Get a job") { value = 0f; }
			break;
		case BehaviourType.GetStat:
			value = getValueFromStat ((CitizenStat)(behaviour_element.objective));
			break;
		case BehaviourType.GetWork:
			value = 1000f;
			break;
		}
		return value;
	}


	// Return the shortest path and its length to a given Building type
	public KeyValuePair<float, List<Coords2D>> getPathDataToCloserBuildingType(BuildingType type, Coords2D depart_position) {
		List<Coords2D> potential_targets = new List<Coords2D> ();
		foreach (WholeBuilding building in building_manager.buildings[type]) {
			foreach (EntranceNode node in building.entrances) {
				potential_targets.Add (Coords2D.getCoords (node.gameObject));
			}
		}
		return world.pathfindFromCoordinatesMultipleTargets (depart_position, potential_targets);
	}


	// Return the shortest path and its length to a WholeBuilding object
	KeyValuePair<float, List<Coords2D>> getPathDataToSpecific(object objective) {
		if (objective.GetType () == typeof(WholeBuilding)) {
			List<Coords2D> entrance_indexes = new List<Coords2D> ();
			foreach (EntranceNode entrance in ((WholeBuilding)(objective)).entrances) {
				entrance_indexes.Add(Coords2D.getCoords(entrance.gameObject));
			}
			return world.pathfindFromCoordinatesMultipleTargets (citizen.current_coords, entrance_indexes);
		} else if (objective.GetType () == typeof(Job)) {
			List<Coords2D> entrance_indexes = new List<Coords2D> ();
			foreach (EntranceNode entrance in ((Job)(objective)).employer.entrances) {
				entrance_indexes.Add(Coords2D.getCoords(entrance.gameObject));
			}
			return world.pathfindFromCoordinatesMultipleTargets (citizen.current_coords, entrance_indexes);
		}
		return new KeyValuePair<float, List<Coords2D>> (20000f, new List<Coords2D> ());
	}


	// Returns a value from the stats
	float getValueFromStat(CitizenStat stat) {
		switch (stat) {
		case CitizenStat.None:
			return 0f;
		case CitizenStat.Money:
			float current_money = citizen.money, ideal_money = citizen.ideal_money;
			if (current_money > 1.1f * ideal_money) { return 10000f; } 
			else if (current_money < 0.1f * ideal_money) { return 0f; }
			else { return 10000f * (current_money - 0.1f * ideal_money) / ideal_money; }
		case CitizenStat.Hunger:
			return citizen.hunger * 15000f;
		case CitizenStat.Health:
			if (citizen.health >= 1f) { return 20000f; } else { return 1000f * citizen.health * citizen.health; }
		case CitizenStat.Sleep:
			return 10000f * citizen.sleep / 0.4f;
		case CitizenStat.SocialHealth:
			return 15000f * citizen.social_health;
		case CitizenStat.Happiness:
			return 15000f * citizen.happieness;
		default:
			return 20000f;
		}
	}


	List<Action> getActionsFromData(List<int> path) {
		List<Action> list = new List<Action> ();
		for (int i=0; i < path.Count; i++) {
			int index = path[i];
			CitizenBehaviourElement behaviour_element = behaviour_list [index];
			switch (behaviour_element.type) {
			case BehaviourType.MoveToNearest:
			case BehaviourType.MoveToSpecific:
				list.AddRange (getMovementAction (behaviour_element));
				break;
			case BehaviourType.GetStat:
				list.Add (getStatAction (behaviour_element));
				break;
			case BehaviourType.None:
				list.Add (new Action (ActionType.Wait, behaviour_element.time));
				break;
			case BehaviourType.GetWork:
				Job getAJob = tryToGetAJob();
				list.Add (new Action (ActionType.Wait, 5f));
				if (getAJob != null) { citizen.job = getAJob; behaviour_list = MakeBehaviourElements.getBehaviours (citizen); } else { i += 10; }
				break;
			}
		}
		print ("We have " + list.Count.ToString() + " actions to perform");
		return list;
	}


	List<Action> getMovementAction(CitizenBehaviourElement movement_behaviour) {
		List<Coords2D> movement_path;
		List<Action> actions = new List<Action> ();
		if (movement_behaviour.type == BehaviourType.MoveToNearest) { movement_path = getPathDataToCloserBuildingType ((BuildingType)(movement_behaviour.objective), citizen.current_coords).Value; } 
		else { movement_path = getPathDataToSpecific (movement_behaviour.objective).Value; }
		foreach (Coords2D coords in movement_path) { actions.Add (new Action (ActionType.Move, coords.toVector2 ())); }
		foreach (EntranceNode node in world.entrances) {
			Coords2D last_coords = movement_path [movement_path.Count - 1];
			if (Coords2D.getCoords (node.gameObject) == last_coords) {
				switch (node.buildingEntranceDirection) {
				case Direction.Up:
					actions.Add (new Action (ActionType.Move, (last_coords + new Coords2D(0, 1)).toVector2 (), last_coords));
					break;
				case Direction.Down:
					actions.Add (new Action (ActionType.Move, (last_coords + new Coords2D(0, -1)).toVector2 (), last_coords));
					break;
				case Direction.Right:
					actions.Add (new Action (ActionType.Move, (last_coords + new Coords2D(1, 0)).toVector2 (), last_coords));
					break;
				case Direction.Left:
					actions.Add (new Action (ActionType.Move, (last_coords + new Coords2D(-1, 0)).toVector2 (), last_coords));
					break;
				}
			}
		}
		return actions;
	}


	Action getStatAction(CitizenBehaviourElement behaviour) {
		switch ((CitizenStat)(behaviour.objective)) {
		case CitizenStat.None:
			return new Action (ActionType.Wait, 5f);
		case CitizenStat.Money:
			if (citizen.job != null) { return new Action (ActionType.Wait, 0f, CitizenStat.Money, 100f); }
			return new Action (ActionType.Wait, 0f, CitizenStat.Money, 0f);
		case CitizenStat.Hunger:
			return new Action (ActionType.Wait, 0f, CitizenStat.Hunger, 0.4f);
		case CitizenStat.Health:
			return new Action (ActionType.Wait, 20f, CitizenStat.Health, 1f);
		case CitizenStat.Sleep:
			return new Action (ActionType.Wait, 20f, CitizenStat.Sleep, 0.8f);
		case CitizenStat.Happiness:
			return new Action (ActionType.Wait, 0f, CitizenStat.Happiness, 0.2f);
		case CitizenStat.SocialHealth:
			return new Action (ActionType.Wait, 0f, CitizenStat.SocialHealth, 0.8f);
		default:
			return new Action (ActionType.Wait, 0f, CitizenStat.None, 0f);
		}

	}


	// Returns a Job if was recruited
	Job tryToGetAJob() {
		WholeBuilding potential_workplace = building_manager.pickAWorkplace ();
		if (potential_workplace == null) { return null; }
		float score = Random.value;
		float min_score = (float)potential_workplace.employees.Count / (float)potential_workplace.capacity;
		print ("Score : " + score.ToString() + "/" + min_score.ToString() + " => " + (score - min_score).ToString());
		if (score - min_score >= 0f) {
			print("Got new job!");
			return new Job (potential_workplace, citizen);
		}
		return null;
	}


}
