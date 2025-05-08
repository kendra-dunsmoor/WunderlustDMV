using UnityEngine;

/*
* Dialogue
* ~~~~~~~~~
* Data object for instance of dialogue
*/
[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3,10)]
    public string[] sentences;
}