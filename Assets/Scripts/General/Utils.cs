using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour 
{

	public static string behaviourPathToString(List<CitizenBehaviourElement> path) {
		string txt = "";
		for (int i = 0; i < path.Count; i++) {
			if (i > 0) { txt += " -> "; }
			txt += path[i].description;
		}
		return txt;
	}

}
