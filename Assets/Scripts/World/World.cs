using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	Dictionary<int,Dictionary<int,WorldObject>> objects = new Dictionary<int,Dictionary<int,WorldObject>>();
	List<EntranceNode> entrances = new List<EntranceNode> ();


	public void registerObject(WorldObject obj) 
	{
		Coords2D obj_coords = Coords2D.getCoords (obj.gameObject);
		addObj (obj, obj_coords);
		if (obj is EntranceNode) {
			entrances.Add (obj as EntranceNode);
		}
	}


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


	void Start()
	{
		print("Hello world");
		print(new Coords2D(1,1) * new Coords2D(2,2));
	}


	void Update() 
	{
	}


}
