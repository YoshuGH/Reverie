using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RespawnSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.GetComponent<PlayerController>())
            {
                other.transform.position = other.transform.GetComponent<PlayerController>().CurrentTransform.position;
            }
        }
    }
}
