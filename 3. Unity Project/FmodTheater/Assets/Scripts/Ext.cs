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
            newX = org.x + (float) x;

        if (y == null)
            newY = org.y;
        else
            newY = org.y + (float) y;

        if (z == null)
            newZ = org.z;
        else
            newZ = org.z + (float) z;

        return new Vector3(newX, newY, newZ);
    }

    // Vector2
    public static Vector2 SetX(this Vector2 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }

    public static Vector2 SetY(this Vector2 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    // Vector3
    public static Vector3 SetX(this Vector3 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }

    public static Vector3 SetY(this Vector3 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    public static Vector3 SetZ(this Vector3 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }

    // Vector4
    public static Vector4 SetX(this Vector4 aVec, float aXValue)
    {
        aVec.x = aXValue;
        return aVec;
    }

    public static Vector4 SetY(this Vector4 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    public static Vector4 SetZ(this Vector4 aVec, float aZValue)
    {
        aVec.z = aZValue;
        return aVec;
    }

    public static Vector4 SetW(this Vector4 aVec, float aWValue)
    {
        aVec.w = aWValue;
        return aVec;
    }
}