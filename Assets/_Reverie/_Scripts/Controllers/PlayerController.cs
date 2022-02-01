using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector3 playerVelocity, move, objVelocity, movableBoxMoveDir;
    private bool groundedPlayer, activeControls = true, isPushing = false;
    private CharacterController controller;
    private Animator anim;
    private int speedAnimParamID, jumpAnimation, pushAnimation, pushingAnimParamID;
    private float currentSpeed, smoothRefVel;
    private Vector2 inputSmoothVel, currentInputVector, movement;
    private Rigidbody movableBoxRb;


    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float pushBoxSpeed = 2.0f;
    [SerializeField] private float pushSpeed = 1.4f;
    [SerializeField] private float minDistanceForStopPushing = 0.1f;
    [SerializeField] private float runningMultiplier = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference runControl;
    [SerializeField] private InputActionReference pickObjControl;
    [SerializeField] private InputActionReference pauseControl;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float inputSmoothSpeed = 0.2f;
    [SerializeField] private float animSmoothSpeed = 0.2f;

    #region Accesors
    public bool ActiveControls
    {
        get { return activeControls; }
        set { activeControls = value; }
    }

    public InputActionReference MovementControl
    {
        get { return movementControl; }
    }
    #endregion

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        runControl.action.Enable();
        pickObjControl.action.Enable();
        pauseControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        runControl.action.Disable();
        pickObjControl.action.Disable();
        pauseControl.action.Disable();
    }

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChange;
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        speedAnimParamID = Animator.StringToHash("Speed");
        pushingAnimParamID = Animator.StringToHash("Pushing");
        jumpAnimation = Animator.StringToHash("Jump");
        pushAnimation = Animator.StringToHash("Push");
        currentSpeed = playerSpeed;
    }

    void Update()
    {
        #region Pushing

        //Calculate box distance

        if (isPushing)
        {
            Vector3 moveDir = Vector3.zero;

            moveDir = Vector3.SmoothDamp(movableBoxRb.transform.position,
                movableBoxRb.transform.position + Helpers.NearestWorldAxis(movableBoxMoveDir) * pushBoxSpeed * movement.magnitude,
                ref objVelocity, inputSmoothSpeed);

            movableBoxRb.MovePosition(moveDir);

            float distanceBoxToPlayer = Vector3.Distance(transform.position, movableBoxRb.transform.position);
            if(distanceBoxToPlayer > minDistanceForStopPushing)
            {
                isPushing = false;
                anim.SetBool(pushingAnimParamID, isPushing);
                movableBoxRb = null;
                movableBoxMoveDir = Vector3.zero;
            }
        }

        if (isPushing && pickObjControl.action.triggered)
        {
            isPushing = false;
            anim.SetBool(pushingAnimParamID, isPushing);
            movableBoxRb = null;
            movableBoxMoveDir = Vector3.zero;
        }

        #endregion

        #region Movement
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (activeControls)
        {
            move = GetInputVector3();
            if(isPushing)
            {
                if(Helpers.NearestWorldAxis(movableBoxMoveDir).x >= 0.1f || Helpers.NearestWorldAxis(movableBoxMoveDir).z <= -0.1f)
                {
                    controller.Move(Helpers.NearestWorldAxis(movableBoxMoveDir) * move.x * Time.deltaTime * currentSpeed);
                }
                else
                {
                    controller.Move(Helpers.NearestWorldAxis(movableBoxMoveDir) * -move.x * Time.deltaTime * currentSpeed);
                }
                
            }
            else
            {
                controller.Move(move.ToIsometric() * Time.deltaTime * currentSpeed);
            }

            // Changes the height position of the player..
            if (jumpControl.action.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                anim.CrossFade(jumpAnimation, animSmoothSpeed);
            }
        }
        else { movement = Vector2.zero; currentInputVector = Vector2.zero; move = Vector3.zero; }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        if (movement != Vector2.zero)
        {
            Vector3 targetAngle = (transform.position + move.ToIsometric()) - transform.position;
            Quaternion rot = Quaternion.identity;

            if (activeControls)
            {
                if (isPushing)
                {
                    Vector3 alignedForward = Helpers.NearestWorldAxis(transform.forward);
                    Vector3 alignedUp = Helpers.NearestWorldAxis(transform.up);
                    rot = Quaternion.LookRotation(alignedForward, alignedUp);
                }
                else
                {
                    rot = Quaternion.LookRotation(targetAngle, Vector3.up);
                }

                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, rotationSpeed);

                //Im running
                if (runControl.action.ReadValue<float>() == 1 && !isPushing)
                {
                    float targetSpeed = playerSpeed * runningMultiplier * movement.magnitude;
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothRefVel, inputSmoothSpeed);
                    anim.SetFloat(speedAnimParamID, Mathf.Clamp(movement.magnitude, 0f, 1f), animSmoothSpeed, Time.deltaTime);
                }
                else // Im Walking
                {
                    if(isPushing)
                    {
                        currentSpeed = Mathf.SmoothDamp(currentSpeed, pushSpeed, ref smoothRefVel, inputSmoothSpeed);
                        anim.SetFloat(speedAnimParamID, Mathf.Clamp(movement.magnitude, 0f, 1f), animSmoothSpeed, Time.deltaTime);
                    }
                    else
                    {
                        currentSpeed = Mathf.SmoothDamp(currentSpeed, playerSpeed, ref smoothRefVel, inputSmoothSpeed);
                        anim.SetFloat(speedAnimParamID, Mathf.Clamp(movement.magnitude, 0f, 0.65f), animSmoothSpeed, Time.deltaTime);
                    }
                }
            }
        }
        else { anim.SetFloat(speedAnimParamID, 0f, animSmoothSpeed, Time.deltaTime); }
        #endregion

        #region Pause
        if(pauseControl.action.triggered)
        {
            GameState currentGameState = GameManager.Instance.state;
            print(currentGameState);
            GameState newState = currentGameState == GameState.Gameplay ? GameState.Pause : GameState.Gameplay;

            GameManager.Instance.SetGameState(newState);
        }
        #endregion
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("MovableObject"))
        {
            if(!isPushing && pickObjControl.action.triggered)
            {
                isPushing = true;
                if (hit.collider.gameObject.GetComponent<Rigidbody>() == null) return;
                movableBoxRb = hit.collider.gameObject.GetComponent<Rigidbody>();
                movableBoxMoveDir = hit.moveDirection;
                anim.SetBool(pushingAnimParamID, isPushing);
            }
            else if (isPushing && pickObjControl.action.triggered)
            {
                isPushing = false;
                anim.SetBool(pushingAnimParamID, isPushing);
                movableBoxRb = null;
                movableBoxMoveDir = Vector3.zero;
            }
        }
    }

    private void OnGameStateChange(GameState _newState)
    {
        activeControls = _newState == GameState.Gameplay;
    }

    public void ChangeAnimationCrossfade(string _animationName)
    {
        int _anim = Animator.StringToHash(_animationName);
        anim.CrossFade(_anim, animSmoothSpeed);
    }

    public void ChangeAnimationCrossfade(int _animHash)
    {
        anim.CrossFade(_animHash, animSmoothSpeed);
    }

    public void ChangeAnimationCrossfade(int _animHash, float _smoothTime)
    {
        anim.CrossFade(_animHash, _smoothTime);
    }

    public Vector3 GetInputVector3()
    {
        movement = movementControl.action.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, movement, ref inputSmoothVel, inputSmoothSpeed);
        return new Vector3(currentInputVector.x, 0, currentInputVector.y);
    }
}
