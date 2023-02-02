using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float gravityForce;
    public float ySpeed; //vertical speed
    public float jumpForce;
    public float hangTime; //maximun height based on holding the jump
    public float hangTimer;
    public float gravityModifier; //to allow higher jumps
    public float forwardSpeed;
    public float runSpeed;
    public float lerpTime; //time from current to run

    private CharacterController myController;
    
    //Wall Jump
    private Quaternion myRotation; //rotation of the player
    public bool hasJump;
    
    //Coin Pickup
    public int coinScore;
    public TMP_Text coinText;

    // Start is called before the first frame update
    void Start()
    {
        myController = GetComponent<CharacterController>();
        myRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MyGravity();
        Jump();
        ForwardMovement();
        SpeedApply();
    }

    void Update() 
    {
        if (Input.GetButtonUp("Fire1"))
            hasJump = false;
    }

    void MyGravity()
    {
        ySpeed = myController.velocity.y;
        ySpeed -= gravityForce * Time.deltaTime;
    }

    void Jump()
    {
        if (Input.GetButton("Fire1"))
        {
            if (myController.isGrounded && !hasJump)
            {
                hasJump = true;
                hangTimer = hangTime;
                ySpeed = jumpForce;
            }
            else
            {
                if (hangTimer > 0)
                {
                    hangTimer -= Time.deltaTime;
                    ySpeed += gravityModifier * hangTimer * Time.deltaTime;
                }
            }
        }
    }

    void ForwardMovement()
    {
        if (myController.isGrounded)
        {
            if (forwardSpeed <= runSpeed - 0.1f || forwardSpeed >= runSpeed + 0.1f)
                forwardSpeed = Mathf.Lerp(forwardSpeed, runSpeed, lerpTime);
            else
                forwardSpeed = runSpeed;
        }
    }

    void SpeedApply()
    {
        myController.Move(transform.forward * forwardSpeed * Time.deltaTime);
        myController.Move(new Vector3(0f, ySpeed, 0f) * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        //had to change Character Controller Skin Width to 0.0001 to fix isGrounded
        GroundLanding();
        WallJump(hit);
    }

    void GroundLanding()
    {        
        if (myRotation != transform.rotation && myController.isGrounded)
        {
            transform.rotation = myRotation;
        }
    }

    void WallJump(ControllerColliderHit hitSent)    
    {
        if (!myController.isGrounded && hitSent.normal.y < 0.1f && hitSent.normal.y > -0.1f)
        {
            if (Input.GetButton("Fire1") && !hasJump)
            {
                hasJump = true;
                transform.forward = hitSent.normal;
                transform.rotation = Quaternion.Euler(new Vector3(0,transform.rotation.eulerAngles.y, 0));
                forwardSpeed = runSpeed;
                hangTimer = hangTime;
                ySpeed = jumpForce;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform.tag == "Coin")
        {
            coinScore++;
            coinText.text = coinScore.ToString();
            Destroy(other.gameObject);
        }        
    }
}
