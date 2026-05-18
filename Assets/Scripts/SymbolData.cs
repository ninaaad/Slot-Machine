using UnityEngine;

[CreateAssetMenu(fileName = "SymbolData", menuName = "SlotGame/SymbolData")]
public class SymbolData : ScriptableObject
{
    public string symbolName;
    public Sprite sprite;
    public int payoutMultiplier; // e.g., 10x, 20x, 50x
}