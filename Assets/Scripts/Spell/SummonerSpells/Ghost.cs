using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements the ghost summoner spell. Applies a movement speed bonus to the caster based on level.
*
* @author: Colin Keys
*/
public class Ghost : Spell, IHasCast
{
    new private GhostData spellData;
    private NavMeshAgent navMeshAgent;

    // Called when the script instance is being loaded.
    protected override void Awake(){
        base.Awake();
        IsSummonerSpell = true;
    }

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (GhostData) base.spellData;
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
        navMeshAgent = GetComponent<NavMeshAgent>();
        IsQuickCast = true;
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        player.statusEffects.AddEffect(spellData.speedBonus.InitializeEffect(0, CalculateSpeedBonus(), player, player));
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[0]));
    }

    /*
    * CalculateSpeedBonus - Calculates the bonus to grant.
    * @return float - float of the speed bonus the spell will grant.
    */
    private float CalculateSpeedBonus(){
        return 0.24f + ((0.24f / 17f) * (player.levelManager.Level - 1));
    }
}

