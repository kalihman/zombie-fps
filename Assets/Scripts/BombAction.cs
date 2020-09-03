using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;
    // 충돌했을때 처리
    private void OnCollisionEnter(Collision collision)
    {
        // 이펙트 프리팹 생성
        GameObject eff = Instantiate(bombEffect);

        // 이펙트 프리팹의 위치는 수류탄 오브젝트 자신의 위치와 동일.
        eff.transform.position = transform.position;
        
        Destroy(gameObject);
    }
}
