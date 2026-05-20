using NUnit.Framework;
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
    private Rigidbody2D rigid;
    private bool bJump;
    private bool isFacingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        bJump = false;
        isFacingRight = Mathf.Approximately(transform.eulerAngles.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurn();
        Move();
        Jump();
        Debug.Log(hp);
        HitFloor();
    }

    private void Move()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float speedX = 0.0f;
        if (isFacingRight && keyboard.wKey.isPressed && keyboard.rKey.isPressed)
        {
            speedX = MoveSpeed;
        }
        else if (!isFacingRight && keyboard.wKey.isPressed && keyboard.lKey.isPressed)
        {
            speedX = -MoveSpeed;
        }

        rigid.linearVelocity = new Vector2(speedX, rigid.linearVelocity.y);
    }

    private void Jump()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!bJump && keyboard.jKey.wasReleasedThisFrame)
        {
            rigid.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            bJump = true;
        }
    }

    private void UpdateTurn()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.hKey.isPressed)
        {
            if (isFacingRight && keyboard.lKey.wasPressedThisFrame)
            {
                isFacingRight = false;
                transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            }
            else if (!isFacingRight && keyboard.rKey.wasPressedThisFrame)
            {
                isFacingRight = true;
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
       // if(collision.gameObject.tag == "Enemy")
        //{
        //  HitEnemy(collision.gameObject);
        //}
    }

    private void HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f);
        Vector3 raySize = new Vector3(transform.lossyScale.x -0.1f, 0.1f);
        RaycastHit2D rayHit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if(rayHit.transform == null)
        {
            bJump = true;
            return;        
        }

        if(rayHit.transform.tag == "Floor" && bJump)
        {
            bJump = false;
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

    private void OnBecameInvisible()
    {
        Camera camera = Camera.main;
        if (camera.name == "Main Camera" && camera.transform.position.y > transform.position.y)
        {
            Destroy(gameObject);
        }
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
