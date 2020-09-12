using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.forward = target.forward;
    }
}
