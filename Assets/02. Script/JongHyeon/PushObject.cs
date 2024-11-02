using UnityEngine;

public class PushObject : MonoBehaviour
{
    public int currentRow;
    public int currentColumn;
    public void PrintRowColumn()
    {
        Debug.Log($"({currentRow}, {currentColumn})");
    }
}
