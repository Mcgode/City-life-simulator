using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	// Here we store the Road and the Building blocks, to acess them easily from coordinates
	Dictionary<int,Dictionary<int,WorldObject>> objects = new Dictionary<int,Dictionary<int,WorldObject>>();

	// Here we store the entrance nodes, the nodes on which the citizen has to stand on in order to enter a given building. 
	List<EntranceNode> entrances = new List<EntranceNode> ();

	// Here are stored the corner nodes, for pathfinding.
	List<Coords2D> nodes = new List<Coords2D> ();


	// Here are the lists containing adjacent coords, to make it easier to store the coordinates.
	List<Coords2D> corners = new List<Coords2D>() { new Coords2D(1, 1), new Coords2D(-1, 1), new Coords2D(-1, -1), new Coords2D(1, -1) };
	List<Coords2D> horizontalAlign = new List<Coords2D>() { new Coords2D(1, 0), new Coords2D(-1, 0) };
	List<Coords2D> verticalAlign = new List<Coords2D>() {new Coords2D(0, 1), new Coords2D(0, -1) };


	// Call this function to register a Road or a Building object, to map their coordinates.
	public void registerObject(WorldObject obj) 
	{
		Coords2D obj_coords = Coords2D.getCoords (obj.gameObject);
		addObj (obj, obj_coords);
	}

	// Call this function to register the entrance nodes.
	public void registerEntrance(EntranceNode node) { entrances.Add (node); } 


	// Private method for adding a WorldObject in the objects dictionary without some "no such key" error.
	private void addObj(WorldObject obj, Coords2D coords)
	{
		if (!objects.ContainsKey(coords.x)) 
		{
			objects.Add (coords.x, new Dictionary<int,WorldObject> ());
		}

		if (!objects [coords.x].ContainsKey (coords.y)) 
		{
			objects [coords.x].Add (coords.y, obj);
		}
	}


	// Maps out the different corners of buildings which correspond to a road. Necessary step for the pathfinding mapping out of the map.
	void mapOutCornersOfWorldMap() 
	{
		// We go through all the roads and the buildings
		foreach (int x in objects.Keys) 
		{
			foreach (int y in objects[x].Keys)
			{
				// We select the buildings
				if (objects[x][y] as Building) 
				{
					// We check if the building block may have some potential corner
					if (buildingPartHasPotentialCorners (new Coords2D (x, y))) 
					{
						foreach (Coords2D coord in corners) 
						{
							// Final checks : do the coordinates correspond to a road, and do they correspond to a real corner, not the building side :
							//	 C R B					R R B							C = Corner
							//	 R B B  : Correct 		R B B  : Not correct   			B = Building
							//   R B B					C B B							R = Road
							Coords2D checking_coords = coord + new Coords2D (x, y);
							if ((get(checking_coords) as Road) && isReallyCorner(new Coords2D (x, y), checking_coords)) 
							{
								nodes.Add (checking_coords);
								// Displays corner, for debug purpose
								GameObject node = pointAt (checking_coords);
								(node.GetComponent (typeof(SpriteRenderer)) as SpriteRenderer).color = (objects[x][y].gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color;
							}
						}
					}
				}
			}
		}
	}



	// Returns the WorldObject at the given coords.
	private WorldObject get(Coords2D coords) {
		if (objects.ContainsKey(coords.x) && objects[coords.x].ContainsKey(coords.y)) {
			return objects [coords.x] [coords.y];
		}
		return null;
	}


	/* Returns whether the building block seems to have at least one corner : 
	   Checked coords are in the middle
		   R R R				R B	R				R R R						Basically, if there is a vertical or an horizontal shape, it does'nt work 		
		   R B B  : Ok			R B R  : Nope		B B B  : Nope
		   R B B				R B R				R R R					*/
	private bool buildingPartHasPotentialCorners (Coords2D coords) 
	{
		bool vertical_ok = (get(coords + verticalAlign[0]) as Building) && (get(coords + verticalAlign[1]) as Building);
		bool horizontal_ok = (get(coords + horizontalAlign[0]) as Building) && (get(coords + horizontalAlign[1]) as Building);
		return !(vertical_ok || horizontal_ok);
	}


	// Start program, classic. We launch our map out from here 
	void Start()
	{
		foreach (Coords2D coord in corners) { print (coord.ToString()); } 
		mapOutCornersOfWorldMap ();
		nodes = new List<Coords2D> (new HashSet<Coords2D>(nodes));
	}


	// Displays a circle node at the given coords, and returns it
	private GameObject pointAt(Coords2D coords) {
		GameObject node_obj = Instantiate (Resources.Load("Node", typeof(GameObject))) as GameObject;
		node_obj.transform.position = new Vector3 (coords.x, coords.y, -1);
		return node_obj;
	}


	// Returns false if the target is not really a corner of the origin bulding block. 
	private bool isReallyCorner(Coords2D origin, Coords2D target) {
		foreach (Coords2D coords in origin.squareTwoPoints(target)) {
			if (get (coords) as Building) {
				return false;
			}
		}
		return true;
	}

}
