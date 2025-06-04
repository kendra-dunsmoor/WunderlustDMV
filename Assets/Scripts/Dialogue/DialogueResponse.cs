using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueResponse
{
    public string responseText;
    public int nextNodeIndex;

    public bool containsReward;
    public int officeBucks;
    public int soulCredits;
    public List<Item> itemsRewards;
    public List<ActionEffectStacks> effectRewards;
    public float performanceBoost;
    public float willBoost;
}