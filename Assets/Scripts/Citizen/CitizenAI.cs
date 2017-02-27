using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class CitizenAI : MonoBehaviour 
{

	World world;
	BuildingManager building_manager;
	List<CitizenBehaviourElement> behaviour_list = new List<CitizenBehaviourElement>();
	List<int> get_stat_behaviours = new List<int> ();
	List<string> descriptions = new List<string>();
	List<List<KeyValuePair<int, float>>> links;
	public Citizen citizen;
	bool currently_planning = false;


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


	public void planNext() {
		if (!currently_planning) {
			makeLinks ();
			currently_planning = true;
			List<int> plan = GraphTools.getShortestDistanceToNodeList (links, 0, get_stat_behaviours);
			print ("found a plan");
			currently_planning = false;
			foreach (int e in plan) {
				print (e.ToString() + ": " + behaviour_list[e].description);
			}
		}
	}


	public KeyValuePair<float, List<Coords2D>> getPathDataToCloserBuildingType(BuildingType type, Coords2D depart_position) {
		List<Coords2D> potential_targets = new List<Coords2D> ();
		foreach (WholeBuilding building in building_manager.buildings[type]) {
			foreach (EntranceNode node in building.entrances) {
				potential_targets.Add (Coords2D.getCoords (node.gameObject));
			}
		}
		return world.pathfindFromCoordinatesMultipleTargets (depart_position, potential_targets);
	}


	private void makeLinks() {
		links = new List<List<KeyValuePair<int, float>>> ();
		for (int i = 0; i < behaviour_list.Count; i++) {
			links.Add(new List<KeyValuePair<int, float>>());
			foreach (CitizenBehaviourElement element in behaviour_list[i].canComeFrom) {
				links [descriptions.IndexOf (element.description)].Add (new KeyValuePair<int, float> (i, getLength (behaviour_list[i])));
			}
		}
	}


	private float getLength(CitizenBehaviourElement behaviour_element) {
		float value = 20000f;

		switch (behaviour_element.type) {
		case BehaviourType.None:
			value = (float)(behaviour_element.objective);
			break;
		case BehaviourType.MoveToNearest:
			break;
		}
		return value;
	}

}
