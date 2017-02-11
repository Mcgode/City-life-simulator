using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour {

	// Base awake for all the WorldObject objects
	public void Awake () {
		print ("Za Warudo!");
		World world = GameObject.FindGameObjectWithTag ("World").GetComponent(typeof(World)) as World;
		world.registerObject (this);
	}
}
