using System;
using System.Collections.Generic;
using Opsive.Shared.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
[Serializable]
[InlineProperty]
[HideDuplicateReferenceBox]
public class VectorStat : IStat<Vector3>
{
    public Vector3 baseValue;
    public List<StatModifier<Vector3>> modifiers { get; } = new();

    public Vector3 cachedValue;
    bool isDirty = true;
    public VectorStat(Vector3 baseValue) 
    {
        this.baseValue = baseValue;
        isDirty = true;
    }

    public StatModifier<Vector3> AddModifier(StatModifier<Vector3> mod)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing == null)
        {
            modifiers.Add(mod);
        }

        isDirty = true;
        return mod;
    }
    public StatModifier<Vector3> AddModifier(Vector3 value, StatModifierType type)
    {
        var mod = new StatModifier<Vector3>(value, type);
        var existing = modifiers.Find(m => m == mod);

        if (existing == null)
        {
            modifiers.Add(mod);
        }

        isDirty = true;
        return mod;
    }
    public void RemoveModifier(StatModifier<Vector3> mod)
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
        RemoveModifier(mod as StatModifier<Vector3>);
    }
    public void ClearModifier(StatModifier<Vector3> mod)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing != null)
        {
            modifiers.Remove(existing);

            isDirty = true;

        }
    }
    public void UpdateModifier(StatModifier<Vector3> mod, Vector3 value)
    {
        var existing = modifiers.Find(m => m == mod);

        if (existing != null)
        {
            existing.value = value;

            isDirty = true;
        }
    }
    public Vector3 Value
    {
        get
        {
            if (!isDirty)
                return cachedValue;

            Vector3 flatSum = Vector3.zero;
            Vector3 additiveMultSum = Vector3.zero;
            Vector3 multiplicativeMultProduct = Vector3.one;
            Vector3 setValue = Vector3.zero;
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
                            multiplicativeMultProduct.x *= mod.value.x;
                            multiplicativeMultProduct.y *= mod.value.y;
                            multiplicativeMultProduct.z *= mod.value.z;
                            break;
                        case StatModifierType.Set:
                            if (!hasSet)
                            {
                                setValue = baseValue;
                                hasSet = true;
                            }

                            if (mod.value.x != baseValue.x) setValue.x = mod.value.x;
                            if (mod.value.y != baseValue.y) setValue.y = mod.value.y;
                            if (mod.value.z != baseValue.z) setValue.z = mod.value.z;
                            break;
                    }
                }
            }

            Vector3 baseToUse = hasSet ? setValue : baseValue;

            cachedValue = Vector3.Scale(baseToUse + flatSum, (Vector3.one + additiveMultSum));

            cachedValue = Vector3.Scale(cachedValue, multiplicativeMultProduct);
            isDirty = false;

            return cachedValue;
        }
    }
    public float x
    {
        get
        {
            return Value.x;
        }
    }
    public float y
    {
        get
        {
            return Value.y;
        }
    }
    public float z
    {
        get
        {
            return Value.z;
        }
    }
    public float magnitude
    {
        get
        {
            return Value.magnitude;
        }
    }
    public Vector3 normalized
    {
        get
        {
            return Value.normalized;
        }
    }
}
