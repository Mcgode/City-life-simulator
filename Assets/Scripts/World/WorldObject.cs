using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour {

	// Base awake for all the WorldObject objects, to register to Za Warudo! (for those who know the reference ;) ) 
	public void Awake () {
		World za_warudo = GameObject.FindGameObjectWithTag ("World").GetComponent(typeof(World)) as World;
		za_warudo.registerObject (this);
	}
}
