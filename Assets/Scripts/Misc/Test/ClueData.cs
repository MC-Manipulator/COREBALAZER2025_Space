using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueDatabase", menuName = "Clue System/Clue Database")]
public class ClueData : ScriptableObject
{
    public List<Clue> allClues = new List<Clue>();

    public bool TryGetClue(string clueID, out Clue result)
    {
        result = allClues.Find(c => c.clueID == clueID);
        return result != null;
    }
}