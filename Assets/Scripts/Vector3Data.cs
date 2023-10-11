using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vector3がJSONにするときに余計なデータを省いたクラス
/// </summary>
public class Vector3Data
{
    public float X {get; set;}
    public float Y {get; set;}
    public float Z {get; set;}

    public Vector3Data()
    {
    }

    public Vector3Data(float x, float y, float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector3 toVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public static Vector3Data From(Vector2 v2) {
        return new Vector3Data(v2.x, v2.y, 0);
    }
}
