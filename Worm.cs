using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class Worm : Entity
{
    float[] arrayX = new float[] { -44.11f, -34.209f, -24.365f, -39.223f, -29.348f, -44.123f, -34.22f, -24.29f, -39.239f, -29.26f};
    float[] arrayY = new float[] { 13.449f, 13.457f, 13.45f, 10.596f, 10.599f, 7.741f, 7.741f, 7.741f, 4.88f, 4.88f};

    int a;

    private Animator anim;

    [SerializeField] private int RandChar;

    public static Worm Instance
    {
        get;
        set;
    }

    private States State
    {
        get { return (States)anim.GetInteger("State"); }
        set { anim.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        lives = 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && lives > 1)
        {
            Hero.Instance.GetDamage();
            GetDamage();
        }
        else if (collision.gameObject == Hero.Instance.gameObject && lives == 1)
        {
            Hero.Instance.GetDamage();
        }
    }

    public override void GetDamage()
    {
        State = States.wizzard_disappearance;
        lives--;
        if (lives == 0)
            Die();
        StartCoroutine(Teleport());
    }

    public void Randomazer()
    {
        RandChar = Random.Range(1, 10);
    }

    private IEnumerator Teleport()
    {
        yield return new WaitForSeconds(0.2f);
        while (a == RandChar)
            Randomazer();
        transform.position = new Vector2(arrayX[RandChar], arrayY[RandChar]);
        a = RandChar;
        State = States.wizzard_appearance;
        yield return new WaitForSeconds(0.2f);
        State = States.wizard_idle;
    }

    public enum States
    {
        wizard_idle,
        wizzard_disappearance,
        wizzard_appearance
    }
}