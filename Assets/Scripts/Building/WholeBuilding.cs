using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeBuilding : MonoBehaviour 
{
	// Stores the type of building (bar, housing,...)
	public BuildingType type; 
	public List<EntranceNode> entrances = new List<EntranceNode> ();
	public int capacity_per_block = 2;
	public int capacity = 0;
	public List<Citizen> inhabitants;
	public List<Job> employees;
	public float default_pay = 100f;

	public void registerEntrance(EntranceNode entrance) {
		entrances.Add (entrance);
	}


	private void Awake() {
		GameObject.FindGameObjectWithTag ("World").GetComponent<BuildingManager> ().registerWholeBuilding (this);
		if (type == BuildingType.Housing) {
			inhabitants = new List<Citizen> ();
		}
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			if (child.GetComponent<BoxCollider2D>()) {
				capacity += capacity_per_block;
			}
		}
	}
}
