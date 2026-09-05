using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) // 에리어 태그가 아니라면
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;


        switch (transform.tag)
        {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;

                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1; //방향

                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 70);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 70);
                }
                break;

            case "Enemy": // 몬스터가 영역을 벗어나면 플레이어 이동 방향 전방에 재배치
                if (coll.enabled) // 몬스터가 살아있는가?
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);

                    transform.Translate(ran + dist * 2);
                    //transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f),Random.Range(-3f,3f),0f ));
                }
                break;
        }
    }
}