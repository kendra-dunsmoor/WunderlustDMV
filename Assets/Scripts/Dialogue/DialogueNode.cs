using UnityEngine;
using System.Collections.Generic;
 
[System.Serializable]
public class DialogueNode
{
    [TextArea(3,10)]
    public string dialogueText;
    public List<DialogueResponse> responses;
 
    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}