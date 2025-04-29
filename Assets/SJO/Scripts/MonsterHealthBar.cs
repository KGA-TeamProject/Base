using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;

    private int hp;

    public int monsterHP
    {
        get => hp;

        private set => hp = Mathf.Clamp(value, 0, hp);
    }

    private void Awake()
    {
        hp = 100;
        SetMaxHealth(hp);
    }

    public void SetMaxHealth(int monsterHealth)
    {
        // hpBar.maxValue = monsterHealth;
        hpBar.value = monsterHealth;
    }

    public void TakenDamage(int damage)
    {
        int damageTaken = monsterHP - damage;
        monsterHP = damageTaken;
        hpBar.value = monsterHP;
    }
}
