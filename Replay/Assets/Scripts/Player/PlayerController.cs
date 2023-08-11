using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("Movement")]
    [SerializeField] float speed = 1f;
    float xInput = 0;

    [Header("Jump")]
    [SerializeField] float jumpCut = 1f;
    [SerializeField] float fallMultiplier;
    [SerializeField] float jumpTime;
    [SerializeField] float jumpForce = 10f;
    float jumpCounter;
    bool isJumping;

    [Header("Controls")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayers;

    private Recorder recorder;
    private bool recording = false;

    private void Awake()
    {
        recorder = GetComponent<Recorder>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

   

    private void Move()
    {
        xInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            isJumping = true;
            jumpCounter = 0;
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        }

        if(Input.GetButton("Jump") && isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCounter += Time.deltaTime;
            if(jumpCounter > jumpTime)
            {
                isJumping = false;
            }
        }

        if(Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y / jumpCut);
            isJumping = false;
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity -= new Vector2(rb.velocity.x, fallMultiplier) * Time.deltaTime;
        }

    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayers);
    }
}