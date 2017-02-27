using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAI : MonoBehaviour 
{
	World world;
	BuildingManager building_manager;


	void Start() {
		world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
		building_manager = GameObject.FindGameObjectWithTag("World").GetComponent<BuildingManager>();
	}


	public List<Coords2D> getPathToCloserBuildingType(BuildingType type, Coords2D depart_position) {
		List<Coords2D> potential_targets = new List<Coords2D> ();
		foreach (WholeBuilding building in building_manager.buildings[type]) {
			foreach (EntranceNode node in building.entrances) {
				potential_targets.Add (Coords2D.getCoords (node.gameObject));
			}
		}
		return getNodesToGoToClosestInList (potential_targets, depart_position);
	}


	private List<Coords2D> getNodesToGoToClosestInList(List<Coords2D> list, Coords2D depart_position) {
		return new List<Coords2D> ();
	}

}
