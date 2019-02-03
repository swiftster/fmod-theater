using UnityEngine;

public static class Ext
{
	public static Vector3 Change(this Vector3 org, object x = null, object y = null, object z = null)
	{
		float newX;
		float newY;
		float newZ;
 
		if (x == null)
			newX = org.x;
		else
			newX = org.x + (float)x;
 
		if (y == null)
			newY = org.y;
		else
			newY = org.y + (float)y;
 
		if (z == null)
			newZ = org.z;
		else
			newZ = org.z + (float)z;
 
		return new Vector3(newX, newY, newZ);
	}
}