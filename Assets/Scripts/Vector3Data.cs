using UnityEngine;

/// <summary>
/// Vector3がJSONにするときに余計なデータを省いたクラス
/// </summary>
public class Vector3Data
{
    public const int X_INDEX = 0;
    public const int Y_INDEX = 1;
    public const int Z_INDEX = 2;

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

    public static Vector3Data From(Vector2 v2)
    {
        return new Vector3Data(v2.x, v2.y, 0);
    }

    public static Vector3Data From(Vector3 v3)
    {
        return new Vector3Data(v3.x, v3.y, v3.z);
    }

    public Vector3Data Clone()
    {
        return new Vector3Data(this.X, this.Y, this.Z);
    }

    public static Vector3Data operator +(Vector3Data a, Vector3Data b)
    {
        return new Vector3Data(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3Data operator -(Vector3Data a, Vector3Data b)
    {    
        return new Vector3Data(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    override public string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }
}
