using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Dialogue", menuName = "Dialogue/Tutorial Dialogue")]
public class TutorialDialogue : ScriptableObject
{
    public TutorialDialogueNode[] lines;
}
