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
                SkillContainer prefab = Resources.Load<SkillContainer>("SkillContainer"); // �κ��丮 �ε�
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

    #region Skill
    public List<Skill> skills = new List<Skill>(); // ������ ���

    // �߰�
    public void AddSkill(Skill skill)
    {
        // �߰� ����
        skills.Add(skill); // �߰�
        skill.gameObject.SetActive(false); // ��Ȱ��ȭ
        skill.transform.parent = transform; // �θ� �κ��丮�� ����
    }

    //public void AddPotion(GameObject potionPrefab)
    //{
    //    GameObject newPotion = Instantiate(potionPrefab);
    //    // �κ��丮 UI�� ���� �� �߰� �۾�
    //}


    // ��� ����
    public void DropSkill(Skill skill)
    {
        skills.Remove(skill); // ������ ��Ͽ��� ����
        skill.gameObject.SetActive(true); // ������ Ȱ��ȭ
        skill.transform.parent = null; // ������ �θ� null�� ����
    }

    public void DropSkill(int index)
    {
        Skill skill = skills[index]; // ����� ������
        skills.RemoveAt(index); // ������ ��Ͽ��� ����
        skill.gameObject.SetActive(true); // ������ Ȱ��ȭ
        skill.transform.parent = null; // ������ �θ� null�� ����
    }


    // ����
    public void RemoveSkill(Skill skill)
    {
        // ������ ���� ����
        skills.Remove(skill); // ������ ����
    }


    // ��� ��ȯ
    //public List<Skill> GetItems()
    //{
    //    return skills; // ��� ��ȯ
    //}
    #endregion
}
