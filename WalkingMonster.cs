using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class WalkingMonster : Entity
{
    private float speed = 0.8f;
    private Vector3 dir;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Animator anim2;
    private bool isAttack = false;
    private int selectionNumber = 0;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public static WalkingMonster Instance
    {
        get;
        set;
    }

    private States State
    {
        get { return (States)anim2.GetInteger("State"); }
        set { anim2.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        Instance = this;
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim2 = GetComponent<Animator>();
        isRecharged = true;
    }

    private void Start()
    {
        dir = transform.right;
        lives = 5;
    }

    private void Update()
    {
        if (isAttack)
            Attack();
        else
            Move();
    }

    private void Attack()
    {
        if (isRecharged)
        {
            State = States.attack_monstr;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }

    private void Move()
    {
        State = States.run_monstr;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.05f);
        
        if (selectionNumber == 0 && (Hero.Instance.transform.position.x - gameObject.transform.position.x > attackRange || gameObject.transform.position.x - Hero.Instance.transform.position.x > attackRange))
        {
            if (colliders.Length > 0) dir *= -1f;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * speed);
            sprite.flipX = dir.x < 0.0f;
        }
        if (selectionNumber == 0 && (Hero.Instance.transform.position.x - gameObject.transform.position.x <= attackRange * 0.95f || gameObject.transform.position.x - Hero.Instance.transform.position.x <= attackRange * 0.95f))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * speed * 1.5f);
        }
        else if (selectionNumber == 1 && (Hero.Instance.transform.position.x - gameObject.transform.position.x <= attackRange + 1f || gameObject.transform.position.x - Hero.Instance.transform.position.x <= attackRange + 1f))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * speed);
            Update();
        }
        else if (selectionNumber == 2 && (Hero.Instance.transform.position.x - gameObject.transform.position.x <= attackRange + 1f || gameObject.transform.position.x - Hero.Instance.transform.position.x <= attackRange + 1f))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * speed);
            Update();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void ConstraintXY() 
    {
        if (dir.x > 0.0f && (selectionNumber == 1 || selectionNumber == 2) && Hero.Instance.transform.position.x - gameObject.transform.position.x <= attackRange && Hero.Instance.transform.position.y - gameObject.transform.position.y <= attackRange)
        {
            Hero.Instance.GetDamage();
        }
        else if (dir.x < 0.0f && (selectionNumber == 1 || selectionNumber == 2) && gameObject.transform.position.x - Hero.Instance.transform.position.x <= attackRange && gameObject.transform.position.y - Hero.Instance.transform.position.y <= attackRange)
        {
            Hero.Instance.GetDamage();
        }
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;
        ConstraintXY();
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(1.5f);
        isRecharged = true;
        isAttack = false;
        OnCollisionEnter2D();
    }

    private void Suspect()
    {
        State = States.idle_monstr;
        dir *= -1f;
        sprite.flipX = dir.x < 0.0f;
        isAttack = true;
        Update();
    }

    public void OnCollisionEnter2D()
    {
        if (gameObject.transform.position.x >= Hero.Instance.transform.position.x && gameObject.transform.position.x - Hero.Instance.transform.position.x <= attackRange)
        {
            if (dir.x > 0.0f)
            {
                selectionNumber = 2;
                Suspect();
            }
            else
            {
                selectionNumber = 1;
                isAttack = true;
                Update();
            }
        }
        else if (gameObject.transform.position.x <= Hero.Instance.transform.position.x && Hero.Instance.transform.position.x - gameObject.transform.position.x <= attackRange)
        {
            if (dir.x < 0.0f)
            {
                selectionNumber = 2;
                Suspect();
            }
            else
            {
                selectionNumber = 1;
                isAttack = true;
                Update();
            }
        }
        else
        {
            isAttack = false;
            selectionNumber = 0;
        }

        if (lives < 1)
            Die();
    }

    public enum States
    {
        run_monstr,
        attack_monstr,
        idle_monstr
    }
}