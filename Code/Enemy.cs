using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float mon_health;
    public float max_mon_health;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter; //x축 뒤집기

    WaitForFixedUpdate wait; //다음 업데이트까지 대기

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    // Update is called once per frame
    void FixedUpdate() //몬스터가 플레이어를 추적하는 업데이트
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") )
            return;

        if (!GameManager.instance.isLive)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero; 
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true; // coll 활성화? ㄴㄴ 
        rigid.simulated = true; //물리시뮬할거임? ㄴㄴ
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);

        mon_health = max_mon_health;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.mon_speed;
        max_mon_health = data.mon_health;
        mon_health = data.mon_health;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        mon_health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (mon_health > 0)
        {
            anim.SetTrigger("Hit");

        }
        else
        {
            isLive = false;
            coll.enabled = false; // coll 활성화? ㄴㄴ 
            rigid.simulated = false; //물리시뮬할거임? ㄴㄴ
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }
    IEnumerator KnockBack()
    {
        //yield return null; // null = 1프레임 쉬기
        //yield return new WaitForSeconds(2f); //2초 쉬기
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; //플레이어 반대방향으로 넉백
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }
}

