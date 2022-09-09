using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

/// <summary>
/// FPController - class to handle a first person controller (head camera and body translation)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    [Header("Camera Values")]
    [SerializeField] GameObject CameraPosition;
    [SerializeField] bool CameraPitchInverted = false;
    [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None; //debug set to false
    [SerializeField] float StartMotionPauseDuration = 2.0f;
    [Range(0, 100)][SerializeField] float rotationSpeed = 1f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    Vector3 CamPitchDirection;
    private Vector2 calculatedCamRotation;
    private bool bMovementActive = true;
    private Camera mainCamera;

    [Header("Movement Values")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float dashSpeed = 6f;
    [Range(0f, 1f)]
    [SerializeField] float dashDuration;
    float moveSpeedBoost = 0f;
    [SerializeField] float additionalSpeedModifiers = 0f;
    [SerializeField] float jumpModifier = 0f;
    [SerializeField] bool jumpingActive = false;
    [SerializeField] bool StartSceneCurrentScene = false;

    [Header("Collision Values")]
    [SerializeField] GroundCollider groundColliderRef;
    public bool Grounded = false;
    private Rigidbody rb;
    private Vector3 direction = Vector3.zero;
    private Vector3 moveVel = Vector3.zero;

    //[Header("Gun")]
    //[SerializeField] GameObject arms;
    //Gun gun;

    //HUD hud;
    //PauseScreen pause;

    //[SerializeField] HealthDamageScaler damage;
    //[SerializeField] HealthDamageScaler healthScaler;

    //Player location
    static public Vector3 FPLocation;

    //Interactable lookingAt;

    private void Awake()
    {
        bMovementActive = false;
    }

    public void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = CameraPosition.transform.position;
        mainCamera.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        Cursor.lockState = cursorLockMode;
        CamPitchDirection = CameraPitchInverted ? Vector3.right : Vector3.left;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //health = maxHealth;
        //FPController.FPLocation = gameObject.transform.position;
        //bool holder = arms.gameObject.activeSelf;
        //arms.gameObject.SetActive(true);
        //gun = arms.GetComponentInChildren<Gun>();
        //arms.gameObject.SetActive(holder);
        StartCoroutine(SceneStartMotionPause());
        //base.Start();
        //hud = FindObjectOfType<HUD>();
        //if (hud != null) hud.UpdateDisplayText("");
        //pause = FindObjectOfType<PauseScreen>(true);
        if (PlayerPrefs.HasKey("Sensitivity")) rotationSpeed = PlayerPrefs.GetFloat("Sensitivity");
    }

    private void OnApplicationQuit()
    {
        //healthScaler.scaler = 10;
    }

    /// <summary>
    /// SceneStartMotionPause - used at scene start to remove mouse capture while scene is 
    ///     not fully loaded.
    /// </summary>
    /// <returns></returns>
    IEnumerator SceneStartMotionPause()
    {
        yield return new WaitForSeconds(StartMotionPauseDuration);
        bMovementActive = true;
    }

    Vector2 inputValues;
    /// <summary>
    /// OnMouseDelta - handle changes delta mouse changes and applies them to a rotation
    /// </summary>
    /// <param name="value"></param>
    private void OnMouseDelta(InputValue value)
    {
        if (bMovementActive)
        {
            inputValues = value.Get<Vector2>();
            /*inputValues *= .5f;
            inputValues *= .1f;
            inputValues = inputValues * rotationSpeed * Time.deltaTime;
            calculatedCamRotation += new Vector2(inputValues.x, inputValues.y);
            OrientCamera();*/
        }
    }

    /// <summary>
    /// OrientCamera - orients the camera based on on mouse position changes
    ///     Since camera is not childed to the player we rotate both the camera and the player.
    /// </summary>
    private void OrientCamera()
    {
        inputValues *= rotationSpeed;
        calculatedCamRotation += inputValues;
        calculatedCamRotation.y = Mathf.Clamp(calculatedCamRotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(calculatedCamRotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(calculatedCamRotation.y, CamPitchDirection);

        mainCamera.transform.localRotation = xQuat * yQuat; // set head cam pitch and yaw
        transform.localRotation = xQuat;                    // set player yaw 
        inputValues = Vector2.zero;
    }

    private void OnEnable()
    {
        //Debug.Log("FPController Enable");
        Cursor.lockState = cursorLockMode;
    }

    private void OnDisable()
    {
        //Debug.Log("FPController Disable");
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Update - orients camera if bMovementActive
    ///     Positions mainCamera to head
    /// </summary>
    private void Update()
    {
        if (bMovementActive)
        {
            GroundTriggerCheck(); //update the ground trigger
            mainCamera.transform.position = CameraPosition.transform.position;
            if (inputValues.magnitude > 0) OrientCamera();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        //LookForInteractableObjects();
        //FPController.FPLocation = gameObject.transform.position;
    }

    //void LookForInteractableObjects()
    //{
    //    if (Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.forward, out RaycastHit fireHit, 2))
    //    {
    //        if (fireHit.collider.TryGetComponent<Interactable>(out Interactable obj))
    //        {
    //            lookingAt = obj;
    //            lookingAt.Highlight();
    //            hud.UpdateDisplayText(lookingAt.GetDisplayText());
    //        }
    //        else if (lookingAt != null)
    //        {
    //            lookingAt.UnHighlight();
    //            hud.UpdateDisplayText("");
    //            lookingAt = null;
    //        }
    //    }
    //    else if (lookingAt != null)
    //    {
    //        lookingAt.UnHighlight();
    //        hud.UpdateDisplayText("");
    //        lookingAt = null;
    //    }
    //}

    /// <summary>
    /// FixedUpdate - Moves object if bMovementActive
    /// </summary>
    private void FixedUpdate()
    {
        if (bMovementActive) // only operate movement when active
        {
            Move(); //apply speed to the calculated direction                  
        }
    }

    /// <summary>
    /// OnMove - handles key input for translation
    /// </summary>
    /// <param name="value"></param>
    private void OnMove(InputValue value)
    {
        direction.x = value.Get<Vector2>().y;
        direction.z = value.Get<Vector2>().x;
        //if (direction.magnitude >= 0.1f) AudioManager.instance.Play("PlayerMovement");
        //else AudioManager.instance.Stop("PlayerMovement");
    }


    /// <summary>
    /// Move - applies speed to the generated direction if the player is grounded
    /// </summary>
    private void Move()
    {
        if (direction.magnitude >= 0.1f)
        {
            //if grounded apply speed to direction otherwise allow rv.velocity to take over
            //if (Grounded) { moveVel = (transform.forward * direction.x + transform.right * direction.z) * Time.deltaTime * (moveSpeed + moveSpeedBoost); }
            //else { moveVel = rb.velocity; }
            //moveVel.y = 0;
            if (Grounded || StartSceneCurrentScene)
            {
                moveVel = (transform.forward * direction.x + transform.right * direction.z) * Time.deltaTime * (moveSpeed + moveSpeedBoost);
                SpeedClamp(ref moveVel);
                rb.velocity = moveVel;
            }
            else
            {
                // if not on ground apply an arbitrary negative y to velocity to force back to ground
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + -5, rb.velocity.z);
            }
        }
    }

    /// <summary>
    /// SpeedClamp - if the velocity of the player is beyond the max, scale it back to the max
    /// </summary>
    /// <param name="inputVelocity"></param>
    private void SpeedClamp(ref Vector3 inputVelocity)
    {
        float tempMoveMax = moveSpeed + moveSpeedBoost + additionalSpeedModifiers;
        if (inputVelocity.magnitude > tempMoveMax)
        {
            inputVelocity.Normalize();
            inputVelocity *= moveSpeed + tempMoveMax;
        }
    }

    /// <summary>
    /// GroundTriggerCheck - checks referenced ground clamp to see if is on the ground
    /// </summary>
    /// <returns></returns>
    public bool GroundTriggerCheck()
    {
        Grounded = groundColliderRef.IsGrounded;
        return Grounded;
    }

    /// <summary>
    /// OnJump - handles input from FirstPersonInput controller
    ///     Jumps if the character is grounded
    /// </summary>
    private void OnJump(InputValue value)
    {
        if (Grounded && jumpingActive)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpModifier, rb.velocity.z);
        }
    }

    //private void OnBash(InputValue value)
    //{
    //    gun.Bash();
    //}

    //private void OnShoot(InputValue value)
    //{
    //    if (gun != null && arms.gameObject.activeSelf)
    //    {
    //        if (value.Get<float>() > 0) gun.Shoot();
    //        else gun.StopShooting();
    //    }
    //}

    //private void OnCharge(InputValue value)
    //{
    //    if (gun != null && arms.gameObject.activeSelf)
    //    {
    //        if (value.Get<float>() > 0) gun.Charge();
    //        else gun.StopCharging();
    //    }
    //}

    private void OnDash(InputValue value)
    {
        Dash();
    }

    //private void OnInteract(InputValue value)
    //{
    //    if (lookingAt != null)
    //    {
    //        if (!arms.gameObject.activeSelf && lookingAt.GetComponent<PickupGun>()) arms.gameObject.SetActive(true);
    //        gun.Interact(lookingAt);
    //    }
    //}

    //private void OnPause(InputValue value)
    //{
    //    if (!pause.paused)
    //    {
    //        pause.gameObject.SetActive(true);
    //        pause.Pause();
    //    }
    //    else pause.Resume();
    //}

    /// <summary>
    /// Adds a move speed boost for a short time
    /// </summary>
    void Dash()
    {
        moveSpeedBoost = dashSpeed;
        //AudioManager.instance.Stop("PlayerMovement");
        //AudioManager.instance.Play("PlayerDash");
        //await Task.Delay((int)(dashDuration * 1000));
        //AudioManager.instance.Play("PlayerMovement");
        moveSpeedBoost = 0;
    }

    //protected override void Die()
    //{
    //    AudioManager.instance.StopAll();
    //    LoadSceneAsync.instance.LoadScene(7);
    //}

    //public void UpdateHealth(float amount)
    //{
    //    maxHealth = healthScaler.scaler += amount;
    //    TakeDamage(-amount);
    //}

    //public void UpdateDamage(float amount)
    //{
    //    damage.scaler += amount;
    //}

    //overrides for Damagable
    //public override void EnableNavAgent() { }

    //public override void DisableNavAgent() { }

    //public override IEnumerator WaitForVelocityToDissipate() { yield return null; }
}