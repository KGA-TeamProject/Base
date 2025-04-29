using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;

    private int hp;

    private int playerHP
    {
        // get으로 PlayerHealthBar 이외에 접근하지 못하도록 설정
        get => hp;
        // 최소 / 최대값 설정으로 int가 0의 범위를 넘지 않도록 설정
        set => hp = Mathf.Clamp(value, 0, hp);
    }


}
