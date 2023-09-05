using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class spell_input_controller
{

    // Spell Button Press
    
    // A Test behaves as an ordinary method
    [Test]
    public void does_not_cast_unlearned_spell()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum = "Not Casted";
        spell.IsQuickCast = true;
        spellInput.SpellLevels.Add(spell.SpellNum, 0);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        Assert.AreEqual("Not Casted", spell.SpellNum);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_cast_spell_on_CD()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum = "Not Casted";
        spell.IsQuickCast = true;
        spell.OnCd = true;
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        Assert.AreEqual("Not Casted", spell.SpellNum);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_hide_readied_last_pressed_spells_cast_due_to_same_spell_button_press()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockSpell spell = new MockSpell();
        MockSpell lastPressed = new MockSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        lastPressed.IsDisplayed = true;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        spellInput.LastSpellPressed = lastPressed;
        spellInput.LastButtonPressed = KeyCode.Q;
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        Assert.AreEqual(true, lastPressed.IsDisplayed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void hides_last_pressed_spells_cast_due_to_different_spell_button_press()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockSpell spell = new MockSpell();
        MockSpell lastPressed = new MockSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        lastPressed.IsDisplayed = true;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        spellInput.LastSpellPressed = lastPressed;
        spellInput.LastButtonPressed = KeyCode.Q;
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        Assert.AreEqual(false, lastPressed.IsDisplayed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_ready_non_quick_cast_spell_from_different_spell_button_press()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsDisplayed = false;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        spellInput.LastButtonPressed = KeyCode.Q;
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        Assert.AreEqual(false, spell.IsDisplayed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void readies_non_quick_cast_spell_from_spell_button_press()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsDisplayed = false;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        spellInput.LastButtonPressed = KeyCode.Q;
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        Assert.AreEqual((true, KeyCode.W, "Not Casted"), (spell.IsDisplayed, spellInput.LastButtonPressed, spellInput.LastSpellPressed.SpellNum));
    }

    // A Test behaves as an ordinary method
    [Test]
    public void casts_quick_cast_spell_from_spell_button_press()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsQuickCast = true;
        spell.IsDisplayed = false;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        Assert.AreEqual("Casted", spell.SpellNum);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void clears_readied_spell_from_quick_cast_spell_cast()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsQuickCast = true;
        spell.IsDisplayed = false;
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add(spell.SpellNum, 1);
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        Tuple<ISpell, KeyCode> expected = new Tuple<ISpell, KeyCode>(null, KeyCode.None);
        Tuple<ISpell, KeyCode> actual = new Tuple<ISpell, KeyCode>(spellInput.LastSpellPressed, spellInput.LastButtonPressed);
        Assert.AreEqual(expected, actual);
    }

    // Left Click

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_unready_spell_from_no_last_spell_pressed()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastButtonPressed = KeyCode.Q;
        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(KeyCode.Q, spellInput.LastButtonPressed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_unready_spell_from_button_click_left_click()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockSpell spell = new MockSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastSpellPressed = spell;
        spellInput.LastButtonPressed = KeyCode.Q;
        spellInput.ButtonClick = true;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(KeyCode.Q, spellInput.LastButtonPressed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void hides_non_quick_cast_spells_cast_from_left_click()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockSpell spell = new MockSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsDisplayed = true;
        spellInput.LastSpellPressed = spell;
        spellInput.LastButtonPressed = KeyCode.Q;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(false, spell.IsDisplayed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void unready_targeted_cast_spell_from_no_target_input()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockTargetedCastSpell spell = new  MockTargetedCastSpell();
        MockUnit unit = new MockUnit();
        SpellInputController controller = new SpellInputController(spellInput);
        unit.GameObject.AddComponent<BoxCollider>();
        spell.SpellNum = "Not Casted";
        spellInput.LastSpellPressed = spell;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(KeyCode.None, spellInput.LastButtonPressed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void casts_targeted_cast_spell()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockTargetedCastSpell spell = new  MockTargetedCastSpell();
        MockUnit unit = new MockUnit();
        SpellInputController controller = new SpellInputController(spellInput);
        unit.GameObject.AddComponent<BoxCollider>();
        unit.GameObject.transform.position = Vector3.zero;
        spell.SpellNum = "Not Casted";
        spellInput.LastSpellPressed = spell;

        // Act
        controller.LeftClick(new Ray(Vector3.left, Vector3.right));

        // Assert
        Assert.AreEqual("Targeted spell casted", spell.SpellNum);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void casts_non_quick_cast_spell()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new  MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum = "Not Casted";
        spellInput.LastSpellPressed = spell;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual("Casted", spell.SpellNum);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void unready_spell_without_cast()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockSpell spell = new  MockSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastSpellPressed = spell;
        spellInput.LastButtonPressed = KeyCode.Q;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(KeyCode.None, spellInput.LastButtonPressed);
    }
}
