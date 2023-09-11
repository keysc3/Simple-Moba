using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class user_interface
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_mana_ui_with_null_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        Slider slider = CreateSlider(0.8f, parent);
        MockPlayer player = new MockPlayer();
        player.unitStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        UpdateManaUI manaUI = parent.AddComponent<UpdateManaUI>();
        slider.value = 0.1f;
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(0.1f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_mana_ui_with_dead_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        Slider slider = CreateSlider(50f, parent);
        MockPlayer player = new MockPlayer();
        player.IsDead = true;
        player.unitStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        UpdateManaUI manaUI = parent.AddComponent<UpdateManaUI>();
        manaUI.player = player;
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(50f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator updates_mana_ui()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        Slider slider = CreateSlider(0.8f, parent);
        MockPlayer player = new MockPlayer();
        ChampionStats stats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        stats.maxMana.BaseValue = 100f;
        stats.CurrentMana = 40f;
        player.unitStats = stats;
        UpdateManaUI manaUI = parent.AddComponent<UpdateManaUI>();
        manaUI.player = player;
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(40f, slider.value);
    }

    private Slider CreateSlider(float startingValue, GameObject g1){
        Slider slider = g1.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = startingValue;
        return slider;
    }

    private GameObject CreateParentChild(){
        GameObject parent = new GameObject();
        GameObject child = new GameObject("Value");
        child.AddComponent<TextMeshProUGUI>();
        child.transform.SetParent(parent.transform);
        return parent;
    }
}
