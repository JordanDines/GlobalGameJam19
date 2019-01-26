using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 3;
    [SerializeField]
    private float shuffleSpeed = 2;

    [SerializeField]
    private float smoothMove = 15.0f;

    [SerializeField]
    private Vector2 mouseSensitivity = new Vector2(1, 1);

    [SerializeField]
    private Vector2 cameraXMinMax = new Vector2(-60.0f, 60.0f);

    [SerializeField]
    private float cameraLerpSpeed = 10.0f;

    [SerializeField]
    private GameObject cameraPivot;

    private CharacterController cc;

    private float targetSpeed;
    private float currentSpeed;

    private Vector2 targetRotation;
    private Vector2 rotation;

    private Camera cam;

    private Animator animator;

    private bool moving = false;
    private bool onBed = false;

    enum PlayerState
    {
        Default,
        Interacting
    }

    private PlayerState state;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        targetRotation = Vector2.zero;
        rotation = Vector2.zero;
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.Default;
    }

    private void Update()
    {
        if (state != PlayerState.Interacting)
        {
            Movement();

            Mouse();

            Animations();
        }
    }

    private void Movement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }

        if (movement.magnitude > 0)
            moving = true;
        else
            moving = false;

        targetSpeed = moving ? (onBed ? shuffleSpeed : moveSpeed) : 0;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothMove * Time.deltaTime);

        {
            movement = movement.normalized * currentSpeed;
            cc.Move(movement * Time.deltaTime);
        }


        // Move the camera with the player
        cam.transform.position = cameraPivot.transform.position;
    }

    private void Mouse()
    {
        targetRotation += new Vector2(Input.GetAxis("Mouse Y") * mouseSensitivity.x, Input.GetAxis("Mouse X") * mouseSensitivity.y);

        targetRotation.x = Mathf.Clamp(targetRotation.x, cameraXMinMax.x, cameraXMinMax.y);

        // Smoothly move the camera
        rotation = Vector2.Lerp(rotation, targetRotation, cameraLerpSpeed * Time.deltaTime);

        transform.localEulerAngles = new Vector3(0, rotation.y, 0);
        cam.transform.localEulerAngles = new Vector3(-rotation.x, rotation.y, 0);

    }

    private void Animations()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("OnBed", onBed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bed")
        {
            onBed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bed")
        {
            onBed = false;
        }
    }
}
