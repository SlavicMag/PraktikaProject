using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
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
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
           return;
        }
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }
} 
                                                   