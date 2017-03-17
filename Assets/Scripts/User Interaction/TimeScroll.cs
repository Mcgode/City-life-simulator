using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScroll : MonoBehaviour {

	int current_index = 4;
	Scrollbar scrollbar;

	List<float> scales = new List<float> () { 0.000001f, 0.25f, 0.5f, 0.75f, 1f, 1.5f, 2f, 3f, 5f, 10f, 20f };

	void Awake() {
		scrollbar = GetComponent<Scrollbar> ();
	}

	public void ScrollUpdate(float value) {
		int index = Mathf.RoundToInt (value * 10f);
		Time.timeScale = scales [index];
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.O)) {
			current_index = Mathf.Min(10, current_index + 1);
			Time.timeScale = scales [current_index];
			scrollbar.value = (float)current_index / 10f;
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			current_index = Mathf.Max(0, current_index - 1);
			Time.timeScale = scales [current_index];
			scrollbar.value = (float)current_index / 10f;
		}
	}
}
