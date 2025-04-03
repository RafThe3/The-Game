using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 5)] public string[] dialogue = null;
}
