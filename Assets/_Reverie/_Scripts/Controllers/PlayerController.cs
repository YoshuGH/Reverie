using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private CharacterController controller;
    private Animator anim;
    private int speedAnimParamID, jumpAnimation;
    private float currentSpeed, smoothRefVel;
    private Vector2 inputSmoothVel, currentInputVector;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float runningMultiplier = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference runControl;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float inputSmoothSpeed = 0.2f;
    [SerializeField] private float animSmoothSpeed = 0.2f;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        runControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        runControl.action.Disable();
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        speedAnimParamID = Animator.StringToHash("Speed");
        jumpAnimation = Animator.StringToHash("Jump");
        currentSpeed = playerSpeed;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }    

        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector,movement, ref inputSmoothVel, inputSmoothSpeed);
        Vector3 move = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        controller.Move(move.ToIsometric() * Time.deltaTime * currentSpeed);

        // Changes the height position of the player..
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            anim.CrossFade(jumpAnimation, animSmoothSpeed);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        if (movement != Vector2.zero)
        {
            Vector3 targetAngle = (transform.position + move.ToIsometric()) - transform.position;//Mathf.Atan2(moveVec3.x, moveVec3.y) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.LookRotation(targetAngle, Vector3.up);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, rotationSpeed);

            //Im running
            if (runControl.action.ReadValue<float>() == 1)
            {
                float targetSpeed = playerSpeed * runningMultiplier * movement.magnitude;
                currentSpeed  = Mathf.SmoothDamp(currentSpeed,targetSpeed, ref smoothRefVel, inputSmoothSpeed);
                anim.SetFloat(speedAnimParamID, Mathf.Clamp(movement.magnitude, 0f, 1f), animSmoothSpeed, Time.deltaTime);
            }
            else // Im Walking
            {
                currentSpeed = Mathf.SmoothDamp(currentSpeed, playerSpeed, ref smoothRefVel, inputSmoothSpeed);
                anim.SetFloat(speedAnimParamID, Mathf.Clamp(movement.magnitude, 0f, 0.65f), animSmoothSpeed, Time.deltaTime);
            }
        }
        else { anim.SetFloat(speedAnimParamID, 0f, animSmoothSpeed, Time.deltaTime); }
    }

}
