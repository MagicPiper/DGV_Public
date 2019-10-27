using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainTesterTerrain : MonoBehaviour
{
    public int spitOuts;
    private Collider lastCollider;
   

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != lastCollider)
        {
            ++spitOuts;
            lastCollider = collision.collider;
            Debug.Log("spitOut!");
        }
    }
}