using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// 衝突した相手を削除する
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
	{
		if (other == null || other.gameObject == null) {
            return;
        }

        // 削除用の壁以外は削除する
        if (Tags.DESTROY_ZONE.Equals(other.gameObject.tag)) {
            return;
        }
        Destroy(other.gameObject);
	}
}
