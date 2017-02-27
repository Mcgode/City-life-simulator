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

		List<int> neighbours = new List<int>(), reached_nodes = new List<int>() { depart_index };
		Dictionary<int, float> total_distance = new Dictionary<int, float>();
		Dictionary<int, List<int>> path = new Dictionary<int, List<int>> ();

		path.Add (depart_index, new List<int>() { depart_index });

		foreach (KeyValuePair<int, float> kvp in node_links[depart_index]) {
			neighbours.Add (kvp.Key);
			total_distance.Add (kvp.Key, kvp.Value);
			path.Add (kvp.Key, new List<int> () { depart_index, kvp.Key });
		}

		while (neighbours.Count > 0) {

			string neighbours_string = "Neighbours: [";
			foreach (int neighbour in neighbours) { neighbours_string += neighbour.ToString() + ", "; }
			print (neighbours_string + "]");

			string reached_string = "Reached Nodes: [";
			foreach (int reached_node in reached_nodes) { reached_string += reached_node.ToString() + ", "; }
			print (reached_string + "]");

			int best_neighbour = neighbours [0];
			float shortest_distance = total_distance [neighbours [0]];

			foreach (int neighbour in neighbours) {
				if (shortest_distance > total_distance [neighbour]) {
					shortest_distance = total_distance [neighbour];
					best_neighbour = neighbour;
				}
			}

			print ("Best neighbour: " + best_neighbour.ToString());

			reached_nodes.Add (best_neighbour);
			neighbours.Remove (best_neighbour);
			foreach (KeyValuePair<int, float> link in node_links[best_neighbour]) {
				print (link.Key);
				if (!reached_nodes.Contains (link.Key)) {
					if (neighbours.Contains (link.Key)) {
						if (total_distance [link.Key] > shortest_distance + link.Value) {
							total_distance [link.Key] = shortest_distance;
							path [link.Key] = path [best_neighbour].GetRange (0, path [best_neighbour].Count);
							path [link.Key].Add (link.Key);
						}
					} else {
						neighbours.Add (link.Key);
						total_distance.Add(link.Key, link.Value + shortest_distance);
						path.Add(link.Key, path [best_neighbour].GetRange (0, path [best_neighbour].Count));
						path [link.Key].Add (link.Key);
					}
				}
			}
			if (target_nodes.Contains (best_neighbour)) {
				neighbours_string = "[";
				foreach (int neighbour in path[best_neighbour]) {
					neighbours_string += neighbour.ToString () + ", ";
				}
				print (neighbours_string + "]");
				return path [best_neighbour];
			}

		}

		return new List<int>();
	}
}
