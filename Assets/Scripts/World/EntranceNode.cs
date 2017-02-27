using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceNode : MonoBehaviour 
{
	public WholeBuilding linkedBuilding;
	public Direction buildingEntranceDirection = Direction.None;


	void Awake() {

		// If no building was linked, links to the parent WholeBuildingComponent.
		if (!linkedBuilding) {
			linkedBuilding = GetComponentInParent<WholeBuilding> ();
		}

		// Register entrance to the building
		linkedBuilding.registerEntrance (this);

		// If no directions were given, determines the first available direction to the building
		if (buildingEntranceDirection == Direction.None) {
			Coords2D currentCoords = Coords2D.getCoords (gameObject);
			foreach (Building buildingBlock in transform.parent.gameObject.GetComponentsInChildren<Building>()) {
				if (currentCoords.distance (Coords2D.getCoords (buildingBlock.gameObject)) == 1.0) {
					buildingEntranceDirection = (Coords2D.getCoords (buildingBlock.gameObject) - currentCoords).getDirection ();
					break;
				}
			}
		}

		World za_warudo = GameObject.FindGameObjectWithTag ("World").GetComponent(typeof(World)) as World;
		za_warudo.registerEntrance (this);
		GetComponent<SpriteRenderer> ().enabled = false;
	}
}
