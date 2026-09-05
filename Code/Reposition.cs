using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    [Header("맵 규격 설정")]
    [Tooltip("타일맵 1개의 가로/세로 크기 (예: 20, 32, 40)")]
    public float tileSize = 32f; // 원하시는 타일맵 크기로 인스펙터에서 수정 가능

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
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    // tileSize * 2 로 재배치 거리 자동 계산
                    transform.Translate(Vector3.right * dirX * (tileSize * 2f));
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * (tileSize * 2f));
                }
                break;

            case "Enemy": // 몬스터가 영역을 벗어나면 플레이어 이동 방향 전방에 재배치
                if (coll.enabled) // 몬스터가 살아있는가?
                {
                    // 맵이 커지면 몬스터 재배치 거리(기존 20)도 tileSize 수준으로 맞춰주는 것이 좋습니다.
                    transform.Translate(playerDir * tileSize + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                }
                break;
        }
    }
}