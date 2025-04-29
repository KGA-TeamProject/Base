using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // 인벤토리 클래스
    // 인벤토리 클래스는 플레이어의 아이템 목록을 관리합니다.
    // 아이템 추가, 제거, 사용 등의 기능을 제공합니다.

    #region Singleton
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                Inventory prefab = Resources.Load<Inventory>("Inventory"); // 인벤토리 로드
                Instantiate(prefab); // 인스턴스 생성
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // 인스턴스 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }
    #endregion

    public List<Item> items;

    // 아이템 추가
    public void AddItem(Item item)
    {
        // 아이템 추가 로직
        items.Add(item); // 아이템 추가
        item.gameObject.SetActive(false); // 아이템 비활성화
        item.transform.parent = transform; // 아이템 부모를 인벤토리로 설정
    }

    // 아이템 사용
    public void UseItem(Item item)
    {
        // 아이템 사용 로직
        item.Use(); // 아이템 사용
        RemoveItem(item); // 아이템 제거
    }
    public void UseItem(int index)
    {
        // 아이템 사용 로직
        Item item = items[index]; // 사용하려는 아이템
        items.RemoveAt(index); // 아이템 목록에서 제거
        item.Use(); // 아이템 사용
        Destroy(item.gameObject); // 아이템 오브젝트 파괴

    }

    // 아이템 드롭 로직
    public void DropItem(Item item)
    {        
        items.Remove(item); // 아이템 목록에서 제거
        item.gameObject.SetActive(true); // 아이템 활성화
        item.transform.parent = null; // 아이템 부모를 null로 설정
    }

    public void DropItem(int index)
    {
        Item item = items[index]; // 드롭할 아이템
        items.RemoveAt(index); // 아이템 목록에서 제거
        item.gameObject.SetActive(true); // 아이템 활성화
        item.transform.parent = null; // 아이템 부모를 null로 설정
    }


    // 아이템 제거
    public void RemoveItem(Item item)
    {
        // 아이템 제거 로직
        items.Remove(item); // 아이템 제거
    }
    

    // 아이템 목록 반환
    //public List<Item> GetItems()
    //{
    //    return items; // 아이템 목록 반환
    //}

}
