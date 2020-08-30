using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    // 충돌했을때 처리
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
