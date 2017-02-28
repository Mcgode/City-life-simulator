using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenOverseer : MonoBehaviour {

	List<Citizen> citizens = new List<Citizen>();
	World za_warudo;
	BuildingManager building_manager;
	int last_attributed_id = 0;
	public int amount_of_citizen = 10;

	public void registerCitizen(Citizen citizen) {
		citizen.id = last_attributed_id;
		last_attributed_id++;
		citizens.Add (citizen);
	}

	void Start() {
		za_warudo = GameObject.FindObjectOfType<World> ();
		building_manager = GameObject.FindObjectOfType<BuildingManager> ();
		for (int i = 0; i < amount_of_citizen; i++) {
			GameObject new_citizen_object = Instantiate(Resources.Load("Citizen", typeof(GameObject))) as GameObject;
			Citizen new_citizen = new_citizen_object.GetComponent<Citizen> ();
			Coords2D coords = za_warudo.nodes [Mathf.FloorToInt (Random.value * za_warudo.nodes.Count)].coordinates;
			new_citizen.current_coords = coords;
			new_citizen_object.transform.position = (Vector3)(coords.toVector2 ()) + Vector3.back * 1.5f;
			new_citizen_object.transform.SetParent (transform);
			new_citizen.ideal_money = 1000f + Random.value * 5000f;
			while (new_citizen.home == null) {
				int j = Mathf.FloorToInt (Random.value * building_manager.buildings [BuildingType.Housing].Count);
				WholeBuilding building = building_manager.buildings [BuildingType.Housing] [j];
				if (building.capacity > building.inhabitants.Count) {
					building.inhabitants.Add (new_citizen);
					new_citizen.home = building;
				}
			}
			new_citizen.ai.behaviour_list = MakeBehaviourElements.getBehaviours(new_citizen);
			new_citizen.good_to_go = true;
		}
	}

}
