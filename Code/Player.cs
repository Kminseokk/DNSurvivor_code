using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//함수 인식 제대로 안되면, Edit > preferences > external tools 에서 비쥬얼스튜디오 연결
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    SpriteRenderer spriter;
    Animator anim;


    Rigidbody2D rigid; 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>(); //스크립트도 컴포넌트 취급
    }

    //void Update()
    //{
    //    if (!GameManager.instance.isLive)
    //        return;

    //    inputVec.x = Input.GetAxisRaw("Horizontal");
    //    inputVec.y = Input.GetAxisRaw("Vertical");
    //}


    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime; // normalized = 대각선 이동 역시 1로 값 조정하기 위함. deltatime은 프레임마다 이동속도 고정
        rigid.MovePosition(rigid.position + nextVec); // 다음에 나아가야하는 위치이기에 현재위치 + 입력 값
        
    }

    void OnMove(InputValue value)
    {
        //if (!GameManager.instance.isLive)
        //    return;

        inputVec = value.Get<Vector2>();
    }


    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude); //magnitude = 순수한 크기, 값을 스피드에 넣는다.
        

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; 
        }
    }

    void OnCollisionStay2D(Collision2D collision) //플레이어 피격
    {
        if (!GameManager.instance.isLive)
            return;

        // 게임 시작 후 경과 시간(초)
        float elapsedTime = Time.timeSinceLevelLoad;
        float currentDamage = 10f + (Mathf.Floor(elapsedTime / 60f) * 10f);

        GameManager.instance.user_health -= currentDamage * Time.deltaTime;

        if (GameManager.instance.user_health < 0)
        {
            for (int index=2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");

            GameManager.instance.GameOver();
        }

    }
}
