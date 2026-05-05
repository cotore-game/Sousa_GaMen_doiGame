using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField,Header("MoveSpeed")]
    private float MoveSpeed;
    private Vector2 inputDirection;
    private Rigidbody2D rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
        rigid.linearVelocity = new Vector2(inputDirection.x * MoveSpeed, rigid.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }
}
