using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "Scriptable Objects/Class")]
public class Class : ScriptableObject
{
    public List<Action> actionLoadout; // leave for now in case we want to customize more than just one action later
    public Action specialAction;
    public string className;
    public ArtifactItem startingItem;
}