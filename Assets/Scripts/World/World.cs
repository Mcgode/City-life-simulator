using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	List<Building> buildings;
	List<Node> entrances;

	void registerObject(Object obj, ObjectType type) {

		switch (type) 
		{
		case ObjectType.Building:
			if (obj as Building) { buildings.Add(obj as Building); }
			break;
		case ObjectType.Entrance:
			if (obj as Node) { 
				entrances.Add(obj as Node); 
			}
			break;
		}
	}

	void Start() {
		print("Hello world");
		print(new Coords2D(1,1) * new Coords2D(2,2));
	}


}
