using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coords2D {


	public int x, y;


	public Coords2D(int x, int y) {
		this.x = x;
		this.y = y;
	}


	public override string ToString ()
	{ return "(" + x + ";" + y + ")";}


	// Operator overload for the struct (+, -, *)
	public static Coords2D operator +(Coords2D c1, Coords2D c2) {
		return new Coords2D (c1.x + c2.x, c1.y + c2.y);
	}

	public static Coords2D operator -(Coords2D c1, Coords2D c2) {
		return new Coords2D (c1.x - c2.x, c1.y - c2.y);
	}
		
	// This is a scalar product, rather than just a product
	public static int operator *(Coords2D c1, Coords2D c2) {
		return c1.x * c2.x +  c1.y * c2.y;
	}


	// Returns the 2D coords of the given game object. 
	public static Coords2D getCoords(GameObject obj) {
		Vector3 pos = obj.transform.position;
		return new Coords2D (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y));
	}


	/* Returns the two other points to make a square : 
			- - T			X - T
			- - -	 --\	- - - 		O is the origin (this), T is the target (other)
			- - -	 --/  	- - -   	Returns X and Y coordinates.
			O - -			O - Y 														*/
	public List<Coords2D> squareTwoPoints(Coords2D other) {
		Coords2D diff = other - this;
		return new List<Coords2D> () {
			this + new Coords2D (diff.x, 0),
			this + new Coords2D (0, diff.y)
		};
	}
}
