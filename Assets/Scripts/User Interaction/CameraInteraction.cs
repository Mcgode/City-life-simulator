using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInteraction : MonoBehaviour {

	public float max_zoom, min_zoom, scroll_speed, move_speed;


	// Update is called once per frame
	void Update () {
		float x_input = Input.GetAxisRaw("Horizontal"), y_input = Input.GetAxisRaw("Vertical");
		if (x_input != 0f || y_input != 0f) {
			Vector3 movement = new Vector3 (x_input, y_input, 0f).normalized * move_speed * Time.deltaTime;
			transform.position = transform.position + movement;
		}

		float scroll_input = Input.GetAxisRaw ("Mouse ScrollWheel");
		if (scroll_input != 0f) {
			Camera camera = GetComponent<Camera>();
			camera.orthographicSize = Mathf.Min (max_zoom, Mathf.Max (min_zoom, camera.orthographicSize + scroll_input * scroll_speed * Time.deltaTime));
		}
	}
}
