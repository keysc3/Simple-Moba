using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class unit_stats
{

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_to_62_with_100_max_health(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 100f;

        // Act
        unitStats.CurrentHealth = 62f;

        // Assert 
        Assert.AreEqual(62f, unitStats.CurrentHealth);

    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_of_42_to_max_health_of_100_from_attempted_current_health_set_of_162(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 100f;
        unitStats.CurrentHealth = 42f;

        // Act
        unitStats.CurrentHealth = 162f;

        // Assert 
        Assert.AreEqual(100f, unitStats.CurrentHealth);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_to_72_with_max_health_100_with_base_50_and_modifier_50(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 50f;
        unitStats.maxHealth.AddModifier(50f);

        // Act
        unitStats.CurrentHealth = 72f;

        // Assert 
        Assert.AreEqual(72f, unitStats.CurrentHealth);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void resets_current_health_of_50_to_max_health_value_of_1032(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 1032f;
        unitStats.CurrentHealth = 50f;

        // Act
        unitStats.ResetHealth();

        // Assert 
        Assert.AreEqual(1032f, unitStats.CurrentHealth);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_attack_speed_to_0x876875_from_base_0x625_with_bonus_attack_speed_modifiers(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.attackSpeed.BaseValue = 0.625f;
        unitStats.bonusAttackSpeed.BaseValue = 7.7f;
        unitStats.bonusAttackSpeed.AddModifier(26.4f);
        unitStats.bonusAttackSpeed.AddModifier(6.2f);

        // Act
        unitStats.UpdateAttackSpeed();

        // Assert 
        Assert.AreEqual(0.876875f, unitStats.attackSpeed.GetValue());
    }

    // A Test behaves as an ordinary method
    [Test]
    public void gets_speed_with_multiple_speed_bonuses_and_slows(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.speed.BaseValue = 2.1f;
        unitStats.speed.AddModifier(0.4f);
        
        List<float> slowValues  = new List<float>(){0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
        List<float> durationValues = new List<float>(){1f, 2f, 3f, 4f, 5f};
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        StatusEffects se = new StatusEffects();
        List<Effect> myEffects  = new List<Effect>();

        for(int i = 0; i < 3; i++){
            ScriptableSlow slow = ScriptableObject.CreateInstance<ScriptableSlow>();
            slow.name = "Slow" + i;
            slow.duration.AddRange(durationValues);
            slow.slowPercent.AddRange(slowValues);
            myEffects.Add((Slow) slow.InitializeEffect((i+2)%slowValues.Count, unit1, unit2));
        }

        for(int i = 0; i < 3; i ++){
            myEffects.Add(CreateSpeedBonusEffect("AdditiveStackable", 3, true, true));
        }
        myEffects.Add(CreateSpeedBonusEffect("Additive1", 3, true, false));
        myEffects.Add(CreateSpeedBonusEffect("Additive2", 4, true, false));
        myEffects.Add(CreateSpeedBonusEffect("Additive3", 1, true, false));
        myEffects.Add(CreateSpeedBonusEffect("Multi1", 0, false, false));
        myEffects.Add(CreateSpeedBonusEffect("Multi2", 1, false, false));
        
        myEffects.ForEach(e => se.AddEffect(e));

        // Act
        float finalMS = unitStats.CalculateMoveSpeed(se);

        // Assert 
        Assert.AreEqual(5.4236875f, finalMS);
    }

    /*
    *   CreateSpeedBonusEffect - Creates a slow effect.
    *   @param speedBonusName - Name of the new speed bonus.
    *   @param index - Spell level index.
    *   @param isAdditive - bool for isAdditive field.
    *   @param isStackable - bool for isStackable field.
    *   @return SpeedBonus - New SpeeBonus.
    */
    public SpeedBonus CreateSpeedBonusEffect(string speedBonusName, int index, bool isAdditive, bool isStackable){
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        List<float> sbValues  = new List<float>(){0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
        List<float> durationValues = new List<float>(){1f, 2f, 3f, 4f, 5f};
        ScriptableSpeedBonus sb = ScriptableObject.CreateInstance<ScriptableSpeedBonus>();
        sb.name = speedBonusName;
        sb.isStackable = isStackable;
        sb.isAdditive = isAdditive;
        sb.duration.AddRange(durationValues);
        return (SpeedBonus) sb.InitializeEffect(index, sbValues[index], unit1, unit2);
    } 
}
