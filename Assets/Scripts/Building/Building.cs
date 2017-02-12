using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : WorldObject 
{
	new void Awake() {
		base.Awake ();
		gameObject.AddComponent<BoxCollider2D> ();
	}
}
