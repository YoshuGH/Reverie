using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    

    public float objMass = 300;
    public float pushAtMass = 100;
    public float pushingTime;
    public float forceToPush;
    Rigidbody rb;
    public float vel;
    Vector3 dir;

    private bool isInRange = false;
    Vector3 lastPos;
    float pushedTime = 0;

    private void OnEnable()
    {
        //pickObjControl.action.Enable();
    }

    private void OnDisable()
    {
        //pickObjControl.action.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) return;
        rb.mass = objMass;
    }

    bool IsMoving()
    {
        if (rb.velocity.magnitude > 0.06f)
        {
            return true;
        }
        return false;

    }

    private void Update()
    {
        /*+
        vel = rb.velocity.magnitude;
        if (pickObjControl.action.triggered && isInRange)
        {
            rb.isKinematic = false;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
        }
        
        if (rb.isKinematic == false)
        {
            pushedTime += Time.deltaTime;
            if (pushedTime >= pushingTime)
            {
                pushedTime = pushingTime;
            }

            print(Mathf.Lerp(objMass, pushAtMass, Time.deltaTime));
            rb.mass = Mathf.Lerp(objMass, pushAtMass, Time.deltaTime);
            rb.AddForce(dir * forceToPush, ForceMode.Force);
        }
        else
        {
            rb.mass = objMass;
            pushingTime = 0;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInRange = true;
            dir = other.transform.position - transform.position;
        }      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isInRange = false;
    }
}