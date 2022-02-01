using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.transform.GetComponent<PlayerController>())
            {
                other.transform.GetComponent<PlayerController>().CurrentTransform = transform;
            }
        }
    }
}
