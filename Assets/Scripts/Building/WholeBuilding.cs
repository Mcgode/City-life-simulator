﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeBuilding : MonoBehaviour 
{
	// Stores the type of building (bar, housing,...)
	public BuildingType type; 
	public List<EntranceNode> entrances = new List<EntranceNode>();


	public void registerEntrance(EntranceNode entrance) {
		entrances.Add (entrance);
	}
}
