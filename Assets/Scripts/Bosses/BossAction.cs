using UnityEngine;

[CreateAssetMenu(fileName = "BossAction", menuName = "Scriptable Objects/BossAction")]
public class BossAction : ScriptableObject
{
    public string BossActionName;
    public string generalDescription;

    [Header("------------- Action Effects -------------")]
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;
    public float BOSS_WILL_MODIFIER;
    public float ACTION_CHANCE = 1.0f; // The chance of the action happening, 1.0f = 100%
    public string GetDescription()
    {
        string description = generalDescription;
        if (WILL_MODIFIER > 0) description += "Gain " + WILL_MODIFIER + "Will";
        if (WILL_MODIFIER < 0) description += "Lose " + WILL_MODIFIER + "Will";
        if (PERFORMANCE_MODIFIER > 0) description += "Gain " + PERFORMANCE_MODIFIER + "Performance";
        if (PERFORMANCE_MODIFIER < 0) description += "Lose " + PERFORMANCE_MODIFIER + "Performance";
        if (BOSS_WILL_MODIFIER > 0) description += "Gain " + BOSS_WILL_MODIFIER + "% Boss Will";
        if (BOSS_WILL_MODIFIER < 0) description += "Lose " + BOSS_WILL_MODIFIER + "% Boss Will";
        // foreach (ActionEffectStacks effectStacks in effects)
        // {
        //     if (effectStacks != null) description += " Adds effect " + effectStacks.effect.type + " for " + effectStacks.stacks + " turns.";
        // }
        return description;
    }
}