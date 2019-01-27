using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private float fadeTime = 1.0f;

    [SerializeField]
    private GameObject cameraPivot;

    [SerializeField]
    private GameObject armsPivot;

    [SerializeField]
    private GameObject armsPivot2;

    [SerializeField]
    private AnimationCurve shuffleSpeedCurve;

    [SerializeField]
    private Animator armsAnimator;

    [SerializeField]
    private GameObject boxes;

    [SerializeField]
    private Animator doorAnimator;

    [SerializeField]
    private Image fadePanel;

    [SerializeField]
    private Transform endGameCamPos;

    [SerializeField]
    private GameObject creditsPanel;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject crosshair;

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

    private bool paused = false;

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
        arms = armsAnimator.gameObject;
        itemManager = GetComponent<ItemManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        state = PlayerState.Default;
        FadeIn(fadeTime);
        boxes.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Open");
        SetAnimating(6.0f);
        Destroy(cam.GetComponent<Animator>(), 6.0f);
        Time.timeScale = 1;

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        //}

        if (state != PlayerState.Animating)
        {
            Movement();

            Mouse();

            Animations();

            if (Input.GetKey(KeyCode.Escape))
            {
                if (paused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
        else
        {
            arms.transform.position = armsPivot2.transform.position;
            arms.transform.rotation = armsPivot2.transform.rotation;
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
        armsAnimator.SetBool("isWalking", moving);

    }

    private void EndGame()
    {
        Destroy(doorAnimator.gameObject);
        Destroy(arms.gameObject);
        cam.transform.position = endGameCamPos.position;
        cam.transform.rotation = endGameCamPos.rotation;

        FadeIn(fadeTime);

        PlayCredits(fadeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bed")
        {
            onBed = true;
        }
        if (other.tag == "Door" && itemManager.ItemPlaceCount == 5)
        {
            state = PlayerState.Animating;
            FadeOut(fadeTime);
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
        Destroy(boxes.transform.GetChild(0).gameObject);

        if (itemManager.ItemPlaceCount == 5)
        {
            doorAnimator.SetTrigger("OpenDoor");
        }
        else
        {
            boxes.transform.GetChild(1).GetComponent<Animator>().SetTrigger("Open");
        }
    }

    public void FadeOut(float time)
    {
        StartCoroutine(FadeImage(0.0f, 1.0f, time));
    }

    public void FadeIn(float time)
    {
        StartCoroutine(FadeImage(1.0f, 0.0f, time));
    }

    // Fades the fadePlane image from a colour to another over x seconds.
    private IEnumerator FadeImage(float from, float to, float time)
    {
        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / time;
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, Mathf.Lerp(from, to, t));

            yield return null;
        }

        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, to);

        if (to == 1.0f)
        {
            EndGame();
        }
    }

    private IEnumerator PlayCredits(float time)
    {
        yield return new WaitForSeconds(time);

        creditsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshair.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        state = PlayerState.Animating;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshair.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        state = PlayerState.Default;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.SetActive(true);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

}
