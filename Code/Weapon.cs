using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count; //근접 무기 개수
    public float speed; //회전속도

    float timer;
    Player player;

     void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); //무기 회전
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

    }

    public void LevelUP(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
            batch();

        player.BroadcastMessage("ApplyGear");
    }

    public void Init(ItemData data)
    {
        // basic set
        name = "Weapon " + data.itemID;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // property set
        id = data.itemID;
        damage = data.baseDamage;
        count = data.baseCount;

        for (int index=0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if(data.projecttile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150; //시계방향 회전
                batch();
                break;

            default:
                speed = 0.3f; //연사속도, 적을수록 많이 쏨 150이면 150초에 한 번
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void batch() //무기 배치
    {
        for (int index=0; index < count; index++)
        {
            //Transform bullet = GameManager.instance.pool.Get(prefabId).transform; //부모를 풀매니저에서 플레이어로 바꾸기 위함 
            //bullet.parent = transform;
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity; //이동시 플레이어 위치를 기준으로 생성하기 위함

            Vector3 rotVec = Vector3.forward * 360 * index / count; //무기 회전
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);


            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // -1 => 무한한 관통, -1 is infinity per.
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); //총알 나가는 방향
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
