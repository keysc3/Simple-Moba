using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class player_spells
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    /*[UnityTest]
    public IEnumerator sets_up_player_spells_spell_buttons()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        MockPlayerBehaviour player = g1.AddComponent<MockPlayerBehaviour>();
        ISpell spell = AddSpellComponent(g1, SpellType.Passive);
        player.playerUI = CreateSpellsContainer();
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();

        // Act
        yield return null;

        // Assert
        SpellButton spellButton = player.playerUI.transform.Find("Player/Combat/SpellsContainer/Passive_Container/SpellContainer/Spell/Button").GetComponent<SpellButton>();
        Assert.AreEqual(spell, spellButton.spell);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator sets_up_player_spells_spell_level_up_buttons()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        MockPlayerBehaviour player = g1.AddComponent<MockPlayerBehaviour>();
        ISpell spell = AddSpellComponent(g1, SpellType.Spell1);
        player.playerUI = CreateSpellsContainer();
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();

        // Act
        yield return null;

        // Assert
        SpellLevelUpButton spellLevelUpButton = player.playerUI.transform.Find("Player/Combat/SpellsContainer/Spell1_Container/LevelUp/Button").GetComponent<SpellLevelUpButton>();
        Assert.AreEqual(spell.spellData.defaultSpellNum, spellLevelUpButton.spell);
    }*/ 

    public GameObject CreateSpellsContainer(){
        GameObject parent = new GameObject("Parent");
        Transform player = new GameObject("Player").transform;
        Transform combat = new GameObject("Combat").transform;
        Transform spellsContainer = new GameObject("SpellsContainer").transform;
        player.SetParent(parent.transform);
        combat.SetParent(player);
        spellsContainer.SetParent(combat);
        for(int i = 0; i < 5; i++){
            string name;
            if(i != 4)
                name = $"Spell{i + 1}";
            else
                name = "Passive";
            Transform parentContainer = new GameObject(name + "_Container").transform;
            Transform spellContainer = new GameObject("SpellContainer").transform;
            Transform spell = new GameObject("Spell").transform;
            Transform button = new GameObject("Button").transform;
            Transform icon = new GameObject("Icon").transform;
            Transform levelUp = new GameObject("LevelUp").transform;
            Transform levelUpButton = new GameObject("Button").transform;
            parentContainer.SetParent(spellsContainer);
            spellContainer.SetParent(parentContainer);
            spell.SetParent(spellContainer);
            button.SetParent(spell);
            icon.SetParent(spell);
            levelUp.SetParent(parentContainer);
            levelUpButton.SetParent(levelUp);
            button.gameObject.AddComponent<SpellButton>();
            levelUpButton.gameObject.AddComponent<SpellLevelUpButton>();
            icon.gameObject.AddComponent<Image>();
        }
        return parent;
    }

    private ISpell AddSpellComponent(GameObject parent, SpellType defaultNum){
        MockSpellBehaviour spell = parent.AddComponent<MockSpellBehaviour>();
        spell.spellData = SpellData.CreateNewInstance(defaultNum);
        return spell;
    }
}
