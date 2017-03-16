using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job 
{

	public WholeBuilding employer;
	public Citizen employee;
	public float pay_per_session;
	public int number_of_sessions;

	public Job(WholeBuilding workplace, Citizen employee) {
		employer = workplace;
		this.employee = employee;
		pay_per_session = workplace.default_pay;
		number_of_sessions = 0;
	}

}
