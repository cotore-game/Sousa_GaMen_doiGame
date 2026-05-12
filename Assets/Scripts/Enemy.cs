using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Header("MoveSpeed")]
    private float MoveSpeed;
    [SerializeField, Header("AttackPower")]
    private int AttackPower;
    private Rigidbody2D rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        rigid.linearVelocity = new Vector2(Vector2.left.x * MoveSpeed, rigid.linearVelocityY);
    }

    private void PlayerDamage(PlayerConnectionInitiateMode player)
    {
        
    }
}
