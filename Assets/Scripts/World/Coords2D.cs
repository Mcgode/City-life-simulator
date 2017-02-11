using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coords2D {

	public int x, y;

	public Coords2D(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static Coords2D operator +(Coords2D c1, Coords2D c2) {
		return new Coords2D (c1.x + c2.x, c1.y + c2.y);
	}

	public static Coords2D operator -(Coords2D c) {
		return new Coords2D (-c.x, -c.y);
	}

	public static int operator *(Coords2D c1, Coords2D c2) {
		return c1.x * c2.x +  c1.y * c2.y;
	}

}
