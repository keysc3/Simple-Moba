using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    public ChampionSpells championSpells;

    // Start is called before the first frame update
    void Start()
    {
        championSpells = GetComponent<ChampionSpells>();
    }

    // Update is called once per frame
    void Update()
    {
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            championSpells.spell1.Cast();
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            championSpells.spell2.Cast();
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            championSpells.spell3.Cast();
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            championSpells.spell4.Cast();
        }
    }
}
