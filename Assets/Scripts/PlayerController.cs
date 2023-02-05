using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Movement
    private CharacterController myController;
    public float gravityForce = 9.8f;
    public float xSpeed = 5f;
    public float ySpeed;
    public bool moveRight;
    public bool moveLeft;

    //Jump
    public float jumpForce = 3f;
    public float hangTime = 0.5f; //maximun height based on holding the jump
    public float gravityModifier = 50f; //to allow higher jumps
    public float hangTimer;
    public bool hasJump = false;

    //Animation
    public Animator myAnimator;
    private const string ANIM_XSPEED = "XSpeed";
    private const string ANIM_YSPEED = "YSpeed";
    private const string ANIM_ISGROUNDED = "IsGrounded";
    private const string ANIM_ISWALLED = "IsWalled";

    //GameOver
    public static bool isAlive;
    public GameObject gameOverText;

    //Head Block
    public ParticleSystem lavaBlockParticle;

    private void Start()
    {
        isAlive = true;
        myController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump"))
            hasJump = false;

        myAnimator.SetBool(ANIM_ISGROUNDED, myController.isGrounded);
    }

    void FixedUpdate()
    {
        MyGravity();
        Jump();
        GetMovement();
        Turn();
        SpeedApply();
    }

    void MyGravity()
    {
        ySpeed = myController.velocity.y;
        ySpeed -= gravityForce * Time.deltaTime;
    }

    void Jump()
    {
        if (Input.GetButton("Fire1") || Input.GetButton("Jump"))
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
    private void GetMovement()
    {
        moveRight = Input.GetKey(KeyCode.D);
        moveLeft = Input.GetKey(KeyCode.A);

        if (moveRight || moveLeft)
            xSpeed = 5f;
        else
            xSpeed = 0f;
    }

    private void Turn()
    {
        if (moveRight && moveLeft)
            return;

        if (moveRight)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        if (moveLeft)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

    }

    void SpeedApply()
    {
        myController.Move(transform.forward * xSpeed * Time.deltaTime);
        myAnimator.SetFloat(ANIM_XSPEED, myController.velocity.x);

        myController.Move(new Vector3(0f, ySpeed, 0f) * Time.deltaTime);
        myAnimator.SetFloat(ANIM_YSPEED, myController.velocity.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.transform.tag == "Coin")
        //{
        //    coinScore++;
        //    coinText.text = coinScore.ToString();
        //    Destroy(other.gameObject);

        //}

        if (other.transform.CompareTag("DeathZone"))
        {
            isAlive = false;
            gameOverText.SetActive(true);
            StartCoroutine(Death());
        }

        if (other.transform.CompareTag("Block"))
        {
            lavaBlockParticle.Play();
        }

        if (other.transform.CompareTag("Exit"))
        {
            StartCoroutine(Exit());
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
