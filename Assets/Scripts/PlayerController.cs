using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Animator animator;

    [Header("Property")]
    [SerializeField] float movePower;
    [SerializeField] float brakePower;
    [SerializeField] float maxXSpeed;
    [SerializeField] float maxYSpeed;
    [SerializeField] float jumpSpeed;

    [SerializeField] LayerMask groundCheck;

    Vector2 moveDir;
    bool isGround;

    Collider2D myColl;
    Collider2D coll;

    private void Awake()
    {
        myColl = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (moveDir.x < 0 && rigid.velocity.x > -maxXSpeed)
        {
            rigid.AddForce(Vector2.right * moveDir.x * movePower);
        }
        else if (moveDir.x > 0 && rigid.velocity.x < maxXSpeed)
        {
            rigid.AddForce(Vector2.right * moveDir.x * movePower);
        }
        else if (moveDir.x == 0 && rigid.velocity.x > 0.1f)
        {
            rigid.AddForce(Vector2.left * brakePower);
        }
        else if (moveDir.x == 0 && rigid.velocity.x < -0.1f)
        {
            rigid.AddForce(Vector2.right * brakePower);
        }

        if (rigid.velocity.y < -maxYSpeed)
        {
            Vector2 velocity = rigid.velocity;
            velocity.y = -maxYSpeed;
            rigid.velocity = velocity;
        }

        animator.SetFloat("YSpeed", rigid.velocity.y);

        if (transform.position.y < -30)
        {
            transform.position = new Vector2(-6.4f, -1.5f);
        }
    }

    void Jump()
    {
        Vector2 velocity = rigid.velocity;
        velocity.y = jumpSpeed;
        rigid.velocity = velocity;
    }

    void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();

        if (moveDir.x < 0)
        {
            render.flipX = true;
            animator.SetBool("Run", true);
        }
        else if (moveDir.x > 0)
        {
            render.flipX = false;
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if(moveDir.y < 0)
        {
            animator.SetBool("Crouch", true);
        }
        else
        {
            animator.SetBool("Crouch", false);
        }
    }

    void OnJump(InputValue value)
    {
        if (moveDir.y < 0)
        {
            StartCoroutine(DownJump());
            return;
        }

        if (value.isPressed && isGround)
        {
            Jump();
        }
    }

    IEnumerator DownJump()
    {
        Collider2D platform = coll;
        Physics2D.IgnoreCollision(myColl, platform);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(myColl, platform, false);
    }

    private int groundCount;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundCheck) != 0)
        {
            coll = collision.gameObject.GetComponent<Collider2D>();
            groundCount++;
            isGround = groundCount > 0;
            animator.SetBool("IsGround", isGround);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (groundCheck.Contain(collision.gameObject.layer))
        {
            groundCount--;
            isGround = groundCount > 0;
            animator.SetBool("IsGround", isGround);
        }
    }
}
