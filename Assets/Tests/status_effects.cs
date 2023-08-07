using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class status_effects
{
    private List<float> slowValues = new List<float>(){0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
    private List<float> durationValues = new List<float>(){1f, 2f, 3f, 4f, 5f};
    /*public ScriptableSlow slow1;
    public ScriptableSlow slow2;*/

    // A Test behaves as an ordinary method
    [Test]
    public void add_3_slows_where_second_slow_is_the_strongest()
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
