using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class spell_input_controller
{
    // A Test behaves as an ordinary method
    [Test]
    public void does_not_cast_unlearned_spell()
    {
        // Arrange
        MockSpellInput spellInput = new MockSpellInput();
        MockCastableSpell spell = new MockCastableSpell();
        SpellInputController controller = new SpellInputController(spellInput);
        spell.SpellNum = "Not Casted";
        spellInput.SpellLevels.Add("Not Casted", 0);
        
        // Act
        controller.SpellButtonPressed(KeyCode.Q, spell);

        // Assert
        Assert.AreEqual("Not Casted", spell.SpellNum);
    }
}
