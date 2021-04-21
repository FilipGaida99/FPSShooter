using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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

    [Header("Recoil from Moving")]
    public float runningRecoil = 1;
    public float jumpingRecoil = 1;
    public float walkingRecoil = 0.4f;

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

    [Header("Input Mapping")]
    public string verticalAxis = "Vertical";
    public string horizontalAxis = "Horizontal";
    public string mouseAxisX = "Mouse X";
    public string mouseAxisY = "Mouse Y";
    public string jumpButton = "Jump";
    public string mouseScroll = "Mouse ScrollWheel";
    public string runButton = "Run";
    public List<InputEvent> buttonActions;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        characterAudio = GetComponent<CharacterAudioController>();
    }

    private void Update()
    {
        if (!PauseManager.Instance.isPaused)
        {
            UpdateMovement();
            UpdateWeapons();
            characterAudio.UpdateSound();
        }
    }

    public float GetRecoilFromMoveValue()
    {
        if (IsWalking)
        {
            if (isRunning)
            {
                return runningRecoil;
            }
            else
            {
                return walkingRecoil;
            }
        }
        else if (!characterController.isGrounded)
        {
            return jumpingRecoil;
        }
        return 0;
    }


    private void UpdateWeapons()
    {
        foreach (var buttonAction in buttonActions)
        {
            buttonAction.Invoke();
        }

        if (Mathf.Abs(Input.GetAxis(mouseScroll)) > mouseRollTreshold)
        {
            player.ChangeWeaponToNext(Input.mouseScrollDelta.y > 0 ? 1 : -1);
        }

        player.SetRecoilFromMove(GetRecoilFromMoveValue());
    }
    private void UpdateMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        isRunning = Input.GetButton(runButton);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis(verticalAxis) : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis(horizontalAxis) : 0;
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

        // Move object
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and childs rotation
        if (canMove)
        {
            rotationX -= Input.GetAxis(mouseAxisY) * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY += Input.GetAxis(mouseAxisX) * lookSpeed;
            transform.rotation = Quaternion.identity;
            transform.Rotate(Vector3.right, rotationX, Space.World);
            transform.Rotate(Vector3.up, rotationY, Space.World);
        }
    }

    private bool IsWalking { get { return new Vector2(moveDirection.x, moveDirection.z) != Vector2.zero && characterController.isGrounded; } }
    private bool IsJumping { get { return Input.GetButton(jumpButton) && canMove && characterController.isGrounded; } }
    private bool IsLanding { get { return !wasGrounded && characterController.isGrounded; } }
}
