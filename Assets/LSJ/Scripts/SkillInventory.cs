using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInventory : MonoBehaviour
{
    #region Singleton
    private static SkillInventory instance;
    public static SkillInventory Instance
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
            //DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }
    #endregion

    #region Skile
    public List<Skill> skills;

    public void AddSkill(Skill skill)
    {
        skills.Add(skill);
        skill.gameObject.SetActive(false); // 스킬 비활성화
        skill.transform.parent = transform; // 스킬 부모를 인벤토리로 설정
    }

    public void RemoveSkill(Skill skill)
    {
        skills.Remove(skill);
    }

    //public void UseSkill(Skill skill)
    //{
    //    skill.Use(); // 스킬 사용
    //}
    #endregion


}
