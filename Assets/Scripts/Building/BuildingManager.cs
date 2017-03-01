using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour 
{
	public Dictionary<BuildingType, List<WholeBuilding>> buildings = new Dictionary<BuildingType, List<WholeBuilding>> ();
	List<WholeBuilding> workplaces = new List<WholeBuilding>();

	List<BuildingType> work_buildings = new List<BuildingType>() {
		BuildingType.Office,
		BuildingType.FireDepartement,
		BuildingType.PoliceDepartement,
		BuildingType.SuperMarket,
		BuildingType.Bar,
		BuildingType.LeisureShop,
		BuildingType.Hospital
	};

	public void registerWholeBuilding(WholeBuilding building) {
		if (!buildings.ContainsKey (building.type)) {
			buildings.Add (building.type, new List<WholeBuilding> ());
		}
		buildings [building.type].Add (building);
		workplaces = new List<WholeBuilding> ();
		foreach (BuildingType type in work_buildings) {
			if (buildings.ContainsKey (type)) {
				foreach (WholeBuilding workplace in buildings[type]) {
					workplaces.Add (workplace);
				}
			}
		}
	}


	public WholeBuilding pickAWorkplace() {
		return workplaces[Mathf.FloorToInt(Random.value * workplaces.Count)];
	}
}
