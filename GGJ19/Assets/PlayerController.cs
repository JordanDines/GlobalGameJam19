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

    private CharacterController cc;

    private Vector2 rotation;

    private Camera camera;

    private float cameraY = 1.8f;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rotation = Vector2.zero;
        camera = Camera.main;
    }

    private void Start()
    {
        cameraY = Camera.main.transform.position.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
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

        movement = movement.normalized * moveSpeed;

        cc.Move(movement * Time.deltaTime);

        // Move the camera with the player
        camera.transform.position = new Vector3(transform.position.x, cameraY, transform.position.z);
    }

    private void Mouse()
    {
        rotation += new Vector2(Input.GetAxis("Mouse Y") * mouseSensitivity.x, Input.GetAxis("Mouse X") * mouseSensitivity.y);

        rotation.x = Mathf.Clamp(rotation.x, cameraXMinMax.x, cameraXMinMax.y);

        transform.localEulerAngles = new Vector3(0, rotation.y, 0);

        camera.transform.localEulerAngles = new Vector3(-rotation.x, rotation.y, 0);

    }
}
