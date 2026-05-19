using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField,Header("MoveSpeed")]
    private float MoveSpeed;
    [SerializeField,Header("JumpSpeed")]
    private float JumpSpeed;
    [SerializeField, Header("HitPoint")]
    private int hp;
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
        Debug.Log(hp);
        LookMoveDirec();
    }

    private void Move()
    {
        rigid.linearVelocity = new Vector2(inputDirection.x * MoveSpeed, rigid.linearVelocity.y);
    }

    private void LookMoveDirec()
    {
        if(inputDirection.x > 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if(inputDirection.x < 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            bJump = false;
        }
        if(collision.gameObject.tag == "Enemy")
        {
            HitEnemy(collision.gameObject);
        }
    }

    private void HitEnemy(GameObject enemy)
    {
        float halfScaleY = transform.lossyScale.y / 2.0f;
        float enemyHalfScaleY = enemy.transform.lossyScale.y /2.0f;
        if(transform.position.y - (halfScaleY -0.1f) >= enemy.transform.position.y + (enemyHalfScaleY - 0.1f))
        {
            Destroy(enemy);
        }
        else
        {
          enemy.GetComponent<Enemy>().PlayerDamage(this);
        }
    }

    private void Dead()
    {
        if(hp <=0)
        {
            Destroy(gameObject);
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

    public void Damege(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);
        Dead();
    }

    public int GetHP()
    {
        return hp;
    }
}
