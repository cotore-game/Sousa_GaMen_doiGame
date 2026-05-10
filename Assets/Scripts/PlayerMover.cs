using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField,Header("MoveSpeed")]
    private float MoveSpeed;
    [SerializeField,Header("JumpSpeed")]
    private float JumpSpeed;
    private Vector2 inputDirection;
    private Rigidbody2D rigid;
    private bool bJump;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        bJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        rigid.linearVelocity = new Vector2(inputDirection.x * MoveSpeed, rigid.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            bJump = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || bJump) return;

        rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
        bJump = true;
    }
}
