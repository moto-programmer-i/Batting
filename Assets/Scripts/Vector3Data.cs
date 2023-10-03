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
}
