using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct IntVector2 //this is a vector structure that is used to store maze coordinates
{
    public int x, z;
    public IntVector2 (int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.z += b.z;
        return a;
    }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return !(a == b);
    }
}

