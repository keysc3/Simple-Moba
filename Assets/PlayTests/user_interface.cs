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
        parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
        Slider slider = CreateSlider(80f, parent);
        UpdateManaUI manaUI = parent.AddComponent<UpdateManaUI>();
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(80f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_mana_ui_with_dead_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
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
        TMP_Text text = parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
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
        Assert.AreEqual((40f, "40/100"), (slider.value, text.text));
    }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_mana_bar_ui_with_null_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        Slider slider = CreateSlider(40f, parent.transform.GetChild(0).gameObject);
        UpdateManaBarUI manaBarUI = parent.transform.GetChild(0).gameObject.AddComponent<UpdateManaBarUI>();
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(40f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_mana_bar_ui_with_dead_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        MockPlayerBehaviour playerScript = parent.AddComponent<MockPlayerBehaviour>();
        playerScript.IsDead = true;
        Slider slider = CreateSlider(35f, parent.transform.GetChild(0).gameObject);
        UpdateManaBarUI manaBarUI = parent.transform.GetChild(0).gameObject.AddComponent<UpdateManaBarUI>();
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(35f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator updates_mana_bar_ui()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        MockPlayerBehaviour playerScript = parent.AddComponent<MockPlayerBehaviour>();
        Slider slider = CreateSlider(86f, parent.transform.GetChild(0).gameObject);
        ChampionStats stats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        stats.maxMana.BaseValue = 100f;
        stats.CurrentMana = 15f;
        playerScript.unitStats = stats;
        UpdateManaBarUI manaBarUI = parent.transform.GetChild(0).gameObject.AddComponent<UpdateManaBarUI>();
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(15f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_update_health_ui_with_null_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
        Slider slider = CreateSlider(80f, parent);
        parent.AddComponent<UpdateHealthUI>();
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(80f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator updates_health_ui_to_0_current_health_from_dead_player()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
        Slider slider = CreateSlider(50f, parent);
        MockPlayer player = new MockPlayer();
        player.IsDead = true;
        player.unitStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        UpdateHealthUI healthUI = parent.AddComponent<UpdateHealthUI>();
        healthUI.player = player;
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual(0f, slider.value);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator updates_health_ui_to_current_health_percent()
    {
        // Use the Assert class to test conditions.
        GameObject parent = CreateParentChild();
        TMP_Text text = parent.transform.GetChild(0).gameObject.AddComponent<TextMeshProUGUI>();
        Slider slider = CreateSlider(64f, parent);
        MockPlayer player = new MockPlayer();
        ChampionStats stats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        stats.maxHealth.BaseValue = 100f;
        stats.CurrentHealth = 98f;
        player.unitStats = stats;
        UpdateHealthUI healthUI = parent.AddComponent<UpdateHealthUI>();
        healthUI.player = player;
        // Use yield to skip a frame.
        yield return null;
        Assert.AreEqual((98f, "98/100"), (slider.value, text.text));
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
        child.transform.SetParent(parent.transform);
        return parent;
    }
}
