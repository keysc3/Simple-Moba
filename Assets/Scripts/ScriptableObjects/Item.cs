using UnityEngine;

/*
* Purpose: ScriptableObject for an item.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [field: SerializeField] new public string name { get; private set; } = "New Item";
    [field: SerializeField] public Sprite icon { get; private set; } = null;

    [field: SerializeField] public float magicDamage { get; set; }
    [field: SerializeField] public float physicalDamage { get; set; }
    [field: SerializeField] public float health { get; set; }
    [field: SerializeField] public float mana { get; set; }
    [field: SerializeField] public float speed { get; set; }
    [field: SerializeField] public float magicResist { get; set; }
    [field: SerializeField] public float armor { get; set; }
    [field: SerializeField] public float attackSpeed { get; set; }
}
