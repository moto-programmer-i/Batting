using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="point">スクリーン座標</param>
    /// <param name="distance">カメラからの距離</param>
    /// <returns></returns>
    public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 point, float distance) 
    {
        return camera.ScreenToWorldPoint(
            new Vector3(point.x, point.y, distance)
        );
    }
}
