using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	Dictionary<int,Dictionary<int,WorldObject>> objects = new Dictionary<int,Dictionary<int,WorldObject>>();
	List<EntranceNode> entrances = new List<EntranceNode> ();
	List<Coords2D> nodes = new List<Coords2D> ();

	List<Coords2D> corners = new List<Coords2D>() { new Coords2D(1, 1), new Coords2D(-1, 1), new Coords2D(-1, -1), new Coords2D(1, -1) };
	List<Coords2D> horizontalAlign = new List<Coords2D>() { new Coords2D(1, 0), new Coords2D(-1, 0) };
	List<Coords2D> verticalAlign = new List<Coords2D>() {new Coords2D(0, 1), new Coords2D(0, -1) };


	public void registerObject(WorldObject obj) 
	{
		Coords2D obj_coords = Coords2D.getCoords (obj.gameObject);
		addObj (obj, obj_coords);
	}

	public void registerEntrance(EntranceNode node) { entrances.Add (node); } 


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


	void mapOutCornersOfWorldMap() 
	{
		print ("Mapping out");

		foreach (int x in objects.Keys) 
		{
			foreach (int y in objects[x].Keys)
			{
//				print (new Coords2D(x.Key, y.Key));
				if (objects[x][y] as Building) {
					print ("Is building");
					if (buildingPartHasPotentialCorners (new Coords2D (x, y))) {
						print ("Has potential corners");
						foreach (Coords2D coord in corners) {
							Coords2D checking_coords = coord + new Coords2D (x, y);
							print (get (checking_coords) as Road);
							if ((get(checking_coords) as Road) && isReallyCorner(new Coords2D (x, y), checking_coords)) {
								print ("Is road");
								nodes.Add (checking_coords);
								GameObject node = pointAt (checking_coords);
								(node.GetComponent (typeof(SpriteRenderer)) as SpriteRenderer).color = (objects[x][y].gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color;
							}
						}
					}
				}
			}
		}
	}

							
	private WorldObject get(Coords2D coords) {
		if (objects.ContainsKey(coords.x) && objects[coords.x].ContainsKey(coords.y)) {
			return objects [coords.x] [coords.y];
		}
		return null;
	}


	private bool buildingPartHasPotentialCorners (Coords2D coords) 
	{
		bool vertical_ok = (get(coords + verticalAlign[0]) as Building) && (get(coords + verticalAlign[1]) as Building);
		bool horizontal_ok = (get(coords + horizontalAlign[0]) as Building) && (get(coords + horizontalAlign[1]) as Building);
		return !(vertical_ok || horizontal_ok);
	}


	void Start()
	{
		print ("Hello world");
		print (new Coords2D (1, 1) * new Coords2D (2, 2));
		foreach (Coords2D coord in corners) { print (coord.ToString()); } 
		mapOutCornersOfWorldMap ();
		nodes = new List<Coords2D> (new HashSet<Coords2D>(nodes));
		foreach (Coords2D coord in nodes) { 
			print (coord); 
		}

		foreach (KeyValuePair<int,Dictionary<int,WorldObject>> x in objects) {
			foreach (KeyValuePair<int, WorldObject> y in objects[x.Key]) {
				if (y.Value as Building) {
					Coords2D coords = new Coords2D (x.Key, y.Key);
					if (buildingPartHasPotentialCorners(coords)) {
//						pointAt (coords);
					}
				}
			}
		}
	}


	void Update() 
	{
	}

	private GameObject pointAt(Coords2D coords) {
		GameObject node_obj = Instantiate (Resources.Load("Node", typeof(GameObject))) as GameObject;
		node_obj.transform.position = new Vector3 (coords.x, coords.y, -1);
		return node_obj;
	}


	private bool isReallyCorner(Coords2D origin, Coords2D target) {
		foreach (Coords2D coords in origin.squareTwoPoints(target)) {
			if (get (coords) as Building) {
				return false;
			}
		}
		return true;
	}

}
