using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject boat;
    private Vector3 boatPosition;

    public CharacterController charController;
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6f;
    public float gravity = -9.18f;
    public float jumpHeight = .5f;
    private float speed;


    private float x;
    private float z;
    private Vector3 velocity;
    private Vector3 moveDistance;

    private static Animator anim;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool isEnabled = true;
  
    void Start()
    {
        anim = GetComponent<Animator>();
        boatPosition = boat.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        //following sets acceleration to 0 when ground is below feet
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log("is grounded? " + isGrounded);
        if (isGrounded && velocity.y < 0) {
            velocity.y = -1f;
        }


        //moves player
        Move();

        //jumps if you are on ground
        if (Input.GetButtonDown("Jump") && isGrounded) {
            anim.SetTrigger("isJumping");
            StartCoroutine(jump());
        }

        //calculate gravity
        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
        
    }

    public void SetIsActive(bool enabled) {
        isEnabled = enabled;
    }

    IEnumerator jump() {
        yield return new WaitForSeconds(.2f);
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Move()
    {
        if (isEnabled) {
            //gets x z movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //makes vector adding amount of x and z input applied by respective unit vector
            moveDistance = transform.right * x / 4 + transform.forward * z;
            anim.SetFloat("runSpeed", z);

            if (Input.GetKey(KeyCode.LeftShift)) {
                speed = sprintSpeed;
            }
            else {
                speed = walkSpeed;
            }

            //moves object
            charController.Move(moveDistance * speed * Time.deltaTime);
        }

        //charController.Move(boat.transform.position - boatPosition);
        boatPosition = boat.transform.position;

        //used to stop charControllerFromGlitching into floor
        //charController.Move(transform.up * .1f * Time.deltaTime);
    }

}
