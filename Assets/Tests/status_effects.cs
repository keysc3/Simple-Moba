using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

/*
* Purpose: Unit tests for the StatusEffects class.
*
* @author: Colin Keys
*/
public class status_effects
{
    private List<float> slowValues { get; } = new List<float>(){0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
    private List<float> durationValues { get; } = new List<float>(){1f, 2f, 3f, 4f, 5f};

    /*
    *   Adds three slows to the status effects list. Only the strongest should be activated.
    */
    [Test]
    public void sets_only_strongest_slow_active_from_3_slows(){
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

    /*
    *   Adds two non-zero cc value effects to the status effects list. Only the strongest should be active.
    */
    [Test]
    public void sets_only_strongest_cc_effect_active_from_nonzero_cc_effects(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableSleep sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep.name = "Sleep1";
        sleep.duration.AddRange(durationValues);
        Sleep sleep1 = (Sleep) sleep.InitializeEffect(3, g1, g2);

        ScriptableCharm charm = ScriptableCharm.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        StatusEffects se = new StatusEffects();

        // Act
        se.AddEffect(charm1);
        se.AddEffect(sleep1);
        List<bool> isActivated = new List<bool>(){charm1.IsActivated, sleep1.IsActivated};

        // Assert
        Assert.AreEqual(new List<bool>(){false, true}, isActivated);
    }

    /*
    *   Adds 5 different effects to the status effects list. The strongest nonzero cc value, strongest slow, and non-slow 0 cc value effects should be active.
    */
    [Test]
    public void adds_range_of_cc_value_effects_and_sets_0_cc_values_active_and_strongest_nonzero_cc_value_active(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableDrowsy drowsy = ScriptableDrowsy.CreateInstance<ScriptableDrowsy>();
        drowsy.name = "Drowsy1";
        drowsy.duration.AddRange(durationValues);
        drowsy.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        drowsy.slow.duration.AddRange(durationValues);
        drowsy.slow.slowPercent.AddRange(slowValues);
        drowsy.slow.name = "Drowsy1Slow";
        drowsy.sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        drowsy.sleep.duration.AddRange(durationValues);
        Drowsy drowsy1 = (Drowsy) drowsy.InitializeEffect(0, g1, g2);

        ScriptableCharm charm = ScriptableCharm.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        Slow slow1 = CreateSlowEffect("Slow1", 2);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(1, g1, g2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(10f, 1, g1, g2);

        StatusEffects se = new StatusEffects();

        // Act
        se.AddEffect(drowsy1);
        se.AddEffect(drowsy1.drowsySlow);
        se.AddEffect(drowsy1.drowsySleep);
        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);
        se.AddEffect(slow1);
        se.AddEffect(speedBonus1);
        se.AddEffect(dot1);
        List<bool> isActivated = new List<bool>(){drowsy1.IsActivated, drowsy1.drowsySlow.IsActivated, drowsy1.drowsySleep.IsActivated, charm1.IsActivated, 
        charm1.charmSlow.IsActivated, slow1.IsActivated, speedBonus1.IsActivated, dot1.IsActivated};

        // Assert
        Assert.AreEqual(new List<bool>(){true, false, true, false, true, false, true, true}, isActivated);

    }

    /*
    *   Adds multiple 0 cc value effects to the status effects list, only one slow. All the effects should be active.
    */
    [Test]
    public void adds_only_0_cc_values(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        Slow slow1 = CreateSlowEffect("Slow1", 1);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(4, g1, g2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(20f, 3, g1, g2);

        ScriptablePersonalSpell personalSpell1 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell1.name = "PS1";
        personalSpell1.duration.AddRange(durationValues);
        PersonalSpell ps1 = (PersonalSpell) personalSpell1.InitializeEffect(2, g1, g2);

        ScriptablePersonalSpell personalSpell2 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell2.name = "PS2";
        personalSpell2 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell2.duration.AddRange(new List<float>(){-1f});
        PersonalSpell ps2 = (PersonalSpell) personalSpell2.InitializeEffect(0, g1, g2);

        StatusEffects se = new StatusEffects();

        //Act
        se.AddEffect(slow1);
        se.AddEffect(speedBonus1);
        se.AddEffect(dot1);
        se.AddEffect(ps1);
        se.AddEffect(ps2);

        List<bool> isActivated = new List<bool>(){slow1.IsActivated, speedBonus1.IsActivated, dot1.IsActivated, ps1.IsActivated, ps2.IsActivated};

        // Assert
        Assert.AreEqual(new List<bool>(){true, true, true, true, true}, isActivated);
    }

    [Test]
    public void removes_all_effects_besides_personal_spells_from_effects_list(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        Slow slow1 = CreateSlowEffect("Slow1", 2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(10f, 1, g1, g2);

        ScriptablePersonalSpell personalSpell1 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell1.name = "ps1";
        personalSpell1.duration.AddRange(new List<float>(){-1f});
        PersonalSpell ps1 = (PersonalSpell) personalSpell1.InitializeEffect(0, g1, g2);

        StatusEffects se = new StatusEffects();

        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);
        se.AddEffect(slow1);
        se.AddEffect(dot1);
        se.AddEffect(ps1);

        // Act
        se.ResetEffects();

        // Assert
        Assert.AreEqual((1, "ps1"), (se.statusEffects.Count, se.statusEffects[0].effectType.name));

    }

    [Test]
    public void checks_for_effect_with_specific_source(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        StatusEffects se = new StatusEffects();

        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);

        // Act
        bool b = se.CheckForEffectWithSource(ScriptableObject.CreateInstance<ScriptableCharm>(), g1);
    
        // Arrange
        Assert.AreEqual(true, b);
    }

    [Test]
    public void checks_for_effect_by_name(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(4, g1, g2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(10f, 3, g1, g2);

        ScriptableSleep sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep.name = "Sleep1";
        sleep.duration.AddRange(durationValues);
        Sleep sleep1 = (Sleep) sleep.InitializeEffect(2, g1, g2);

        StatusEffects se = new StatusEffects();

        se.AddEffect(speedBonus1);
        se.AddEffect(dot1);
        se.AddEffect(sleep1);

        // Act
        bool b = se.CheckForEffectByName(ScriptableObject.CreateInstance<ScriptableDot>(), dot1.effectType.name);

        // Assert
        Assert.AreEqual(true, b);
    }

    [Test]
    public void returns_all_slow_effects_in_status_effects_list(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        Slow slow1 = CreateSlowEffect("Slow1", 2);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);
    
        ScriptableDrowsy drowsy = ScriptableDrowsy.CreateInstance<ScriptableDrowsy>();
        drowsy.name = "Drowsy1";
        drowsy.duration.AddRange(durationValues);
        drowsy.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        drowsy.slow.duration.AddRange(durationValues);
        drowsy.slow.slowPercent.AddRange(slowValues);
        drowsy.slow.name = "Drowsy1Slow";
        drowsy.sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        drowsy.sleep.duration.AddRange(durationValues);
        Drowsy drowsy1 = (Drowsy) drowsy.InitializeEffect(4, g1, g2);

        Slow slow2 = CreateSlowEffect("Slow2", 1);

        StatusEffects se = new StatusEffects();

        se.AddEffect(slow1);
        se.AddEffect(speedBonus1);
        se.AddEffect(drowsy1);
        se.AddEffect(drowsy1.drowsySlow);
        se.AddEffect(slow2);

        // Act
        List<Effect> effects = se.GetEffectsByType(typeof(ScriptableSlow));

        // Assert
        Assert.AreEqual((typeof(ScriptableSlow), typeof(ScriptableSlow), typeof(ScriptableSlow), "Slow1", "Drowsy1Slow", "Slow2"), 
        (effects[0].effectType.GetType(), effects[1].effectType.GetType(), effects[2].effectType.GetType(), 
        effects[0].effectType.name, effects[1].effectType.name, effects[2].effectType.name));

    }

    [Test]
    public void returns_all_effects_in_status_effects_list_with_given_name(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        Slow slow1 = CreateSlowEffect("Slow1", 2);

        Slow slow2 = CreateSlowEffect("SpeedBonus1", 1);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        SpeedBonus speedBonus2 = (SpeedBonus) speedBonus.InitializeEffect(4, g1, g2);

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, g1, g2);

        StatusEffects se = new StatusEffects();

        se.AddEffect(slow1);
        se.AddEffect(slow2);
        se.AddEffect(speedBonus1);
        se.AddEffect(speedBonus2);
        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);

        // Act
        List<Effect> effects = se.GetEffectsByName("SpeedBonus1");

        // Assert
        Assert.AreEqual((typeof(ScriptableSlow), typeof(ScriptableSpeedBonus), typeof(ScriptableSpeedBonus), "SpeedBonus1", "SpeedBonus1", "SpeedBonus1"), 
        (effects[0].effectType.GetType(), effects[1].effectType.GetType(), effects[2].effectType.GetType(), 
        effects[0].effectType.name, effects[1].effectType.name, effects[2].effectType.name));
    }

    [Test]
    public void returns_next_expiring_stack_of_stackable_given_effect(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        SpeedBonus speedBonus2 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        SpeedBonus speedBonus3 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        SpeedBonus speedBonus4 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        Slow slow1 = CreateSlowEffect("Slow1", 3);

        speedBonus1.TimerTick(0.5f);
        speedBonus2.TimerTick(2.5f);
        speedBonus3.TimerTick(3.8f);
        speedBonus4.TimerTick(1.2f);
        slow1.TimerTick(3.8f);

        StatusEffects se = new StatusEffects();

        se.AddEffect(speedBonus1);
        se.AddEffect(speedBonus2);
        se.AddEffect(speedBonus3);
        se.AddEffect(speedBonus4);
        se.AddEffect(slow1);

        // Act
        Effect nextExpiring = se.GetNextExpiringStack(speedBonus1);

        // Assert
        Assert.AreEqual((3.8f, "SpeedBonus1"), (nextExpiring.effectTimer, nextExpiring.effectType.name));
    }

    [Test]
    public void removes_most_impairing_effect_from_effect_list(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        GameObject g3 = new GameObject();
        GameObject g4 = new GameObject();

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(5f, 2, g1, g2);

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(3, g1, g2);

        ScriptableSleep sleep1 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep1.name = "Sleep1";
        sleep1.duration.AddRange(durationValues);
        Sleep s1 = (Sleep) sleep1.InitializeEffect(4, g1, g2);

        ScriptableSleep sleep2 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep2.name = "Sleep2";
        sleep2.duration.AddRange(durationValues);
        Sleep s2 = (Sleep) sleep2.InitializeEffect(4, g3, g2);

        ScriptableSleep sleep3 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep3.name = "Sleep3";
        sleep3.duration.AddRange(durationValues);
        Sleep s3 = (Sleep) sleep3.InitializeEffect(4, g4, g2);

        s3.TimerTick(2f);

        StatusEffects se = new StatusEffects();

        se.AddEffect(dot1);
        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);
        se.AddEffect(s1);
        se.AddEffect(s2);
        se.AddEffect(s3);

        // Act
        se.RemoveEffect(sleep1, g1);
        List<bool> activatedEffects = new List<bool>(){dot1.IsActivated, charm1.IsActivated, charm1.charmSlow.IsActivated, s1.IsActivated, s2.IsActivated, s3.IsActivated};
        bool b = se.statusEffects.Contains(s1);

        // Assert
        List<bool> aE = new List<bool>(){true, false, true, false, false, true};
        Assert.AreEqual((true, false, 5), (aE.SequenceEqual(activatedEffects), b, se.statusEffects.Count));
    }

    [Test]
    public void adds_3_of_a_stackable_speed_bonuses_to_status_effects(){
        // Arrange
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);
        speedBonus.bonusPercent.AddRange(slowValues);

        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);
        SpeedBonus speedBonus2 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);
        SpeedBonus speedBonus3 = (SpeedBonus) speedBonus.InitializeEffect(3, g1, g2);

        StatusEffects se = new StatusEffects();

        se.AddEffect(speedBonus1);
        se.AddEffect(speedBonus2);
        se.AddEffect(speedBonus3);


        // Act
        int count = se.statusEffects.Count;

        // Assert
        Assert.AreEqual(3, count);
    }

    /*
    *   CreateSlowEffect - Creates a slow effect.
    *   @param slowName - Name of the new slow.
    *   @param index - Spell level index
    *   @return Slow - New Slow.
    */
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
