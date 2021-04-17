using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputController : MonoBehaviour
{

    [Header("Moving Parameters")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    [Range(0, 90)]
    public float lookXLimit = 45.0f;
    public float mouseRollTreshold = 0.1f;

    [HideInInspector]
    public bool canMove = true;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;
    private bool isRunning;
    private bool wasGrounded = true;


    private Player player;
    private CharacterAudioController characterAudio;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        characterAudio = GetComponent<CharacterAudioController>();
    }
    private void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateWeapons();
        characterAudio.UpdateSound();
    }


    private void UpdateWeapons()
    {
        if (Input.GetMouseButton(0))
        {
            player.Shoot();
        }

        if (Input.GetButtonDown("Reload"))
        {
            player.Reload();
        }

        if (Input.GetButtonDown("Quick Attack"))
        {
            player.QuickAttack();
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > mouseRollTreshold)
        {
            player.ChangeWeaponToNext(Input.mouseScrollDelta.y > 0 ? 1 : -1);
        }
    }
    private void UpdateMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        isRunning = Input.GetButton("Run");
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (IsWalking)
        {
            characterAudio.state = characterAudio.state = CharacterState.Walking;
            if (isRunning)
            {
                characterAudio.state = characterAudio.state = CharacterState.Running;
            }
        }
        else
        {
            characterAudio.state = CharacterState.Idle;
        }

        if (IsJumping)
        {
            characterAudio.state = characterAudio.state = CharacterState.Jumping;
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (IsLanding)
        {
            characterAudio.state = characterAudio.state = CharacterState.Landing;
        }
        wasGrounded = characterController.isGrounded;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and childs rotation
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            transform.rotation = Quaternion.identity;
            transform.Rotate(Vector3.right, rotationX, Space.World);
            transform.Rotate(Vector3.up, rotationY, Space.World);
        }
    }

    private bool IsWalking { get { return new Vector2(moveDirection.x, moveDirection.z) != Vector2.zero && characterController.isGrounded; } }
    private bool IsJumping { get { return Input.GetButton("Jump") && canMove && characterController.isGrounded; } }
    private bool IsLanding { get { return !wasGrounded && characterController.isGrounded; } }
}
