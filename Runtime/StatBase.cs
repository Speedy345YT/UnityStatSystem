public interface IStat
{
    void RemoveModifier(StatModifierBase mod);
}
public interface IStat<T> : IStat
{
    public StatModifier<T> AddModifier(StatModifier<T> mod);
    public StatModifier<T> AddModifier(T value, StatModifierType type);
    void RemoveModifier(StatModifier<T> mod);
    void ClearModifier(StatModifier<T> mod);
    void UpdateModifier(StatModifier<T> mod, T value);
    T Value { get; }
}
public abstract class StatModifierBase
{
    public StatModifierType type;
}
public class StatModifier<T> : StatModifierBase
{
    public T value;
    public StatModifier(T value, StatModifierType type)
    {
        this.value = value;
        this.type = type;
    }
}
