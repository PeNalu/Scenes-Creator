using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Generator/Settings", fileName = "Generator Settings")]
public class GeneratorSettings : ScriptableObject
{
    public List<string> positions;
    public List<string> roomsName;
    public List<string> connectedMark;
    public List<string> stopEntities;
    public List<string> animationStates;
    public List<string> animateObjects;
}
