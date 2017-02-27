using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour 
{
	CitizenAI ai;


	void Start() {
		ai = GetComponent<CitizenAI> ();
		if (!ai) {
			gameObject.AddComponent<CitizenAI> ();
			ai = GetComponent<CitizenAI> ();
		}
	}

}
