using UnityEngine;

/*
* Purpose: ScriptableObject for an item.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;

    public float magicDamage;
    public float physicalDamage;
    public float health;
    public float mana;
    public float speed;
    public float magicResist;
    public float armor;
    public float attackSpeed;
}
