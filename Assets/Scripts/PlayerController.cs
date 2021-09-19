using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    CharacterController cc;

    public static PlayerController instance { get; set; } = null;

    [System.Serializable]
    public class MovementVariables
    {
        public bool smoothMotion = false;
        public float playerSpeed = 5f;
        public float jumpStrength = 5f;
        public float gravity = 9.8f;
        public float DashDistance = 5f;
        public Vector3 Drag;
    }

    // private MovementVariables _moveVars;
    [SerializeField] MovementVariables _moveVars;
    public MovementVariables moveVars
    {
        get { return _moveVars; }
        set { _moveVars = moveVars; }
    }
    enum ControllerType { Move, SimpleMove };
    [SerializeField] ControllerType type;

    [Space(10)]

    // If this script controls the camera- disable if another script manipulates the camera
    public bool scriptControlsCamera = true;


    enum CameraType { Perspective, Orthographic }
    [SerializeField] CameraType camType;

    enum CameraMoveType { ParallelToView, ParallelToPlayer }
    [SerializeField] CameraMoveType moveType;


    enum CameraView { Dimetric, Isometric, A45Degrees, A60Degrees, Override };

    [SerializeField, Tooltip("Dimetric and Isometric have a camera angle of 30 and 35.264 degrees respectively from the xz plane")] CameraView view;

    // [HideInInspector]


    public Vector3 cameraViewAngleOverrideValue = new Vector3(70f, 45f, 0f);

    public float cameraDistanceFromPlayer = 10f;
    private Vector3 offset;

    public bool cameraOffsetByMouse = true;
    public float maxCameraOffset = 1f;

    public bool cameraSmoothing = false;
    public float smoothSpeed = 0.2f;

    private bool jump = false;
    private bool dash = false;
    private bool shoot = false;

    Vector3 moveDirection;

    Vector3 lookDirection;

    [Space(10)]

    public Transform projectileSpawn;
    // public Rigidbody projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();

        // speed = moveVars.playerSpeed;
        // jumpSpeed = moveVars.jumpStrength;

        moveDirection = Vector3.zero;



    }
    private void Update()
    {
        // this vector2 is the vector from the mouseposition on the screen to the player position on the screen
        Vector2 mouseVector;
        Vector3 lookVector = Vector3.zero;
        if (Camera.main.orthographic)
        {
            if (projectileSpawn)
            {
                mouseVector = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, projectileSpawn.position.y, 0));
            }
            else
            {
                mouseVector = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            }
            lookVector = new Vector3(mouseVector.x, 0, (mouseVector.y / (Mathf.Cos(Camera.main.transform.eulerAngles.y)))).normalized;
        }

        else
        {
            if (projectileSpawn)
            {
                lookVector = GetWorldPositionOnPlane(Input.mousePosition + new Vector3(0, 0, cameraDistanceFromPlayer), projectileSpawn.position.y);// - projectileSpawn.position;
            }
            else
            {
                lookVector = GetWorldPositionOnPlane(Input.mousePosition + new Vector3(0, 0, cameraDistanceFromPlayer), transform.position.y);// - transform.position;
            }
            // projectileSpawn.position;
        }

        // this vector3 calculates the vector which translates the mousevector to a vector for the player to face in the direction of in 3d space

        lookDirection = lookVector;

       

        // this is in update because I'd rather not have an unresponsive jump
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
            jump = true;
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Dash");
            dash = true;
            // moveDirection += Vector3.Scale(moveDirection.normalized, moveVars.DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * moveVars.Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * moveVars.Drag.z + 1)) / -Time.deltaTime)));
        }
        switch (type)
        {
            
            case ControllerType.Move:
                if (cc.isGrounded)
                {
                    /// Player input is fetched and stored in this variable, and is rotated so forward is relative to the camera.
                    /// Also remember Quaternion-Vector multiplication is not communative.
                    /// In this case Vector = Quaternion * Vector is okay, but Vector = Vector * Quaternion is not.
                    if (moveVars.smoothMotion)
                    {
                        moveDirection = Quaternion.FromToRotation(Vector3.right, Camera.main.transform.right) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    }
                    else
                    {
                        moveDirection = Quaternion.FromToRotation(Vector3.right, Camera.main.transform.right) * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
                    }

                    // multiply moveDirection by speed
                    moveDirection *= moveVars.playerSpeed;

                    if (jump == true)
                    {
                        moveDirection.y = moveVars.jumpStrength;
                        jump = false;
                    }
                    if (dash == true)
                    {
                        // moveDirection;
                    }

                }


                // moveDirection.x /= 1 + moveVars.Drag.x * Time.deltaTime;
                // moveDirection.y /= 1 + moveVars.Drag.y * Time.deltaTime;
                // moveDirection.z /= 1 + moveVars.Drag.z * Time.deltaTime;

                moveDirection.y -= moveVars.gravity * Time.deltaTime;

                break;
        }

        if (Input.GetButtonDown("Fire1") && projectileSpawn)
        {
            shoot = true;
            /*
            Rigidbody temp = Instantiate(projectilePrefab, projectileSpawn.position,
                projectileSpawn.rotation);
            // Shoot projectile
            temp.AddForce(projectileSpawn.forward * 10, ForceMode.Impulse);
            // Destroy projectile after 2.0 seconds
            Destroy(temp.gameObject, 2.0f);
            */
        }

    }

    private void FixedUpdate()
    {
        cc.Move(moveDirection * Time.fixedDeltaTime);
        
        if (shoot)
        {
            RaycastHit hit;
           
            if (Physics.Raycast(projectileSpawn.position, projectileSpawn.forward, out hit, 100 /* change this to Range of weapon eventually */))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }

            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
            shoot = false;
        }
    }

    // If you're wondering why this is FixedUpdate, it's because I don't want my player to jitter.
    void LateUpdate()
    {
      
    }

    void CameraMove()
    {
        if (scriptControlsCamera)
        {
            if (lookDirection != Vector3.zero)
            {
                offset = Quaternion.LookRotation(Camera.main.transform.forward) * new Vector3(0, 0, -cameraDistanceFromPlayer);
                Vector3 mousePos;

                if (Camera.main.orthographic)
                    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
                else if (moveType == CameraMoveType.ParallelToView)
                    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, cameraDistanceFromPlayer)) + offset - Camera.main.transform.position;
                else
                {
                    mousePos = GetWorldPositionOnPlane(Input.mousePosition + new Vector3(0, 0, cameraDistanceFromPlayer), transform.position.y) + offset - Camera.main.transform.position;
                }
                Vector3 cameraPos = transform.position + offset;
                if (cameraOffsetByMouse)
                {
                    // Debug.Log(mousePos);
                    cameraPos += Vector3.ClampMagnitude(mousePos, maxCameraOffset);
                }
                if (cameraSmoothing)
                    cameraPos = Vector3.Lerp(Camera.main.transform.position, cameraPos, smoothSpeed);
                Camera.main.transform.position = cameraPos;

            }
        }
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xz = new Plane(Vector3.up, new Vector3(0, y, 0));
        float distance;
        xz.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

}