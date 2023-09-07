using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using NSubstitute;

public class spell_controller
{
    // A Test behaves as an ordinary method
    [Test]
    public void gets_54_cooldown_from_68_base_with_24_haste()
    {
        // Arrange
        IPlayer player = CreateMockPlayer();
        ISpell spell = Substitute.For<ISpell>();
        SpellController controller = new SpellController(spell, player);
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.haste.BaseValue = 24f;
        player.unitStats.Returns(unitStats);

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
        IPlayer player = CreateMockPlayer();
        ISpell spell = Substitute.For<ISpell>();
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
        IPlayer player = CreateMockPlayer();
        ISpell spell = Substitute.For<ISpell>();
        SpellController controller = new SpellController(spell, player);
        GameObject g1 = new GameObject();
        Image slider = g1.AddComponent<Image>();

        // Act
        controller.UpdateActiveSpellSlider(slider, 10f, 2f);

        // Assert
        Assert.AreEqual(0.8f, slider.fillAmount);

    }

    public IPlayer CreateMockPlayer(){
        IPlayer player = Substitute.For<IPlayer>();
        player.GameObject.Returns(new GameObject());
        return player;
    }
}
