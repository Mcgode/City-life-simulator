using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Just a struct to store node data.
public struct Node 
{
	public Coords2D coordinates;
	public bool isEntrance;
	public WholeBuilding entranceTo;

	public Node(Coords2D coords, WholeBuilding building) {
		coordinates = coords;
		if (building) {
			isEntrance = true;
			entranceTo = building;
		} else {
			isEntrance = false;
			entranceTo = null;
		}
	}


	public static bool isNodeAlreadyInList(List<Node> list, Coords2D coords) {
		foreach (Node node in list) {
			if (node.coordinates == coords) {
				return true;
			}
		}
		return false;
	}

}
