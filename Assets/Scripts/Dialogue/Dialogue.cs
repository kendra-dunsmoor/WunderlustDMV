using System.Collections.Generic;
using UnityEngine;

/*
* Dialogue
* ~~~~~~~~~
* Data object for instance of dialogue
*/
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    //First node of the conversation
    public DialogueNode RootNode;

    // contains all other nodes that dialogue choices can point to
    public List<DialogueNode> nodes;
    public Character character;
}