using System.Collections;
using UnityEngine;

public class PerspectiveTeleporter : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform teleporterReceiver;

    private bool playerIsOverlapping = false;

    public Transform TeleporterReceiver
    {
        get { return teleporterReceiver; }
        set { teleporterReceiver = value; }
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if(playerIsOverlapping)
        {
            Vector3 portalToPlayer = playerTransform.position - transform.position;
            float dot = Vector3.Dot(portalToPlayer, transform.forward);

            //The player has moved across the portal
            if (dot < 0f && teleporterReceiver != null)
            {
                //Teleport Stuff
                float rotationDiff = -Quaternion.Angle(transform.rotation, teleporterReceiver.rotation);
                rotationDiff += 180;
                playerTransform.Rotate(Vector3.up, rotationDiff);

                Vector3 posOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                playerTransform.position = teleporterReceiver.position + posOffset;
                playerIsOverlapping = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            playerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerIsOverlapping = false;
    }
}
