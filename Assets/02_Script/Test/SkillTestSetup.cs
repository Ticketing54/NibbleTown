using UnityEngine;

public class SkillTestSetup : MonoBehaviour
{
    [SerializeField] private int[] skillIds = { 1, 2 };

    private void Start()
    {
        var skillBook = FindObjectOfType<SkillBook>();
        if (skillBook == null) return;

        for (int i = 0; i < skillIds.Length && i < 3; i++)
            skillBook.SetSlot(i, skillIds[i]);
    }
}
