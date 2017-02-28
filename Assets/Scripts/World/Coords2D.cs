using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
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

	public override int GetHashCode ()
	{
		return this.x + this.y;
	}

	public override bool Equals(object obj) {
		if (obj.GetType() == typeof(Coords2D)) {
			return ((Coords2D)obj).x == this.x && ((Coords2D)obj).y == this.y;
		}
		return false;
	}

	public static bool operator ==(Coords2D c1, Coords2D c2) {
		return c1.Equals (c2);
	}

	public static bool operator !=(Coords2D c1, Coords2D c2) {
		return !(c1.Equals (c2));
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


	// Returns the distance to the origin (0, 0)
	public float norm() {
		return Mathf.Sqrt (this.x * this.x + this.y * this.y);
	}


	// Returns the distance between two coordinates.
	public float distance(Coords2D other_coords) {
		Coords2D diff = this - other_coords;
		return diff.norm ();
	}


	// Returns the direction which corresponds to the coordinates (needs to have a norm of less than 1) 
	public Direction getDirection() {
		if (this.norm () > 1.0) { return Direction.None; }
		if (this.x != 0) { if (this.x > 0) { return Direction.Right; } else { return Direction.Left; } }
		if (this.y != 0) { if (this.y > 0) { return Direction.Up; } else { return Direction.Down; } }
		return Direction.None;
	}


	// Returns a Vector2 object with the Coords2D object coordinates
	public Vector2 toVector2() {
		return new Vector2 (this.x, this.y);
	}


	// Returns the angle of 'other' relative to the Coords2D object, with or without a shift (in degrees)
	public float angle(Coords2D other, float shift = 0f) {
		Coords2D distance_coords = other - this;
		float distance = distance_coords.norm();
		float base_angle_from_cos = Mathf.Acos (distance_coords.x / distance);
		if (Mathf.Asin (distance_coords.y / distance) >= 0) { return correctAngle(base_angle_from_cos, shift); }
		return correctAngle(-base_angle_from_cos, shift);
	}

	// Corrects the positive angle, with the eventual shift, in degrees
	private float correctAngle(float angle, float shift) {
		float angle_value = angle + shift * Mathf.Deg2Rad;
		if (angle_value < 0) {
			angle_value = 2 * Mathf.PI + angle_value;
		}
		return angle_value * Mathf.Rad2Deg;
	}
}
