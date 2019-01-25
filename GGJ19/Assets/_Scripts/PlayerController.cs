using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 10;

    [SerializeField]
    private Vector2 mouseSensitivity = new Vector2(1, 1);

    [SerializeField]
    private Vector2 cameraXMinMax = new Vector2(-60.0f, 60.0f);

    [SerializeField]
    private AnimationCurve bobbingCurve;

    private float bobbingTime = 0;

    private CharacterController cc;

    private Vector2 rotation;

    private Camera camera;

    private float cameraHeight;

    private bool moving = false;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rotation = Vector2.zero;
        camera = Camera.main;
    }

    private void Start()
    {
        cameraHeight = Camera.main.transform.position.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (moving)
        {
            bobbingTime += Time.deltaTime;
            if (bobbingTime > 1)
            {
                bobbingTime -= 1;
            }
        }

        Movement();

        Mouse();
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
        {
            movement = movement.normalized * moveSpeed;

            cc.Move(movement * Time.deltaTime);

            moving = true;
        }
        else
        {
            moving = false;
        }

        // Move the camera with the player
        camera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraHeight + bobbingCurve.Evaluate(bobbingTime), transform.position.z);
    }

    private void Mouse()
    {
        rotation += new Vector2(Input.GetAxis("Mouse Y") * mouseSensitivity.x, Input.GetAxis("Mouse X") * mouseSensitivity.y);

        rotation.x = Mathf.Clamp(rotation.x, cameraXMinMax.x, cameraXMinMax.y);

        transform.localEulerAngles = new Vector3(0, rotation.y, 0);

        camera.transform.localEulerAngles = new Vector3(-rotation.x, rotation.y, 0);

    }
}
