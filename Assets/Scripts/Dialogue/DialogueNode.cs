using UnityEngine;
using System.Collections.Generic;
 
[System.Serializable]
public class DialogueNode
{
    // Just adding this so it is hopefully a little easier to visualize in editort
    [SerializeField] private int index;

    [TextArea(3,10)]
    public string dialogueText;
    public List<DialogueResponse> responses;
    public bool containsReward;
    public List<Item> itemsRewards;
    public List<ActionEffect> effectRewards;

    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}