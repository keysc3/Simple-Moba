using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class spell_controller
{
    // A Test behaves as an ordinary method
    [Test]
    public void gets_54_cooldown_from_68_base_with_24_haste()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockSpell spell = new MockSpell();
        SpellController controller = new SpellController(spell, player);
        player.unitStats = new UnitStats(player.SUnit);
        player.unitStats.haste.BaseValue = 24f;

        // Act
        float final = controller.CalculateCooldown(68f);

        // Assert
        Assert.AreEqual(54.839f, final);

    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_children_gameobjects_active()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockSpell spell = new MockSpell();
        SpellController controller = new SpellController(spell, player);
        GameObject parent = new GameObject();
        GameObject child1 = new GameObject();
        GameObject child2 = new GameObject();
        child1.transform.SetParent(parent.transform);
        child2.transform.SetParent(parent.transform);
        child1.SetActive(false);
        child2.SetActive(false);

        // Act
        controller.SpellCDChildrenSetActive(parent.transform, true);

        // Assert
        Assert.AreEqual((true, true), (child1.activeSelf, child2.activeSelf));

    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_image_fill_amount()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockSpell spell = new MockSpell();
        SpellController controller = new SpellController(spell, player);
        GameObject g1 = new GameObject();
        Image slider = g1.AddComponent<Image>();

        // Act
        controller.UpdateActiveSpellSlider(slider, 10f, 2f);

        // Assert
        Assert.AreEqual(0.8f, slider.fillAmount);

    }
}
