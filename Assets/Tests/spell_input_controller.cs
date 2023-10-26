using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using NSubstitute;

public class spell_input_controller
{

    // Spell Button Press
    
    // A Test behaves as an ordinary method
    [Test]
    public void does_not_cast_unlearned_spell()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum.Returns(SpellType.Spell1);
        spell.IsQuickCast.Returns(true);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 0}};
        spellInput.SpellLevels.Returns(dict);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        ((IHasCast) spell).DidNotReceive().Cast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_cast_spell_on_CD()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum.Returns(SpellType.Spell1);
        spell.IsQuickCast.Returns(true);
        spell.OnCd.Returns(true);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        ((IHasCast) spell).DidNotReceive().Cast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_hide_readied_last_pressed_spells_cast_due_to_same_spell_button_press()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        ISpell lastPressed = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        lastPressed.IsDisplayed.Returns(true);
        spell.SpellNum.Returns(SpellType.Spell1);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        spellInput.LastSpellPressed.Returns(lastPressed);
        spellInput.LastButtonPressed.Returns(KeyCode.Q);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        spell.DidNotReceive().HideCast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void hides_last_pressed_spells_cast_due_to_different_spell_button_press()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        ISpell lastPressed = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        lastPressed.IsDisplayed.Returns(true);
        spell.SpellNum.Returns(SpellType.Spell1);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        spellInput.LastSpellPressed.Returns(lastPressed);
        spellInput.LastButtonPressed.Returns(KeyCode.Q);
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        lastPressed.Received().HideCast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_ready_non_quick_cast_spell_from_different_spell_button_press()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum.Returns(SpellType.Spell1);
        spell.IsDisplayed.Returns(false);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        spellInput.LastButtonPressed.Returns(KeyCode.Q);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        spell.DidNotReceive().DisplayCast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void readies_non_quick_cast_spell_from_spell_button_press()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum.Returns(SpellType.Spell1);
        spell.IsDisplayed = false;
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        spellInput.LastButtonPressed = KeyCode.Q;
        spell.When(x => x.DisplayCast()).Do(x => spell.IsDisplayed = true);
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        ((IHasCast) spell).DidNotReceive().Cast();
        Assert.AreEqual((true, KeyCode.W), (spell.IsDisplayed, spellInput.LastButtonPressed));
    }

    // A Test behaves as an ordinary method
    [Test]
    public void casts_quick_cast_spell_from_spell_button_press()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsQuickCast.Returns(true);
        spell.IsDisplayed.Returns(false);
        spell.SpellNum.Returns(SpellType.Spell1);
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        
        // Act
        controller.SpellButtonPressed(KeyCode.W, spell);

        // Assert
        ((IHasCast) spell).Received().Cast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void clears_readied_spell_from_quick_cast_spell_cast()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        ISpell lastPressed = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsQuickCast.Returns(true);
        spell.IsDisplayed.Returns(false);
        spell.SpellNum.Returns(SpellType.Spell1);
        spellInput.LastSpellPressed = lastPressed;
        spellInput.LastButtonPressed = KeyCode.R;
        Dictionary<SpellType, int> dict = new Dictionary<SpellType, int>(){{spell.SpellNum, 1}};
        spellInput.SpellLevels.Returns(dict);
        
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
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        SpellInputController controller = new SpellInputController(spellInput);
        ISpell nullSpell = null;
        spellInput.LastSpellPressed.Returns(nullSpell);
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
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastButtonPressed = KeyCode.Q;
        spellInput.LastSpellPressed.Returns(spell);
        spellInput.ButtonClick.Returns(true);

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
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsDisplayed.Returns(true);
        spellInput.LastSpellPressed.Returns(spell);
        spellInput.LastButtonPressed.Returns(KeyCode.Q);

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        spell.Received().HideCast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void unready_targeted_cast_spell_from_no_target_input()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasTargetedCast>();
        IUnit unit = Substitute.For<IUnit>();
        SpellInputController controller = new SpellInputController(spellInput);
        GameObject g1 = new GameObject();
        g1.AddComponent<BoxCollider>();
        unit.GameObject.Returns(g1);
        spell.SpellNum.Returns(SpellType.Spell1);
        spellInput.LastButtonPressed = KeyCode.Q;
        spellInput.LastSpellPressed.Returns(spell);

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
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasTargetedCast>();
        //IUnit unit = Substitute.For<IUnit>();
        SpellInputController controller = new SpellInputController(spellInput);
        GameObject g1 = new GameObject();
        g1.AddComponent<BoxCollider>();
        g1.AddComponent<MockPlayerBehaviour>();
        IUnit unit = g1.GetComponent<IUnit>();
        g1.transform.position = Vector3.zero;
        //unit.GameObject.Returns(g1);
        spell.SpellNum.Returns(SpellType.Spell1);
        spellInput.LastSpellPressed.Returns(spell);

        // Act
        controller.LeftClick(new Ray(Vector3.left, Vector3.right));

        // Assert
        ((IHasTargetedCast) spell).Received().AttemptCast(unit);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void casts_non_quick_cast_spell()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell, IHasCast>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum.Returns(SpellType.Spell1);
        spellInput.LastSpellPressed.Returns(spell);

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        ((IHasCast) spell).Received().Cast();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void unready_spell_without_cast()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastSpellPressed.Returns(spell);
        spellInput.LastButtonPressed = KeyCode.Q;

        // Act
        controller.LeftClick(new Ray(Vector3.zero, Vector3.right));

        // Assert
        Assert.AreEqual(KeyCode.None, spellInput.LastButtonPressed);
    }

    // CheckForUnready

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_unready_spell_from_non_spell_input_due_to_null_last_pressed_spell()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastButtonPressed = KeyCode.Q;
        ISpell nullSpell = null;
        spellInput.LastSpellPressed.Returns(nullSpell);

        // Act
        controller.CheckForUnready();

        // Assert
        Assert.AreEqual(KeyCode.Q, spellInput.LastButtonPressed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_unready_spell_from_non_spell_input_due_to_no_button_registered()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spellInput.LastSpellPressed = spell;
        spellInput.LastButtonPressed.Returns(KeyCode.None);

        // Act
        controller.CheckForUnready();

        // Assert
        Assert.AreEqual(spell, spellInput.LastSpellPressed);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void unready_spell_from_non_readied_spell_input()
    {
        // Arrange
        ISpellInput spellInput = Substitute.For<ISpellInput>();
        ISpell spell = Substitute.For<ISpell>();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.IsDisplayed = true;
        spellInput.LastSpellPressed = spell;
        spellInput.LastButtonPressed = KeyCode.Q;
        spell.When(x => x.HideCast()).Do(x => spell.IsDisplayed = false);

        // Act
        controller.CheckForUnready();

        // Assert
        Tuple<bool, ISpell, KeyCode> expected = new Tuple<bool, ISpell, KeyCode>(false, null, KeyCode.None);
        Tuple<bool, ISpell, KeyCode> actual = new Tuple<bool, ISpell, KeyCode>(spell.IsDisplayed, spellInput.LastSpellPressed, spellInput.LastButtonPressed);
        Assert.AreEqual(expected, actual);
    }
}
