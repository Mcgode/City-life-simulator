using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour 
{
	public Dictionary<BuildingType, List<WholeBuilding>> buildings;

	public void registerWholeBuilding(WholeBuilding building) {
		if (!buildings.ContainsKey (building.type)) {
			buildings.Add (building.type, new List<WholeBuilding> ());
		}
		buildings [building.type].Add (building);
	}
}
