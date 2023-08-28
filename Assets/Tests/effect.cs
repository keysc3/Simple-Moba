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
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1.2f, g1, g2);

        // Assert
        Assert.AreEqual(0f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_5(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, 5f, g1, g2);

        // Assert
        Assert.AreEqual(5f, effect.EffectDuration);
    }

    [Test]
    public void sets_effect_duration_to_negative_1(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();

        // Act
        Effect effect = new Effect(sEffect, -1f, g1, g2);

        // Assert
        Assert.AreEqual(-1f, effect.EffectDuration);
    }

    [Test]
    public void adds_2_seconds_to_effect_timer_with_duration_3(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 3f, g1, g2);

        // Act
        effect.TimerTick(1.5f);
        effect.TimerTick(0.5f);

        // Assert
        Assert.AreEqual(2.0f, effect.effectTimer);
    }

    [Test]
    public void ends_effect_by_effect_timer_reaching_effect_duration(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 12.2f, g1, g2);

        // Act
        effect.TimerTick(8.6f);
        effect.TimerTick(3.6f);

        // Assert
        Assert.AreEqual(true, effect.isFinished);
    }

    [Test]
    public void resets_effect_timer_from_11_to_0(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 11f, g1, g2);

        // Act
        effect.ResetTimer();

        // Assert
        Assert.AreEqual(0f, effect.effectTimer);
    }

    [Test]
    public void overrides_effect_to_base_values_with_new_source(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        GameObject g3 = new GameObject();
        ScriptableEffect sEffect = ScriptableObject.CreateInstance<ScriptableEffect>();
        Effect effect = new Effect(sEffect, 4f, g1, g2);

        // Act
        effect.OverrideEffect(g3);

        // Assert
        Assert.AreEqual((0f, false, g3), (effect.effectTimer, effect.isFinished, effect.casted));
    }
}
