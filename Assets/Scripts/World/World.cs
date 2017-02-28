using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	// Here we store the Road and the Building blocks, to acess them easily from coordinates
	Dictionary<int,Dictionary<int,WorldObject>> objects = new Dictionary<int,Dictionary<int,WorldObject>>();

	// Here we store the entrance nodes, the nodes on which the citizen has to stand on in order to enter a given building. 
	public List<EntranceNode> entrances = new List<EntranceNode> ();

	// Here are stored the corner nodes, for pathfinding.
	List<Coords2D> cornerNodes = new List<Coords2D> ();


	// Here are the lists containing adjacent coords, to make it easier to store the coordinates.
	List<Coords2D> corners = new List<Coords2D>() { new Coords2D(1, 1), new Coords2D(-1, 1), new Coords2D(-1, -1), new Coords2D(1, -1) };
	List<Coords2D> horizontalAlign = new List<Coords2D>() { new Coords2D(1, 0), new Coords2D(-1, 0) };
	List<Coords2D> verticalAlign = new List<Coords2D>() {new Coords2D(0, 1), new Coords2D(0, -1) };


	// Here we store all of our nodes as Node objects, and the links between them.
	public List<Node> nodes = new List<Node> ();
	List<List<KeyValuePair<int, float>>> links = new List<List<KeyValuePair<int, float>>> ();


	// Call this function to register a Road or a Building object, to map their coordinates.
	public void registerObject(WorldObject obj) 
	{
		Coords2D obj_coords = Coords2D.getCoords (obj.gameObject);
		addObj (obj, obj_coords);
	}


	// Node display parent
	GameObject node_display_parent;


	// Call this function to register the entrance nodes. The entrance needs to have a direction and a linked building
	public void registerEntrance(EntranceNode node) { 
		if (node.buildingEntranceDirection != Direction.None && node.linkedBuilding) {
			entrances.Add (node); 
		}
	} 


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
	private void mapOutCornersOfWorldMap() 
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
								cornerNodes.Add (checking_coords);
								// Displays corner, for debug purpose
								pointAt (checking_coords);
//								node.GetComponent<SpriteRenderer>().color = objects[x][y].gameObject.GetComponent<SpriteRenderer>().color;
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
		   R B B				R B R				R R R																									*/
	private bool buildingPartHasPotentialCorners (Coords2D coords) 
	{
		bool vertical_ok = (get(coords + verticalAlign[0]) as Building) && (get(coords + verticalAlign[1]) as Building);
		bool horizontal_ok = (get(coords + horizontalAlign[0]) as Building) && (get(coords + horizontalAlign[1]) as Building);
		return !(vertical_ok || horizontal_ok);
	}


	// Start program, classic. We launch the 'map out' process from here 
	private void Start()
	{
		if (!node_display_parent) { 
			node_display_parent = new GameObject();
			node_display_parent.transform.SetParent (gameObject.transform);
			node_display_parent.name = "Node Display";
			node_display_parent.AddComponent<NodeInteraction> ();
		}
		node_display_parent.SetActive (false);
		mapOutCornersOfWorldMap ();
		cornerNodes = new List<Coords2D> (new HashSet<Coords2D>(cornerNodes));
		figureOutNodeMapping ();
		for (int i = 0; i < node_display_parent.transform.childCount; i++) {
			GameObject child = node_display_parent.transform.GetChild (i).gameObject;
			if (child.name.Contains("Node")) {
				string name = child.name;
				child.name = "No name";
				while (node_display_parent.transform.FindChild(name)) {
					DestroyImmediate (node_display_parent.transform.FindChild (name).gameObject);
				}
				child.name = name;
			}
		}
		for (int i = 0; i < nodes.Count; i++) {
			string node_string = i.ToString() + " : " + nodes[i].coordinates.ToString() + " [";
			foreach (KeyValuePair<int, float> link in links[i]) {
				node_string += link.Key.ToString() + " (" + link.Value.ToString() + "), ";
			}
			print (node_string + "]");
		}
	}


	// Displays a circle node at the given coords, and returns it
	private GameObject pointAt(Coords2D coords, float offset = -1.0f) {
		GameObject node_obj = Instantiate (Resources.Load("Node", typeof(GameObject))) as GameObject;
		node_obj.transform.SetParent (node_display_parent.transform);
		node_obj.name = "Node at " + coords.ToString ();
		node_obj.transform.position = new Vector3 (coords.x, coords.y, offset);
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


	// Declares the nodes and determines the links between them (corner nodes and entrance nodes).
	private void figureOutNodeMapping() {
		foreach (EntranceNode entranceNode in entrances) {
			Node new_node = new Node(Coords2D.getCoords(entranceNode.gameObject), entranceNode.linkedBuilding);
			makeLinks (new_node);
			GameObject node = pointAt (new_node.coordinates, -1.2f);
			node.GetComponent<SpriteRenderer>().color = new Color(1, 0.7f, 0.7f);
		}
		foreach (Coords2D corner_node in cornerNodes) {
			if (!Node.isNodeAlreadyInList (nodes, corner_node)) {
				Node new_node = new Node(corner_node, null);
				makeLinks (new_node);
			}
		}

		for (int i=0; i < links.Count; i++) {
			foreach (KeyValuePair<int, float> link in links[i]) {
				//pointAt (nodes[i].coordinates);
				displayLine (nodes [link.Key].coordinates, nodes [i].coordinates);
			}
		}
	}


	// Calculates the possible links of new node with the other nodes. 
	private void makeLinks(Node new_node) {
		List<KeyValuePair<int,float>> new_links = new List<KeyValuePair<int, float>> ();
		for (int i = 0; i < nodes.Count; i++) {
			Node node = nodes [i];
			Coords2D c1 = new_node.coordinates, c2 = node.coordinates;
			if (raycast(c1, c2)) {
				new_links.Add (new KeyValuePair<int,float> (i, new_node.coordinates.distance (node.coordinates)));
				links [i].Add (new KeyValuePair<int,float> (nodes.Count, new_node.coordinates.distance (node.coordinates)));
			}
		}
		links.Add (new_links);
		nodes.Add (new_node);
	}


	// Returns true if there are no buildings on the direct line between two points.
	private bool raycast(Coords2D depart, Coords2D arrival) {
		Coords2D diff = arrival - depart;
		int a = diff.x, b = diff.y;
		if (a == 0 || b == 0) {
			RaycastHit2D hit = Physics2D.Raycast (depart.toVector2(), diff.toVector2().normalized, depart.distance(arrival));
			return !hit;
		} else {
			int a_sign = a / Mathf.Abs (a), b_sign = b / Mathf.Abs (b);
			Vector2 depart_1 = depart.toVector2 () + new Vector2 (.49f * -a_sign, .49f * b_sign);
			RaycastHit2D hit_1 = Physics2D.Raycast (depart_1, diff.toVector2().normalized, depart.distance(arrival)); 
			Vector2 depart_2 = depart.toVector2 () + new Vector2 (.49f * a_sign, .49f * -b_sign);
			RaycastHit2D hit_2 = Physics2D.Raycast (depart_2, diff.toVector2().normalized, depart.distance(arrival));
			return !hit_1 && !hit_2;
		}
	}


	// Displays a line between c1 and c2.
	private void displayLine(Coords2D c1, Coords2D c2) {
		GameObject line_obj = Instantiate (Resources.Load("Line", typeof(GameObject))) as GameObject;
		line_obj.transform.SetParent (node_display_parent.transform);
		line_obj.name = "Line from " + c1.ToString () + " to " + c2.ToString ();
		line_obj.transform.position = new Vector3 (((float)(c1.x + c2.x)) / 2f, ((float)(c1.y + c2.y)) / 2f, -0.5f);
		line_obj.transform.localScale = new Vector3(0.1f, c1.distance (c2), 1.0f);
		line_obj.transform.rotation = Quaternion.Euler ( new Vector3(0f, 0f, c1.angle(c2, -90f) )); 
	}


	// Returns shortest path from c1 to c2
	public List<Coords2D> pathfindFromCoordinates(Coords2D c1, Coords2D c2) {
		int i1 = 0, i2 = 0;
		for (int i = 0; i < nodes.Count; i++) {
			if (nodes [i].coordinates == c1) { i1 = i; }
			if (nodes [i].coordinates == c2) { i2 = i; }
		}
		List<Coords2D> coords = new List<Coords2D>();
		foreach (int index in pathfindFromIndexes (i1, new List<int>() { i2 } ).Value) {
			coords.Add (nodes [index].coordinates);
		}
		return coords;
	}


	// Returns shortest path from depart to nearest target in targets list
	public KeyValuePair<float, List<Coords2D>> pathfindFromCoordinatesMultipleTargets(Coords2D depart, List<Coords2D> targets) {
		int depart_index = 0;
		List<int> target_indexes = new List<int>();
		for (int i = 0; i < nodes.Count; i++) {
			if (nodes [i].coordinates == depart) { depart_index = i; }
			if (targets.Contains(nodes [i].coordinates)) { target_indexes.Add(i); }
		}
		List<Coords2D> coords = new List<Coords2D>();
		KeyValuePair<float, List<int>> result = pathfindFromIndexes (depart_index, target_indexes);
		foreach (int index in result.Value) {
			coords.Add (nodes [index].coordinates);
		}
		return new KeyValuePair<float, List<Coords2D>>(result.Key, coords);
	}


	// Returns index path from index info
	private KeyValuePair<float, List<int>> pathfindFromIndexes(int depart, List<int> arrival) {
		return GraphTools.getShortestDistanceToNodeListKVP (links, depart, arrival);
	}

}
