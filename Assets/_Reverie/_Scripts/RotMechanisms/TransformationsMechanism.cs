using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class TransformationsMechanism : MonoBehaviour
{
    [SerializeField] private TransformationType transformationType;
    [SerializeField] private MovingObjects[] movingObjects;
    [SerializeField, Header("Settings")] 
    private float activationDistance = 1.15f;
    [SerializeField, Range(0f,1f)] private float rotationSpeed = 0.2f;
    [SerializeField, Header("(Translation Only)")] 
    private float translationSpeed;
    [SerializeField] private int translationDistance;
    [SerializeField] private float translationOffset = 0.5f;
    [SerializeField, Header("Setup")]
    private CinemachineTargetGroup targetGroup;
    [SerializeField] private InputActionReference objInteractionControl;
    [SerializeField] private InputActionReference movementControl;
    [SerializeField, Header("Debug")]
    //private bool debugMode = false;

    private Vector3 rotAxis = Vector3.zero;
    private Animator anim;
    private Transform player;
    private PlayerController pController;
    private List<Vector3> startPosTranslation;
    private List<PerspectiveTeleporter> oldTeleporters;
    private bool isActive = false, wasBridged = false;
    private int activationAnimation, deactivationAnimation;
    private float correctAngle;

    private void OnEnable()
    {
        objInteractionControl.action.Enable();
        movementControl.action.Enable();
    }

    private void OnDisable()
    {
        objInteractionControl.action.Disable();
        movementControl.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        pController = player.GetComponent<PlayerController>();
        activationAnimation = Animator.StringToHash("RotMec_Activated");
        deactivationAnimation = Animator.StringToHash("RotMec_Deactivated");
        oldTeleporters = new List<PerspectiveTeleporter>();
        oldTeleporters.Clear();
        startPosTranslation = new List<Vector3>();
        BridgeTeleporters();
        for(int i = 0; i < movingObjects.Length; i++)
        {
            startPosTranslation.Add(movingObjects[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mechanismToPlayerDistance = Vector3.Distance(player.position,transform.position);

        if(mechanismToPlayerDistance < activationDistance && objInteractionControl.action.triggered)
        {
            if(!isActive)
            {
                isActive = true;
                pController.ActiveControls = false;
                anim.Play(activationAnimation);
                foreach(MovingObjects _obj in movingObjects)
                {
                    targetGroup.AddMember(_obj.transform, 5, 1);
                }
            }
            else
            {
                isActive = false;
                pController.ActiveControls = true;
                anim.Play(deactivationAnimation);
                foreach (MovingObjects _obj in movingObjects)
                {
                    targetGroup.RemoveMember(_obj.transform);
                }
            }
        }

        if(isActive)
        {
            Vector3 move = movementControl.action.ReadValue<Vector2>();
            int i = 0;
            foreach (MovingObjects _obj in movingObjects)
            {
                if (transformationType == TransformationType.Rotation)
                #region Rotation
                {
                    switch (_obj.movementAxis)
                    {
                        case Axis.x:
                            rotAxis = Vector3.right;
                            break;
                        case Axis.y:
                            rotAxis = Vector3.up;
                            break;
                        case Axis.z:
                            rotAxis = Vector3.forward;
                            break;
                    }

                    Vector3 inputAngle = (_obj.transform.position + move) - _obj.transform.position;
                    correctAngle = (Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg) + (180 * move.magnitude);
                    Quaternion rot = Quaternion.AngleAxis(-correctAngle, rotAxis);
                    if (inputAngle.magnitude <= 0)
                    {
                        if (!wasBridged)
                        {
                            BridgeTeleporters();
                        }

                        Vector3 alignedForward = Helpers.NearestWorldAxis(_obj.transform.forward);
                        Vector3 alignedUp = Helpers.NearestWorldAxis(_obj.transform.up);
                        Quaternion newRot = Quaternion.LookRotation(alignedForward, alignedUp);

                        _obj.transform.rotation = Quaternion.Slerp(_obj.transform.rotation, newRot, rotationSpeed);
                        wasBridged = true;
                    } 
                    else
                    { 
                        wasBridged = false;
                        _obj.transform.rotation = Quaternion.Slerp(_obj.transform.rotation, rot, rotationSpeed);
                    }
                }
                #endregion
                else
                #region Translation
                {
                    Vector3 endVector = _obj.transform.position;
                    int dirMult = (_obj.direction == Direction.Normal) ? 1 : -1;
                    
                    switch(_obj.movementAxis)
                    {
                        case Axis.x:
                            if(move.magnitude > 0)
                            {
                                switch(_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        endVector.x += translationSpeed * move.y * move.magnitude * dirMult;
                                        endVector.x = Mathf.Clamp(endVector.x, startPosTranslation[i].x, startPosTranslation[i].x + translationDistance);
                                        break;
                                    case TranslationPivot.End:
                                        endVector.x += translationSpeed * move.y * move.magnitude * dirMult;
                                        endVector.x = Mathf.Clamp(endVector.x, startPosTranslation[i].x - translationDistance, startPosTranslation[i].x);
                                        break;
                                }

                                wasBridged = false;
                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                            }
                            else
                            {
                                if (!wasBridged)
                                {
                                    BridgeTeleporters();
                                }

                                endVector.x = Mathf.Round(_obj.transform.position.x);
                                
                                switch (_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        //Pos 0
                                        if (endVector.x == (Mathf.Round(startPosTranslation[i].x)))
                                        {
                                            endVector.x -= translationOffset;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.x > (Mathf.Round(startPosTranslation[i].x)))
                                        { endVector.x -= translationOffset; }
                                        break;
                                    case TranslationPivot.End:
                                        //Pos 0
                                        if (endVector.x == (Mathf.Round(startPosTranslation[i].x)))
                                        {
                                            endVector.x -= translationOffset * 2;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.x < (Mathf.Round(startPosTranslation[i].x)))
                                        { endVector.x += translationOffset; }
                                        break;
                                }

                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                                wasBridged = true;
                            }
                            break;
                        case Axis.y:
                            if (move.magnitude > 0)
                            {
                                switch (_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        endVector.y += (translationSpeed) * move.ToIsometric().y * move.magnitude * dirMult;
                                        endVector.y = Mathf.Clamp(endVector.y, startPosTranslation[i].y, startPosTranslation[i].y + translationDistance);
                                        break;
                                    case TranslationPivot.End:
                                        endVector.y += translationSpeed * move.ToIsometric().y * move.magnitude * dirMult;
                                        endVector.y = Mathf.Clamp(endVector.y, startPosTranslation[i].y - translationDistance, startPosTranslation[i].y);
                                        break;
                                }

                                wasBridged = false;
                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                            }
                            else
                            {
                                if (!wasBridged)
                                {
                                    BridgeTeleporters();
                                }

                                endVector.y = Mathf.Round(_obj.transform.position.y);

                                switch (_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        //Pos 0
                                        if (endVector.y == (Mathf.Round(startPosTranslation[i].y)))
                                        {
                                            endVector.y += translationOffset * 2;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.y > (Mathf.Round(startPosTranslation[i].y)))
                                        { endVector.y -= translationOffset; }
                                        break;
                                    case TranslationPivot.End:
                                        //Pos 0
                                        if (endVector.y == (Mathf.Round(startPosTranslation[i].y)))
                                        {
                                            endVector.y += translationOffset;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.y < (Mathf.Round(startPosTranslation[i].y)))
                                        { endVector.y += translationOffset; }
                                        break;
                                }
                               
                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                                wasBridged = true;
                            }
                            break;
                        case Axis.z:
                            if (move.magnitude > 0)
                            {
                                switch (_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        endVector.z += translationSpeed * -move.x * move.magnitude * dirMult;
                                        endVector.z = Mathf.Clamp(endVector.z, startPosTranslation[i].z, startPosTranslation[i].z + translationDistance);
                                        break;
                                    case TranslationPivot.End:
                                        endVector.z += translationSpeed * -move.x * move.magnitude * dirMult;
                                        endVector.z = Mathf.Clamp(endVector.z, startPosTranslation[i].z - translationDistance, startPosTranslation[i].z);
                                        break;
                                }

                                wasBridged = false;
                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                            }
                            else
                            {
                                if (!wasBridged)
                                {
                                    BridgeTeleporters();
                                }

                                endVector.z = Mathf.Round(_obj.transform.position.z);

                                switch (_obj.translationPivot)
                                {
                                    case TranslationPivot.Start:
                                        //Pos 0
                                        if (endVector.z == (Mathf.Round(startPosTranslation[i].z)))
                                        {
                                            endVector.z -= translationOffset;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.z > (Mathf.Round(startPosTranslation[i].z)))
                                        { endVector.z += translationOffset; }
                                        break;
                                    case TranslationPivot.End:
                                        //Pos 0
                                        if (endVector.z == (Mathf.Round(startPosTranslation[i].z)))
                                        {
                                            endVector.z -= translationOffset * 2;
                                        }
                                        //Pos 1 - MaxDistance
                                        if (endVector.z < (Mathf.Round(startPosTranslation[i].z)))
                                        { endVector.z += translationOffset; }
                                        break;
                                }

                                _obj.transform.position = Vector3.Lerp(_obj.transform.position, endVector, rotationSpeed);
                                wasBridged = true;
                            }
                            break;
                    }
                }
                #endregion
                i++;
            }
        }
    }
    
    private void BridgeTeleporters()
    {
        //Delete the conections in the old teleporters because it change position
        foreach (PerspectiveTeleporter tp in oldTeleporters)
        {
            tp.TeleporterReceiver = null;
        }

        //Delete the old teleporters because it change position
        oldTeleporters.Clear();

        int levelMask = 1 << 6;
        RaycastHit hit;
        foreach (MovingObjects _obj in movingObjects)
        {
            //Find Teleporters by iterating in the child of the moving object
            for (int i = 0; i < _obj.transform.childCount; i++)
            {
                //Getting the teleporter on the moving obj
                PerspectiveTeleporter teleporterInMovingOBJ = _obj.transform.GetChild(i).GetComponent<PerspectiveTeleporter>();
                if (teleporterInMovingOBJ != null)
                {
                    //teleporterInMovingOBJ.TeleporterReceiver = null;
                    Transform tpTransform = teleporterInMovingOBJ.GetComponent<Transform>();
                    if (Physics.Raycast(tpTransform.position, tpTransform.TransformDirection(Vector3.forward) * -2, out hit, 2f, levelMask))
                    {
                        //Getting the bridge object
                        TeleporterBridge bridge = hit.transform.GetComponent<TeleporterBridge>();
                        if(bridge != null)
                        {
                            //Getting the teleporter connected to the bridge
                            PerspectiveTeleporter otherTeleporter = bridge.teleporterToConnect.GetComponent<PerspectiveTeleporter>();
                            //Connecting the teleporters
                            teleporterInMovingOBJ.TeleporterReceiver = bridge.teleporterToConnect;
                            otherTeleporter.TeleporterReceiver = tpTransform;
                            //Store the other teleporter to delete the conection once it change the conection
                            oldTeleporters.Add(otherTeleporter);
                        }
                    }
                }
            }
        }
    }
}

enum Axis
{
    x,
    y,
    z
}

enum TransformationType
{
    Rotation,
    Translation
}

enum TranslationPivot
{
    Start,
    End
}

enum Direction
{
    Normal,
    Inverse
}

[Serializable]
struct MovingObjects
{
    public Transform transform;
    public Axis movementAxis;
    public TranslationPivot translationPivot;
    public Direction direction;
}