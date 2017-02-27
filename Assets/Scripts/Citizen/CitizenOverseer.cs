using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenOverseer : MonoBehaviour {

	List<Citizen> citizens = new List<Citizen>();
	int last_attributed_id = -1;

	public void registerCitizen(Citizen citizen) {
		citizen.id = last_attributed_id++;
		citizens.Add (citizen);
	}

}
