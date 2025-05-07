using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    #region Singleton
    private static SkillContainer instance;
    public static SkillContainer Instance
    {
        get
        {
            if (instance == null)
            {
                SkillContainer prefab = Resources.Load<SkillContainer>("SkillContainer"); // 인벤토리 로드
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
            //DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }
    #endregion

    #region Skill
    public List<Skill> skills = new List<Skill>(); // 아이템 목록

    // 추가
    public void AddSkill(Skill skill)
    {
        // 추가 로직
        skills.Add(skill); // 추가
        skill.gameObject.SetActive(false); // 비활성화
        skill.transform.parent = transform; // 부모를 인벤토리로 설정
    }

    //public void AddPotion(GameObject potionPrefab)
    //{
    //    GameObject newPotion = Instantiate(potionPrefab);
    //    // 인벤토리 UI에 연결 등 추가 작업
    //}


    // 드롭 로직
    public void DropSkill(Skill skill)
    {
        skills.Remove(skill); // 아이템 목록에서 제거
        skill.gameObject.SetActive(true); // 아이템 활성화
        skill.transform.parent = null; // 아이템 부모를 null로 설정
    }

    public void DropSkill(int index)
    {
        Skill skill = skills[index]; // 드롭할 아이템
        skills.RemoveAt(index); // 아이템 목록에서 제거
        skill.gameObject.SetActive(true); // 아이템 활성화
        skill.transform.parent = null; // 아이템 부모를 null로 설정
    }


    // 제거
    public void RemoveSkill(Skill skill)
    {
        // 아이템 제거 로직
        skills.Remove(skill); // 아이템 제거
    }


    // 목록 반환
    //public List<Skill> GetItems()
    //{
    //    return skills; // 목록 반환
    //}
    #endregion
}
