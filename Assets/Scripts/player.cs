using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Normal")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpV_instant;
    private int facingDir = 1;
    private bool facingRight = true;


    [Header("Collision info")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded;

    [Header("Backpack")]
    [SerializeField] private int backpackCapacity = 4;
    [SerializeField] private int woodCount = 4;
    public Wood woodData;
    private List<Wood> woodBackpack;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        woodBackpack = new List<Wood>(backpackCapacity);
        for (int i = 0; i < woodCount; i++)
        {
            woodBackpack.Add(woodData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveUnderControl(); 
        GroundCheck();
        CheckInputToJump();

        FlipController();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }

    private void MoveUnderControl()
    {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * (6 - woodCount), rb.velocity.y);
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void CheckInputToJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpV_instant * (6 - woodCount));
            }
        }
    }
    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDir = -facingDir;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public bool AddWood(Wood wood)
    {
        if (woodBackpack == null)
            woodBackpack = new List<Wood>(backpackCapacity);

        if (woodBackpack.Count >= backpackCapacity)
            return false;

        woodBackpack.Add(wood);
        woodCount = Mathf.Min(backpackCapacity, woodCount + 1);
        return true;
    }

    public bool PickWood()
    {
        if (woodBackpack == null || woodBackpack.Count == 0)
            return false;

        // remove the last wood in the backpack
        woodBackpack.RemoveAt(woodBackpack.Count - 1);
        woodCount = Mathf.Max(0, woodCount - 1);
        return true;
    }

    public int GetWoodCount()
    {
        return woodBackpack != null ? woodBackpack.Count : 0;
    }

    public bool IsBackpackFull()
    {
        return GetWoodCount() >= backpackCapacity;
    }
}
