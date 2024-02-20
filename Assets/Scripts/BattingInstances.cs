using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattingInstances : MonoBehaviour
{
    private static DistanceManager DISTANCE_MANAGER;

    /// <summary>
    /// 静的インスタンス（この方が使う際は楽なため）
    /// </summary>
    [SerializeField]
    private DistanceManager distanceManager;

    
    
    // Start is called before the first frame update
    void Start()
    {
        DISTANCE_MANAGER = distanceManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 距離の管理クラスのインスタンスを返す（初期化前だとNULL）
    /// </summary>
    /// <returns></returns>
    public static DistanceManager GetDistanceManager() {
        return DISTANCE_MANAGER;
    }
}
