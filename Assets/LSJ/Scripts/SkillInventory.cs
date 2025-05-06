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
                Inventory prefab = Resources.Load<Inventory>("Inventory"); // �κ��丮 �ε�
                Instantiate(prefab); // �ν��Ͻ� ����
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // �ν��Ͻ� ����
            //DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }
    #endregion

    #region Skile
    public List<Skill> skills;

    public void AddSkill(Skill skill)
    {
        skills.Add(skill);
        skill.gameObject.SetActive(false); // ��ų ��Ȱ��ȭ
        skill.transform.parent = transform; // ��ų �θ� �κ��丮�� ����
    }

    public void RemoveSkill(Skill skill)
    {
        skills.Remove(skill);
    }

    //public void UseSkill(Skill skill)
    //{
    //    skill.Use(); // ��ų ���
    //}
    #endregion


}
