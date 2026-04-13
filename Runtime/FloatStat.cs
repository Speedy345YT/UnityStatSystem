using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[InlineProperty]
[HideDuplicateReferenceBox]
public class FloatStat : IStat<float>
{
    public float baseValue;
    public List<StatModifier<float>> modifiers = new();

    public float cachedValue;
    bool isDirty = true;
    public FloatStat(float baseValue)
    {
        this.baseValue = baseValue;
    }

    public StatModifier<float> AddModifier(StatModifier<float> mod)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing == null)
        {
            modifiers.Add(mod);
        }

        isDirty = true;
        return mod;
    }
    public StatModifier<float> AddModifier(float value, StatModifierType type)
    {
        var mod = new StatModifier<float>(value, type);
        var existing = modifiers.Find(m => m == mod);

        if (existing == null)
        {
            modifiers.Add(mod);
        }

        isDirty = true;
        return mod;
    }
    public void RemoveModifier(StatModifier<float> mod)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing != null)
        {
            modifiers.Remove(mod);

            isDirty = true;
        }
    }
    void IStat.RemoveModifier(StatModifierBase mod)
    {
        RemoveModifier(mod as StatModifier<float>);
    }
    public void ClearModifier(StatModifier<float> mod)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing != null)
        {
            modifiers.Remove(existing);

            isDirty = true;

        }
    }
    public void UpdateModifier(StatModifier<float> mod, float value)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing != null)
        {
            existing.value = value;

            isDirty = true;
        }
    }

    public float Value
    {
        get
        {
            if (!isDirty)
                return cachedValue;

            float flatSum = 0;
            float additiveMultSum = 0;
            float multiplicativeMultProduct = 1f;
            float setValue = 0;
            bool hasSet = false;

            if (modifiers.Count != 0)
            {
                foreach (var mod in modifiers)
                {
                    switch (mod.type)
                    {
                        case StatModifierType.Flat:
                            flatSum += mod.value;
                            break;

                        case StatModifierType.AdditiveMultiplier:
                            additiveMultSum += mod.value;
                            break;

                        case StatModifierType.MultiplicativeMultiplier:
                            multiplicativeMultProduct *= mod.value;
                            break;
                        case StatModifierType.Set:
                            setValue = mod.value;
                            hasSet = true;
                            break;
                    }
                }
            }
            float baseToUse = hasSet ? setValue : baseValue;

            cachedValue = (baseToUse + flatSum) * (1f + additiveMultSum) * multiplicativeMultProduct;
            isDirty = false;

            return cachedValue;
        }
    }
    public int FloorValue => Mathf.FloorToInt(Value);
    public int IntValue => Mathf.RoundToInt(Value);
    public int CeilValue => Mathf.CeilToInt(Value);
}
public enum StatModifierType
{
    Flat, //+5
    AdditiveMultiplier, //+20%
    MultiplicativeMultiplier, //*2
    Set //=2
}