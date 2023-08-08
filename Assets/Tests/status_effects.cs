using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class status_effects
{
    private List<float> slowValues { get;}= new List<float>(){0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
    private List<float> durationValues { get; }= new List<float>(){1f, 2f, 3f, 4f, 5f};
    /*public ScriptableSlow slow1;
    public ScriptableSlow slow2;*/

    // A Test behaves as an ordinary method
    [Test]
    public void sets_only_strongest_slow_active_from_3_slows()
    {
        // Arrange 
        Slow s1 = CreateSlowEffect("Slow1", 0);
        Slow s2 = CreateSlowEffect("Slow2", 4);
        Slow s3 = CreateSlowEffect("Slow3", 2);
        StatusEffects se = new StatusEffects();
        
        // Act
        se.AddEffect(s1);
        se.AddEffect(s2);
        se.AddEffect(s3);
        List<bool> activeEffects = new List<bool>(){s1.IsActivated, s2.IsActivated, s3.IsActivated};

        // Assert
        Assert.AreEqual(new List<bool>(){false, true, false}, activeEffects);
    }

    [Test]
    public void sets_only_strongest_cc_effect_active(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableSleep sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep.duration.AddRange(durationValues);
        Sleep sleep1 = (Sleep) sleep.InitializeEffect(3, g1, g2);

        ScriptableCharm charm = ScriptableCharm.CreateInstance<ScriptableCharm>();
        charm.duration.AddRange(durationValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        StatusEffects se = new StatusEffects();

        // Act
        se.AddEffect(charm1);
        se.AddEffect(sleep1);
        List<bool> isActivated = new List<bool>(){charm1.IsActivated, sleep1.IsActivated};

        // Assert
        Assert.AreEqual(new List<bool>(){false, true}, isActivated);
    }

    public Slow CreateSlowEffect(string slowName, int index){
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        ScriptableSlow slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        slow.name = slowName;
        slow.duration.AddRange(durationValues);
        slow.slowPercent.AddRange(slowValues);
        return (Slow) slow.InitializeEffect(index, g1, g2);
    }
}
