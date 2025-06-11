using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "New Action Upgrade", menuName = "Action/Action Upgrade")]
public class ActionUpgrade : ScriptableObject
{
    public string upgradeName;
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;
    public float ATTENTION_MODIFIER;
    public bool updateMovement = false;
    public Action.ActionMovement movement;
    public List<ActionEffectStacks> effects;
    public Item.Rarity rarity;

     public string GetDescription() {
        string description = "";
        if (WILL_MODIFIER < 0 ) description += "Costs " + -WILL_MODIFIER + " less Will";
        if (WILL_MODIFIER > 0 ) description += "Adds " + WILL_MODIFIER + " Will cost";
        if (FRUSTRATION_MODIFIER > 0) description += "\nAdds " + FRUSTRATION_MODIFIER + " Frustration";
        if (FRUSTRATION_MODIFIER < 0) description += "\nRemoves " + -FRUSTRATION_MODIFIER + " Frustration";

        if (PERFORMANCE_MODIFIER > 0) description += "\nGain " + PERFORMANCE_MODIFIER + " Performance";
        if (PERFORMANCE_MODIFIER < 0) description += "\nLose " + -PERFORMANCE_MODIFIER + " Performance";
        
        if (ATTENTION_MODIFIER > 0) description += "\nGain " + ATTENTION_MODIFIER + "% Attention";
        if (ATTENTION_MODIFIER < 0) description += "\nLose " + -ATTENTION_MODIFIER + "% Attention";

        foreach (ActionEffectStacks effectStacks in effects)
        {
            // TODO: Clearer descriptions for particular actions
            if (effectStacks != null)
            {
                if (effectStacks.effect.type == ActionEffect.EffectType.ADD_TURNS) description += "\nSkips a turn.";
                else if (effectStacks.effect.type == ActionEffect.EffectType.MADE_MISTAKE) description += "\nEach Mistake draws more Attention.";
                else description += "\nAdds effect \"" + effectStacks.effect.effectName + "\" for " + effectStacks.stacks + " turns.";
            }
        }

        if (updateMovement)
        {
            if (movement == Action.ActionMovement.FRONT) description += "\nCustomer stays in line";
            if (movement == Action.ActionMovement.AWAY) description += "\nCustomer is removed";
        }
        return description;
    }
}
