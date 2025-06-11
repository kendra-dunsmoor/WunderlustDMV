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
        string description = "Nepo-Baby used " + BossActionName;
        if (generalDescription != "" || generalDescription != null) description += "\n" + generalDescription;
        if (WILL_MODIFIER > 0) description += "\nGained " + WILL_MODIFIER + " Will";
        if (WILL_MODIFIER < 0) description += "\nLost " + -WILL_MODIFIER + " Will";
        if (WILL_MODIFIER > 0) description += "\nGained " + WILL_MODIFIER + " Boss Will";
        if (WILL_MODIFIER < 0) description += "\nLost " + -WILL_MODIFIER + " Boss Will";
        if (PERFORMANCE_MODIFIER > 0) description += "\nGained " + PERFORMANCE_MODIFIER + " Performance";
        if (PERFORMANCE_MODIFIER < 0) description += "\nLost " + -PERFORMANCE_MODIFIER + " Performance";
        // foreach (ActionEffectStacks effectStacks in effects)
        // {
        //     // TODO: Clearer descriptions for particular actions
        //     if (effectStacks != null)
        //     {
        //         if (effectStacks.effect.type == ActionEffect.EffectType.ADD_TURNS) description += "\nSkips a turn.";
        //         else if (effectStacks.effect.type == ActionEffect.EffectType.MADE_MISTAKE) description += "\nEach Mistake draws more Attention.";
        //         else description += "\nAdded effect \"" + effectStacks.effect.effectName + "\" for " + effectStacks.stacks + " turns.";
        //     }
        // }
        return description;
    }
}