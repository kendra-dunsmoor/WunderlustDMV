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
}