using UnityEngine;

public class PushObject : MonoBehaviour
{
    public int currentRow;
    public int currentColumn;
    public bool isFirstOrLast;
    public bool isPast;
    public MeshRenderer renderer;
    public void PrintRowColumn()
    {
        Debug.Log($"({currentRow}, {currentColumn})");
    }
}
