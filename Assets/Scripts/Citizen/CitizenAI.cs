using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAI : MonoBehaviour 
{
	World world;
	BuildingManager building_manager;
	public Citizen citizen;


	void Start() {
		world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
		building_manager = GameObject.FindGameObjectWithTag("World").GetComponent<BuildingManager>();
		citizen = GetComponent<Citizen> ();
		if (!citizen) { DestroyImmediate (this); }
	}


	public void planNext() {

	}


	public List<Coords2D> getPathToCloserBuildingType(BuildingType type, Coords2D depart_position) {
		List<Coords2D> potential_targets = new List<Coords2D> ();
		foreach (WholeBuilding building in building_manager.buildings[type]) {
			foreach (EntranceNode node in building.entrances) {
				potential_targets.Add (Coords2D.getCoords (node.gameObject));
			}
		}
		return world.pathfindFromCoordinatesMultipleTargets (depart_position, potential_targets);
	}

}
