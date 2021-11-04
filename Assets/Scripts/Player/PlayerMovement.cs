using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;
    float camRayLength = 100f;

#if UNITY_ANDROID
    public Canvas joystickCanvas;
    public VariableJoystick vjMovement;
    public VariableJoystick vjRotation;
    public bool canFire = false;
#endif

    void Awake()
    {

        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
#if UNITY_ANDROID
        joystickCanvas.gameObject.SetActive(true);
#endif
    }

    private void FixedUpdate()
    {
#if UNITY_ANDROID
        float h = vjMovement.Horizontal;
        float v = vjMovement.Vertical;
#else
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
#endif

        Move(h, v);
        Turning();
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {

#if UNITY_ANDROID

        Vector3 moveVector = (Vector3.up * vjRotation.Horizontal + Vector3.left * vjRotation.Vertical);

        if (vjRotation.Horizontal != 0 || vjRotation.Vertical != 0)
        {
            transform.eulerAngles = new Vector3(0, Mathf.Atan2(vjRotation.Horizontal, vjRotation.Vertical) * 180 / Mathf.PI, 0);
            canFire = true;
        }
        else
        {
            canFire = false;
        }
#else
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
#endif
    }

    void Animating(float h, float v)
    {
        bool isWalking = h != 0f || v != 0f;
        anim.SetBool("IsWalking", isWalking);
    }
}