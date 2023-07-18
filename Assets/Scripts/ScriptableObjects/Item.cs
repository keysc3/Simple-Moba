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

    [field: SerializeField] public float magicDamage { get; private set; }
    [field: SerializeField] public float physicalDamage { get; private set; }
    [field: SerializeField] public float health { get; private set; }
    [field: SerializeField] public float mana { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float magicResist { get; private set; }
    [field: SerializeField] public float armor { get; private set; }
    [field: SerializeField] public float attackSpeed { get; private set; }
}
