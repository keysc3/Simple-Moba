using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class effect
{
    // A Test behaves as an ordinary method
    [Test]
    public void does_not_set_effect_duration_to_given_invalid_negative_duration(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1.2f, unit1, unit2);

        // Assert
        Assert.AreEqual(0f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_5(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, 5f, unit1, unit2);

        // Assert
        Assert.AreEqual(5f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_negative_1(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1f, unit1, unit2);

        // Assert
        Assert.AreEqual(-1f, effect.EffectDuration);
    }

    [Test]
    public void adds_2_seconds_to_effect_timer_with_duration_3(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 3f, unit1, unit2);

        // Act
        effect.TimerTick(1.5f);
        effect.TimerTick(0.5f);

        // Assert
        Assert.AreEqual(2.0f, effect.effectTimer);
    }

    [Test]
    public void ends_effect_by_effect_timer_reaching_effect_duration(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 12.2f, unit1, unit2);

        // Act
        effect.TimerTick(8.6f);
        effect.TimerTick(3.6f);

        // Assert
        Assert.True(effect.isFinished);
    }

    [Test]
    public void resets_effect_timer_from_11_to_0(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 11f, unit1, unit2);

        // Act
        effect.ResetTimer();

        // Assert
        Assert.AreEqual(0f, effect.effectTimer);
    }

    [Test]
    public void overrides_effect_to_base_values_with_new_source(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        IUnit unit3 = Substitute.For<IUnit>();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 4f, unit1, unit2);

        // Act
        effect.OverrideEffect(unit3);

        // Assert
        Assert.AreEqual((0f, false, (IUnit) unit3), (effect.effectTimer, effect.isFinished, effect.casted));
    }
}
