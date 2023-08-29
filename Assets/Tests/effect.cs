using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class effect
{
    // A Test behaves as an ordinary method
    [Test]
    public void does_not_set_effect_duration_to_given_invalid_negative_duration(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1.2f, m1, m2);

        // Assert
        Assert.AreEqual(0f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_5(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, 5f, m1, m2);

        // Assert
        Assert.AreEqual(5f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_negative_1(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1f, m1, m2);

        // Assert
        Assert.AreEqual(-1f, effect.EffectDuration);
    }

    [Test]
    public void adds_2_seconds_to_effect_timer_with_duration_3(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 3f, m1, m2);

        // Act
        effect.TimerTick(1.5f);
        effect.TimerTick(0.5f);

        // Assert
        Assert.AreEqual(2.0f, effect.effectTimer);
    }

    [Test]
    public void ends_effect_by_effect_timer_reaching_effect_duration(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 12.2f, m1, m2);

        // Act
        effect.TimerTick(8.6f);
        effect.TimerTick(3.6f);

        // Assert
        Assert.AreEqual(true, effect.isFinished);
    }

    [Test]
    public void resets_effect_timer_from_11_to_0(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 11f, m1, m2);

        // Act
        effect.ResetTimer();

        // Assert
        Assert.AreEqual(0f, effect.effectTimer);
    }

    [Test]
    public void overrides_effect_to_base_values_with_new_source(){
        // Arrange
        MockUnit m1 = new MockUnit();
        MockUnit m2 = new MockUnit();
        MockUnit m3 = new MockUnit();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 4f, m1, m2);

        // Act
        effect.OverrideEffect(m3);

        // Assert
        Assert.AreEqual((0f, false, (IUnit) m3), (effect.effectTimer, effect.isFinished, effect.casted));
    }
}
