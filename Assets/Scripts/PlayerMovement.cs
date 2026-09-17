using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float bounceForce = 10f;   
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()                                                                                         
    { 
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("isRunning", horizontalInput != 0 && isGrounded);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");

        }
    }

    public void BounceUp()
    {
        if (rb == null) return;

        rb.velocity = new Vector2(rb.velocity.x, bounceForce);
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
           return;
        }
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
} 
                                                   