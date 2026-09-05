using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUP : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }   

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    public void select(int index)
    {
        items[index].Onclick();
    }

    void Next()
    {
        // (1) 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // (2) 그 중 랜덤하게 3개 아이템 활성화
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);


            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }

        // (3) 만렙이면 소비아이템으로 대체
        for (int index=0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true); //소비아이템 [4] 번째 인덱스
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
            
         }
        
    }
}
