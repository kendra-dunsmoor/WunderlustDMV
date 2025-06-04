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
    public bool updateMovement = false;
    public Action.ActionMovement movement;
    public List<ActionEffectStacks> effects;
    public Item.Rarity rarity;

     public string GetDescription() {
        string description = "";
        if (WILL_MODIFIER != 0) description += "Reduced will cost: " + WILL_MODIFIER;
        if (FRUSTRATION_MODIFIER != 0) description += " Frustration modifier: " + FRUSTRATION_MODIFIER;
        if (PERFORMANCE_MODIFIER != 0) description += " Performance modifier: " + PERFORMANCE_MODIFIER;
        foreach (ActionEffectStacks effectStacks in effects ) {
            if (effectStacks != null) description += " Adds effect " + effectStacks.effect.type + " for " + effectStacks.stacks + " turns.";
        }
        if (updateMovement) description += " Updates customer movement to " + movement;
        return description;
    }
}
