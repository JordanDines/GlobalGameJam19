﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 3;
    [SerializeField]
    private float shuffleSpeed = 2;
    [SerializeField]
    private float shuffleFreq = 1;

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

    [SerializeField]
    private GameObject armsPivot;

    [SerializeField]
    private AnimationCurve shuffleSpeedCurve;

    [SerializeField]
    private Animator armsAC;

    [SerializeField]
    private GameObject boxes;

    private CharacterController cc;

    private float targetSpeed;
    private float currentSpeed;

    private Vector2 targetRotation;
    private Vector2 rotation;

    private Camera cam;
    private GameObject arms;

    private Animator camAnimator;

    private bool moving = false;
    private bool onBed = false;

    private float shuffleTime = 0;

    private ItemManager itemManager;

    public enum PlayerState
    {
        Default,
        Interacting,
        Animating
    }

    private PlayerState state;

    public PlayerState State { get { return state; } }

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        targetRotation = Vector2.zero;
        rotation = Vector2.zero;
        cam = Camera.main;
        camAnimator = GetComponent<Animator>();
        arms = armsAC.gameObject;
        itemManager = GetComponent<ItemManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = PlayerState.Default;
    }

    private void Update()
    {
        if (state != PlayerState.Animating)
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

        moving = movement.magnitude > 0;

        if (onBed)
        {
            shuffleTime += Time.deltaTime * shuffleFreq;
            if (shuffleTime > 1)
                shuffleTime -= 1;
        }
        else
        {
            shuffleTime = 0;
        }

        targetSpeed = moving ? (onBed ? shuffleSpeedCurve.Evaluate(shuffleTime) * shuffleSpeed : moveSpeed) : 0;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothMove * Time.deltaTime);

        {
            movement = movement.normalized * currentSpeed;
            cc.Move(movement * Time.deltaTime);
        }


        // Move the camera with the player
        cam.transform.position = cameraPivot.transform.position;

        arms.transform.position = armsPivot.transform.position;
        arms.transform.rotation = armsPivot.transform.rotation;
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
        camAnimator.SetBool("Moving", moving);
        camAnimator.SetBool("OnBed", onBed);
        armsAC.SetBool("isWalking", moving);

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

    public void SetInteracting(float time)
    {
        StartCoroutine(StartInteracting(time));
    }

    public void SetAnimating(float time)
    {
        StartCoroutine(StartAnimating(time));
    }

    private IEnumerator StartInteracting(float time)
    {
        if (state != PlayerState.Default)
        {
            Debug.LogError("Changing player state too quick!", gameObject);
            yield break;
        }

        state = PlayerState.Interacting;

        yield return new WaitForSeconds(time);

        state = PlayerState.Default;
    }

    private IEnumerator StartAnimating(float time)
    {
        if (state != PlayerState.Default)
        {
            Debug.LogError("Changing player state too quick!", gameObject);
            yield break;
        }

        state = PlayerState.Animating;

        yield return new WaitForSeconds(time);

        state = PlayerState.Default;
    }

    public void ItemPlaced()
    {

    }

}
