using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTools : MonoBehaviour
{
	// Implementation of the Dijkstra algorithm to get the closed node from a list to a given depart point
	public static List<int> getShortestDistanceToNodeList(List<List<KeyValuePair<int, float>>> node_links, int depart_index, List<int> target_nodes) {

		if (target_nodes.Contains (depart_index)) {
			return new List<int> () { depart_index };
		}

		// Status: 2 = Node reached, 1 = Neighbour, 0 = Not in the vincinity
		List<int> node_status = new List<int>();

		List<int> neighbours = new List<int> (), reached_nodes = new List<int> () { depart_index };
		Dictionary<int, float> total_distance = new Dictionary<int, float>();
		Dictionary<int, List<int>> path = new Dictionary<int, List<int>> ();

		path.Add (depart_index, new List<int>() { depart_index });

		for (int i = 0; i < node_links.Count; i++) { 
			node_status.Add (0);
		}
		node_status [depart_index] = 2;

		foreach (KeyValuePair<int, float> kvp in node_links[depart_index]) {
			neighbours.Add (kvp.Key);
			node_status [kvp.Key] = 1;
			total_distance.Add (kvp.Key, kvp.Value);
			path.Add (kvp.Key, new List<int> () { depart_index, kvp.Key });
		}

		while (neighbours.Count > 0) {

			int best_neighbour = neighbours [0];
			float shortest_distance = total_distance [neighbours [0]];

			// Determine which neighbour is the closest
			foreach (int neighbour in neighbours) {
				if (shortest_distance > total_distance [neighbour]) {
					shortest_distance = total_distance [neighbour];
					best_neighbour = neighbour;
				}
			}

			// Change the status of this best neighbour from neighbour to reached node
			reached_nodes.Add (best_neighbour);
			neighbours.Remove (best_neighbour);
			node_status [best_neighbour] = 2;

			// Check for new neighbours or new closest distance to start point thanks to the newly reached node
			foreach (KeyValuePair<int, float> link in node_links[best_neighbour]) {
				if (node_status[link.Key] < 2) { 
					if (node_status[link.Key] == 1) {
						if (total_distance [link.Key] > shortest_distance + link.Value) {
							total_distance [link.Key] = shortest_distance + link.Value;
							path [link.Key] = path [best_neighbour].GetRange (0, path [best_neighbour].Count);
							path [link.Key].Add (link.Key);
						}
					} else {
						neighbours.Add (link.Key);
						node_status [link.Key] = 1;
						total_distance.Add(link.Key, link.Value + shortest_distance);
						path.Add(link.Key, path [best_neighbour].GetRange (0, path [best_neighbour].Count));
						path [link.Key].Add (link.Key);
					}
				}
			}

			if (target_nodes.Contains (best_neighbour)) {
				return path [best_neighbour];
			}

		}

		return new List<int>();
	}
}
