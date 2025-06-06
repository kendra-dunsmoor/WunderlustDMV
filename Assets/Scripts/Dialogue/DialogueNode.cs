using UnityEngine;
using System.Collections.Generic;
 
[System.Serializable]
public class DialogueNode
{
    // Just adding this so it is hopefully a little easier to visualize in editor
    [SerializeField] private string tag;

    [TextArea(3,10)]
    public string dialogueText;
    public List<DialogueResponse> responses;

    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}