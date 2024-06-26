using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Health,         // 체력 관련
    Speed,          // 이동속도 관련
    Mental,         // 정신력 관련
    All,            // 모든 스탯 관련
}

/// <summary>
/// 플레이어가 사용할 수 있는 소모성 아이템관련 클래스
/// </summary>
public class ConsumableItem : BaseItem
{
    [Header("Item Parameters")]
    public EffectType effectType;   // 아이템 효과 타입

    public override void UseItem()
    {
        switch (effectType)
        {
            case EffectType.Health:

                break;
            case EffectType.Speed:

                break;
            case EffectType.Mental:

                break;
            case EffectType.All:
                
                break;
        }
    }

}