using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using NSubstitute;

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
        StatusEffects se = new StatusEffects(null);
        
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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();

        ScriptableSleep sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep.name = "Sleep1";
        sleep.duration.AddRange(durationValues);
        Sleep sleep1 = (Sleep) sleep.InitializeEffect(3, unit1, unit2);

        ScriptableCharm charm = ScriptableCharm.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, unit1, unit2);

        StatusEffects se = new StatusEffects(null);

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();

        ScriptableDrowsy drowsy = ScriptableDrowsy.CreateInstance<ScriptableDrowsy>();
        drowsy.name = "Drowsy1";
        drowsy.duration.AddRange(durationValues);
        drowsy.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        drowsy.slow.duration.AddRange(durationValues);
        drowsy.slow.slowPercent.AddRange(slowValues);
        drowsy.slow.name = "Drowsy1Slow";
        drowsy.sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        drowsy.sleep.duration.AddRange(durationValues);
        Drowsy drowsy1 = (Drowsy) drowsy.InitializeEffect(0, unit1, unit2);

        ScriptableCharm charm = ScriptableCharm.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, unit1, unit2);

        Slow slow1 = CreateSlowEffect("Slow1", 2);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(1, 1f, unit1, unit2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(10f, 1, unit1, unit2);

        StatusEffects se = new StatusEffects(null);

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();

        Slow slow1 = CreateSlowEffect("Slow1", 1);

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(4, 1f, unit1, unit2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(20f, 3, unit1, unit2);

        ScriptablePersonalSpell personalSpell1 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell1.name = "PS1";
        personalSpell1.duration.AddRange(durationValues);
        PersonalSpell ps1 = (PersonalSpell) personalSpell1.InitializeEffect(2, unit1, unit2);

        ScriptablePersonalSpell personalSpell2 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell2.name = "PS2";
        personalSpell2 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell2.duration.AddRange(new List<float>(){-1f});
        PersonalSpell ps2 = (PersonalSpell) personalSpell2.InitializeEffect(0, unit1, unit2);

        StatusEffects se = new StatusEffects(null);

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();
        List<Effect> myEffects  = new List<Effect>();

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, unit1, unit2);
        myEffects.Add(charm1);
        myEffects.Add(charm1.charmSlow);

        myEffects.Add(CreateSlowEffect("Slow1", 2));

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        myEffects.Add((Dot) dot.InitializeEffect(10f, 1, unit1, unit2));

        ScriptablePersonalSpell personalSpell1 = ScriptableObject.CreateInstance<ScriptablePersonalSpell>();
        personalSpell1.name = "ps1";
        personalSpell1.duration.AddRange(new List<float>(){-1f});
        myEffects.Add((PersonalSpell) personalSpell1.InitializeEffect(0, unit1, unit2));

        StatusEffects se = new StatusEffects(null);

        myEffects.ForEach(e => se.AddEffect(e));

        // Act
        se.ResetEffects();

        // Assert
        Assert.AreEqual((1, "ps1"), (se.statusEffects.Count, se.statusEffects[0].effectType.name));

    }

    [Test]
    public void checks_for_effect_with_specific_source(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, unit1, unit2);

        StatusEffects se = new StatusEffects(null);

        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);

        // Act
        bool hasEffect = se.CheckForEffectWithSource(charm1.effectType, unit1);
    
        // Arrange
        Assert.True(hasEffect);
    }

    [Test]
    public void checks_for_effect_by_name(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        SpeedBonus speedBonus1 = (SpeedBonus) speedBonus.InitializeEffect(4, 1f, unit1, unit2);

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(10f, 3, unit1, unit2);

        ScriptableSleep sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep.name = "Sleep1";
        sleep.duration.AddRange(durationValues);
        Sleep sleep1 = (Sleep) sleep.InitializeEffect(2, unit1, unit2);

        StatusEffects se = new StatusEffects(null);

        se.AddEffect(speedBonus1);
        se.AddEffect(dot1);
        se.AddEffect(sleep1);

        // Act
        bool hasEffect = se.CheckForEffectByName(dot1.effectType.name);

        // Assert
        Assert.True(hasEffect);
    }

    [Test]
    public void returns_all_slow_effects_in_status_effects_list(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();
        List<Effect> myEffects  = new List<Effect>();

        myEffects.Add(CreateSlowEffect("Slow1", 2));

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.duration.AddRange(durationValues);
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
    
        ScriptableDrowsy drowsy = ScriptableDrowsy.CreateInstance<ScriptableDrowsy>();
        drowsy.name = "Drowsy1";
        drowsy.duration.AddRange(durationValues);
        drowsy.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        drowsy.slow.duration.AddRange(durationValues);
        drowsy.slow.slowPercent.AddRange(slowValues);
        drowsy.slow.name = "Drowsy1Slow";
        drowsy.sleep = ScriptableObject.CreateInstance<ScriptableSleep>();
        drowsy.sleep.duration.AddRange(durationValues);
        Drowsy drowsy1 = (Drowsy) drowsy.InitializeEffect(4, unit1, unit2);
        myEffects.Add(drowsy1);
        myEffects.Add(drowsy1.drowsySlow);

        myEffects.Add(CreateSlowEffect("Slow2", 1));

        StatusEffects se = new StatusEffects(null);

        myEffects.ForEach(e => se.AddEffect(e));

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();
        List<Effect> myEffects  = new List<Effect>();

        myEffects.Add(CreateSlowEffect("Slow1", 2));

        myEffects.Add(CreateSlowEffect("SpeedBonus1", 1));

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f,unit1, unit2));

        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(4, 1f, unit1, unit2));

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(4, unit1, unit2);
        myEffects.Add(charm1);
        myEffects.Add(charm1.charmSlow);

        StatusEffects se = new StatusEffects(null);

        myEffects.ForEach(e => se.AddEffect(e));

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        List<Effect> myEffects  = new List<Effect>();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);

        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add(CreateSlowEffect("Slow1", 3));

        myEffects[0].TimerTick(0.5f);
        myEffects[1].TimerTick(2.5f);
        myEffects[2].TimerTick(3.8f);
        myEffects[3].TimerTick(1.2f);
        myEffects[4].TimerTick(3.8f);

        StatusEffects se = new StatusEffects(null);

        myEffects.ForEach(e => se.AddEffect(e));

        // Act
        Effect nextExpiring = se.GetNextExpiringStack(myEffects[0]);

        // Assert
        Assert.AreEqual((3.8f, "SpeedBonus1"), (nextExpiring.effectTimer, nextExpiring.effectType.name));
    }

    [Test]
    public void removes_most_impairing_effect_from_effect_list(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = CreateMockUnit();
        IUnit unit3 = Substitute.For<IUnit>();
        IUnit unit4 = Substitute.For<IUnit>();

        ScriptableDot dot = ScriptableObject.CreateInstance<ScriptableDot>();
        dot.name = "Dot1";
        dot.duration.AddRange(durationValues);
        Dot dot1 = (Dot) dot.InitializeEffect(5f, 2, unit1, unit2);

        ScriptableCharm charm = ScriptableObject.CreateInstance<ScriptableCharm>();
        charm.name = "Charm1";
        charm.duration.AddRange(durationValues);
        charm.slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        charm.slow.name = "Charm1Slow";
        charm.slow.duration.AddRange(durationValues);
        charm.slow.slowPercent.AddRange(slowValues);
        Charm charm1 = (Charm) charm.InitializeEffect(3, unit1, unit2);

        ScriptableSleep sleep1 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep1.name = "Sleep1";
        sleep1.duration.AddRange(durationValues);
        Sleep s1 = (Sleep) sleep1.InitializeEffect(4, unit1, unit2);

        ScriptableSleep sleep2 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep2.name = "Sleep2";
        sleep2.duration.AddRange(durationValues);
        Sleep s2 = (Sleep) sleep2.InitializeEffect(4, unit3, unit2);

        ScriptableSleep sleep3 = ScriptableObject.CreateInstance<ScriptableSleep>();
        sleep3.name = "Sleep3";
        sleep3.duration.AddRange(durationValues);
        Sleep s3 = (Sleep) sleep3.InitializeEffect(4, unit4, unit2);

        s3.TimerTick(2f);

        StatusEffects se = new StatusEffects(null);

        se.AddEffect(dot1);
        se.AddEffect(charm1);
        se.AddEffect(charm1.charmSlow);
        se.AddEffect(s1);
        se.AddEffect(s2);
        se.AddEffect(s3);

        // Act
        se.RemoveEffect(sleep1, unit1);
        List<bool> activatedEffects = new List<bool>(){dot1.IsActivated, charm1.IsActivated, charm1.charmSlow.IsActivated, s1.IsActivated, s2.IsActivated, s3.IsActivated};
        bool b = se.statusEffects.Contains(s1);

        // Assert
        List<bool> aE = new List<bool>(){true, false, true, false, false, true};
        Assert.AreEqual((true, false, 5), (aE.SequenceEqual(activatedEffects), b, se.statusEffects.Count));
    }

    [Test]
    public void adds_3_of_a_stackable_speed_bonuses_to_status_effects(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        List<Effect> myEffects  = new List<Effect>();

        ScriptableSpeedBonus speedBonus = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        speedBonus.name = "SpeedBonus1";
        speedBonus.isStackable = true;
        speedBonus.duration.AddRange(durationValues);

        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));
        myEffects.Add((SpeedBonus) speedBonus.InitializeEffect(3, 1f, unit1, unit2));

        StatusEffects se = new StatusEffects(null);

        myEffects.ForEach(e => se.AddEffect(e));

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
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        ScriptableSlow slow = ScriptableObject.CreateInstance<ScriptableSlow>();
        slow.name = slowName;
        slow.duration.AddRange(durationValues);
        slow.slowPercent.AddRange(slowValues);
        return (Slow) slow.InitializeEffect(index, unit1, unit2);
    }

    public IUnit CreateMockUnit(){
        IUnit unit = Substitute.For<IUnit>();
        unit.GameObject.Returns(new GameObject());
        return unit;
    }
}
