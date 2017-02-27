using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInteraction : MonoBehaviour 
{

	GameObject depart_node, arrival_node;
	Color depart_original_color, arrival_original_color;

	Color depart_color = new Color(0.5f, 0.78f, 1f), arrival_color = new Color(1f, 0.85f, 0.24f);

	World za_warudo;
	GameObject line_parent;


	void Start() {
		za_warudo = GameObject.Find ("World").GetComponent<World> ();
		line_parent = new GameObject ();
		line_parent.transform.SetParent (transform);
	}


	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown ("Fire1")) { Fire1Raycast (); updateLinks(); }

		if (Input.GetButtonDown ("Fire2")) { Fire2Raycast (); updateLinks(); }

	}


	void Fire1Raycast() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider && hit.collider.transform.parent.name == "Node Display") {
			if (depart_node) { depart_node.GetComponent<SpriteRenderer> ().color = depart_original_color; }
			depart_node = hit.collider.gameObject;
			depart_original_color = depart_node.GetComponent<SpriteRenderer> ().color;
			depart_node.GetComponent<SpriteRenderer> ().color = depart_color;
		} else {
			if (depart_node) { depart_node.GetComponent<SpriteRenderer> ().color = depart_original_color; }
			depart_node = null;
			depart_original_color = new Color();
		}
	}


	void Fire2Raycast() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider && hit.collider.transform.parent.name == "Node Display") {
			if (arrival_node) { arrival_node.GetComponent<SpriteRenderer> ().color = arrival_original_color; }
			arrival_node = hit.collider.gameObject;
			arrival_original_color = arrival_node.GetComponent<SpriteRenderer> ().color;
			arrival_node.GetComponent<SpriteRenderer> ().color = arrival_color;
		} else {
			if (arrival_node) { arrival_node.GetComponent<SpriteRenderer> ().color = arrival_original_color; }
			arrival_node = null;
			arrival_original_color = new Color();
		}
	}


	void updateLinks() 
	{
		while (line_parent.transform.childCount > 0) {
			DestroyImmediate (line_parent.transform.GetChild (0).gameObject);
		}

		if (depart_node && arrival_node) {
			List<Coords2D> coords = za_warudo.pathfindFromCoordinates (Coords2D.getCoords (depart_node), Coords2D.getCoords (arrival_node));

			for (int i = 0; i < coords.Count - 1; i++) {
				Coords2D c1 = coords [i], c2 = coords [i + 1];
				GameObject line_obj = Instantiate (Resources.Load("Line", typeof(GameObject))) as GameObject;
				line_obj.transform.SetParent (line_parent.transform);
				line_obj.name = "Line from " + c1.ToString () + " to " + c2.ToString ();
				line_obj.GetComponent<SpriteRenderer> ().color = Color.red;
				line_obj.transform.position = new Vector3 (((float)(c1.x + c2.x)) / 2f, ((float)(c1.y + c2.y)) / 2f, -0.6f);
				line_obj.transform.localScale = new Vector3(0.1f, c1.distance (c2), 1.0f);
				line_obj.transform.rotation = Quaternion.Euler ( new Vector3(0f, 0f, c1.angle(c2, -90f) ));
			}
		}
	}
}
