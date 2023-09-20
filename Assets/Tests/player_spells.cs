using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class player_spells
{
    // A Test behaves as an ordinary method
    [Test]
    public void calls_spells_death_clean_up_method()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        g1.SetActive(false);
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();
        ISpell cleanupSpell = Substitute.For<ISpell, IDeathCleanUp>();
        ISpell spell = Substitute.For<ISpell>();
        playerSpells.spells = new Dictionary<string, ISpell>(){{"Spell_1", spell}, {"Spell_2", cleanupSpell}};
        
        // Act
        playerSpells.OnDeathSpellCleanUp();

        // Assert
        ((IDeathCleanUp) cleanupSpell).Received().OnDeathCleanUp();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void adds_new_spell_without_callback_and_no_duplicate_spell_num_to_player_spells()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        g1.SetActive(false);
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();
        ISpell spell1 = Substitute.For<ISpell>();
        ISpell spell2 = Substitute.For<ISpell>();
        ISpell spell3 = Substitute.For<ISpell>();
        spell3.SpellNum.Returns("Spell_3");
        playerSpells.spells = new Dictionary<string, ISpell>(){{"Spell_1", spell1}, {"Spell_2", spell2}};
        
        // Act
        playerSpells.SetupSpell(spell3, "Spell_3");

        // Assert
        Assert.True(playerSpells.spells.ContainsKey("Spell_3"));
    }

    // A Test behaves as an ordinary method
    [Test]
    public void adds_new_spell_without_callback_and_duplicate_spell_num_to_player_spells()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        g1.SetActive(false);
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();
        ISpell spell1 = Substitute.For<ISpell>();
        ISpell spell2 = Substitute.For<ISpell>();
        ISpell spell3 = Substitute.For<ISpell>();
        spell3.SpellNum.Returns("Spell_3");
        playerSpells.spells = new Dictionary<string, ISpell>(){{"Spell_1", spell1}, {"Spell_2", spell2}};
        LogAssert.Expect(LogType.Error, "Destroy may not be called from edit mode! Use DestroyImmediate instead.\nDestroying an object in edit mode destroys it permanently.");

        // Act
        playerSpells.SetupSpell(spell3, "Spell_2");

        // Assert
        Assert.AreEqual(playerSpells.spells["Spell_2"], spell3);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void adds_new_spell_with_callback_to_player_spells()
    {
        // Arrange
        GameObject g1 = new GameObject("PlayerSpells");
        g1.SetActive(false);
        PlayerSpells playerSpells = g1.AddComponent<PlayerSpells>();
        ISpell spell1 = Substitute.For<ISpell>();
        ISpell spell2 = Substitute.For<ISpell>();
        ISpell spell3 = Substitute.For<ISpell, IHasCallback>();
        spell3.SpellNum.Returns("Spell_3");
        playerSpells.spells = new Dictionary<string, ISpell>(){{"Spell_1", spell1}, {"Spell_2", spell2}};

        // Act
        playerSpells.SetupSpell(spell3, "Spell_3");

        // Assert
        ((IHasCallback) spell3).Received().SetupCallbacks(playerSpells.spells);
    }
}
