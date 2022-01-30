using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    [SerializeField] private float pickUpRange = 1f;
    [SerializeField] private float moveForce = 100f;
    [SerializeField] private float dropForceMultiplier = 4f;
    [SerializeField] private Transform holderObj;
    [SerializeField] private InputActionReference pickObjControl;

    private GameObject pickObject;
    private int pickableMask, pickAnimation, dropAnimation;
    private bool pickUpAnimationStart = false, pickUpAnimationFinish = false, canPickObject = false, dropAnimationStart = false, dropAnimationFinish = false;

    private void OnEnable()
    {
        pickObjControl.action.Enable();
    }

    private void OnDisable()
    {
        pickObjControl.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        pickableMask = 1 << 3;
        pickAnimation = Animator.StringToHash("Pick Up");
        dropAnimation = Animator.StringToHash("Drop");
    }

    // Update is called once per frame
    void Update()
    {
        if(pickObjControl.action.triggered)
        {
            if (pickObject == null)
            {
                RaycastHit hit;
                Vector3 vec = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
                if (Physics.Raycast(vec, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, pickableMask))
                {
                    transform.GetComponent<PlayerController>().ActiveControls = false;

                    if(hit.transform.GetComponent<Rigidbody>())
                    {
                        pickObject = hit.transform.gameObject;
                    }
                    
                    transform.GetComponent<PlayerController>().ChangeAnimationCrossfade(pickAnimation, 0.075f);
                }
            }
            else
            {
                transform.GetComponent<PlayerController>().ActiveControls = false;
                transform.GetComponent<PlayerController>().ChangeAnimationCrossfade(dropAnimation, 0.075f);
            }
            
        }

        if(pickUpAnimationStart && pickObject != null)
        {
            PickUpObject(pickObject);
            canPickObject = true;
        }

        if (dropAnimationStart && pickObject != null)
        {
            DropObject();
        }

        if (pickUpAnimationFinish)
        {
            transform.GetComponent<PlayerController>().ActiveControls = true;
            pickUpAnimationFinish = false;
        }

        if (dropAnimationFinish)
        {
            transform.GetComponent<PlayerController>().ActiveControls = true;
            dropAnimationFinish = false;
        }

        if (canPickObject)
        {
            MoveObject();
        }
    }

    void MoveObject()
    {
        if(Vector3.Distance(holderObj.position, pickObject.transform.position) > 0.1f)
        {
            Vector3 moveDir = holderObj.position - pickObject.transform.position;
            pickObject.GetComponent<Rigidbody>().AddForce(moveDir * moveForce);
        }
    }

    void PickUpObject( GameObject _pickableObject)
    {
        if (_pickableObject.GetComponent<Rigidbody>())
        {
            Rigidbody objRb = _pickableObject.GetComponent<Rigidbody>();
            objRb.useGravity = false;
            objRb.drag = 10;
            _pickableObject.transform.parent = holderObj;
        }
    }

    void DropObject()
    {
        canPickObject = false;
        Rigidbody objRb = pickObject.GetComponent<Rigidbody>();
        objRb.useGravity = true;
        objRb.drag = 0;
        objRb.AddForce(transform.forward * (moveForce * dropForceMultiplier));
        pickObject.transform.parent = null;
        pickObject = null;
    }

    public void PickUpAnimationStartFlag()
    {
        pickUpAnimationStart = true;
        pickUpAnimationFinish = false;
    }

    public void PickUpAnimationEndFlag()
    {
        pickUpAnimationStart = false;
        pickUpAnimationFinish = true;
    }

    public void DropAnimationStartFlag()
    {
        dropAnimationStart = true;
        dropAnimationFinish = false;
    }

    public void DropAnimationEndFlag()
    {
        dropAnimationStart = false;
        dropAnimationFinish = true;
    }
}
