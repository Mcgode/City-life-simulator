using System.Collections;
using System.Collections.Generic;
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
	public string additionnal_info = "";


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
			additionnal_info = "";
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
			if (behaviour_element.objective.GetType() == typeof(float)) { value = (float)(behaviour_element.objective); }
			if (behaviour_element.required_item != Items.None) { dealWithRequiredItem (behaviour_element); value = 0f; }
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
		case BehaviourType.SpendMoney:
			value = 10000f * ((SpendMoneyInfo)(behaviour_element.objective)).money_to_spend / citizen.money;
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
			if (current_money > 1.1f * ideal_money) { return 20000f; } 
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


	// Returns a value depending on the citizen capacity to have the given item up to the previous action. 
	// Returns a value superior to 10 000 if cannot, to prevent action from happening.
	// For that purpose, it needs to trace back behaviour tree if no item was found in inventory 
	void dealWithRequiredItem(CitizenBehaviourElement behaviour_element) {
		additionnal_info += "  Element \"" + behaviour_element.description + "\" requires an item (" + behaviour_element.required_item.ToString() + ")\n";
		if (citizen.inventory [behaviour_element.required_item] <= 0) {
			foreach (List<CitizenBehaviourElement> path in determinePossiblePaths(behaviour_element)) {
				additionnal_info += "    " + Utils.behaviourPathToString(path) + "\n";
			}
			List<KeyValuePair<int, int>> result = dealWithBuyPaths (behaviour_element);
			additionnal_info += "  Links to break:\n";
			foreach (KeyValuePair<int, int> kvp in result) {
				additionnal_info += "    " + descriptions[kvp.Key] + " -> " + descriptions[kvp.Value] + "\n"; 
				for (int i = 0; i < links[kvp.Key].Count; i++) {
					if (links [kvp.Key] [i].Key == kvp.Value) {
						links [kvp.Key] [i] = new KeyValuePair<int, float>(kvp.Value, 10000f);
						break;
					}
				}
			}
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
				list.AddRange (getNoneAction (behaviour_element));
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
		if (movement_path.Count == 0) { return actions; }
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
			if (citizen.job != null) { return new Action (ActionType.Wait, 10f, CitizenStat.Money, 200f); }
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


	List<Action> getNoneAction(CitizenBehaviourElement behaviour) {
		List<Action> actions_to_perform = new List<Action> ();
		if (behaviour.objective.GetType () == typeof(SpendMoneyInfo)) {
			SpendMoneyInfo info = (SpendMoneyInfo)(behaviour.objective);
			actions_to_perform.Add (new Action (ActionType.Spend, info.good_to_buy, info.money_to_spend, info.amount));
		} else if (behaviour.required_item != Items.None) {
			actions_to_perform.Add (new Action (ActionType.Inventory, behaviour.required_item, -(int)(behaviour.objective)));
		}
		actions_to_perform.Add (new Action (ActionType.Wait, behaviour.time));
		return actions_to_perform;
	}


	// Recursively determines the possible paths from "Start" (or else, but it is an error, since that means your first element is inaccessible) to the given depart element 
	List<List<CitizenBehaviourElement>> determinePossiblePaths(CitizenBehaviourElement depart_element) {
		List<CitizenBehaviourElement> previous_elements = depart_element.canComeFrom;
		List<List<CitizenBehaviourElement>> paths = new List<List<CitizenBehaviourElement>> ();

		if (previous_elements.Count == 0) {
			return new List<List<CitizenBehaviourElement>> () { new List<CitizenBehaviourElement>() { depart_element } };
		}

		foreach (CitizenBehaviourElement element in previous_elements) {
			foreach (List<CitizenBehaviourElement> list in determinePossiblePaths(element)) {
				list.Add (depart_element);
				paths.Add (list);
			}
		}
		return paths;
	}


	// Returns the list of links to "break" (by putting their values to 10,000) since they lead to paths where you can't buy the required item
	List<KeyValuePair<int, int>> dealWithBuyPaths(CitizenBehaviourElement element) {
		List<KeyValuePair<int, int>> links_to_break = new List<KeyValuePair<int, int>> ();
		List<List<CitizenBehaviourElement>> paths = determinePossiblePaths (element);
		paths = cleanPossiblePaths (paths);
		List<List<CitizenBehaviourElement>> working_paths = getWorkingPaths (paths, element);
		List<List<CitizenBehaviourElement>> path_with_no_buy_option = getNoBuyPath(paths, element.required_item);
		List<int> indexes = workingPathsPostProcess (working_paths, element.required_item);
		indexes.Sort (); 
		indexes.Reverse ();
		foreach (int index in indexes) {
			path_with_no_buy_option.Add (working_paths [index]);
			working_paths.RemoveAt (index);
		}
		additionnal_info += "  Working paths:\n";
		foreach (List<CitizenBehaviourElement> path in working_paths) { additionnal_info += "    " + Utils.behaviourPathToString(path) + "\n"; }
		additionnal_info += "  Not working paths:\n";
		foreach (List<CitizenBehaviourElement> path in path_with_no_buy_option) { additionnal_info += "    " + Utils.behaviourPathToString(path) + "\n"; }
		foreach (List<CitizenBehaviourElement> path in path_with_no_buy_option) {
			List<int> working_path_current_index = new List<int> ();
			for (int i = 0; i < working_paths.Count; i++) { working_path_current_index.Add (0); }
			for (int i = 1; i < path.Count; i++) {
				CitizenBehaviourElement step = path [i];
				bool got_a_link = false;
				for (int j = 0; j < working_paths.Count; j++) {
					int index = working_paths [j].IndexOf (step);
					if (index == working_path_current_index[j] + 1) {
						got_a_link = true;
					}
					working_path_current_index [j] = Mathf.Max (working_path_current_index [j], index);
				}
				if (!got_a_link) {
					links_to_break.Add (new KeyValuePair<int, int> (descriptions.IndexOf (path [i - 1].description), descriptions.IndexOf (path [i].description)));
					break;
				}
			}
		}
		return links_to_break;
	}
		

	List<List<CitizenBehaviourElement>> getWorkingPaths(List<List<CitizenBehaviourElement>> paths, CitizenBehaviourElement element) {
		List<List<CitizenBehaviourElement>> working_paths = new List<List<CitizenBehaviourElement>> ();
		foreach (List<CitizenBehaviourElement> path in paths) {
			for (int i = 0; i < path.Count; i++) {
				CitizenBehaviourElement path_element = path[i];
				if (path_element.objective.GetType () == typeof(SpendMoneyInfo) && ((SpendMoneyInfo)(path_element.objective)).good_to_buy == element.required_item) {
					working_paths.Add (path);
					break;
				}
			}
		}
		return working_paths;
	}


	List<List<CitizenBehaviourElement>> getNoBuyPath(List<List<CitizenBehaviourElement>> paths, Items required_item) {
		List<List<CitizenBehaviourElement>> not_working_paths = new List<List<CitizenBehaviourElement>> ();
		foreach (List<CitizenBehaviourElement> path in paths) {
			bool got_a_buying_element = false;
			foreach (CitizenBehaviourElement element in path) {
				if (element.objective.GetType () == typeof(SpendMoneyInfo) && ((SpendMoneyInfo)(element.objective)).good_to_buy == required_item) {
					got_a_buying_element = true;
				}
			}
			if (!got_a_buying_element) { not_working_paths.Add (path); }
		}
		return not_working_paths;
	}


	List<int> workingPathsPostProcess(List<List<CitizenBehaviourElement>> working_paths, Items required_item) {
		List<int> indexes = new List<int> ();
		Dictionary<CitizenBehaviourElement,List<List<CitizenBehaviourElement>>> paths_per_buy_item = new Dictionary<CitizenBehaviourElement, List<List<CitizenBehaviourElement>>> ();
		foreach (List<CitizenBehaviourElement> path in working_paths) {
			foreach (CitizenBehaviourElement element in path) {
				if (element.objective.GetType () == typeof(SpendMoneyInfo) && ((SpendMoneyInfo)(element.objective)).good_to_buy == required_item) {
					if (!paths_per_buy_item.ContainsKey (element)) {
						paths_per_buy_item.Add (element, new List<List<CitizenBehaviourElement>>());
					}
					paths_per_buy_item[element].Add (path);
				}
			}
		}
		Dictionary<CitizenBehaviourElement, float> shortest_distances = new Dictionary<CitizenBehaviourElement, float> ();
		foreach (KeyValuePair<CitizenBehaviourElement,List<List<CitizenBehaviourElement>>> kvp in paths_per_buy_item) {
			float min = getDistanceOfPath(kvp.Value[0]);
			foreach (List<CitizenBehaviourElement> path in kvp.Value) {
				float distance = getDistanceOfPath (path);
				if (distance < min) {
					min = distance;
				}
			}
			shortest_distances.Add (kvp.Key, min);
		}
		CitizenBehaviourElement shortest_element = null;
		float min_tot = 200000f;
		foreach (KeyValuePair<CitizenBehaviourElement, float> kvp in shortest_distances) {
			if (kvp.Value < min_tot) {
				min_tot = kvp.Value;
				shortest_element = kvp.Key;
			}
		}
		if (shortest_element != null) {
			for (int i = 0; i < working_paths.Count; i++) {
				if (!working_paths[i].Contains (shortest_element)) {
					indexes.Add (i);
				}
			}
		}
		return indexes;
	}


	float getDistanceOfPath(List<CitizenBehaviourElement> path) {
		float distance = 0f;
		for (int i = 1; i < path.Count; i++) {
			int int1 = descriptions.IndexOf (path [i - 1].description);
			int int2 = descriptions.IndexOf (path [i].description);
			foreach (KeyValuePair<int, float> link in links[int1]) {
				if (link.Key == int2) {
					distance += link.Value;
					break;
				}
			}
		}
		return distance;
	}

	List<List<CitizenBehaviourElement>> cleanPossiblePaths(List<List<CitizenBehaviourElement>> original) {
		List<List<CitizenBehaviourElement>> cleaned = new List<List<CitizenBehaviourElement>> ();
		foreach (List<CitizenBehaviourElement> path in original) {
			if (path [0].description == "Start") {
				cleaned.Add (path);
			}
		}
		return cleaned;
	}


}
