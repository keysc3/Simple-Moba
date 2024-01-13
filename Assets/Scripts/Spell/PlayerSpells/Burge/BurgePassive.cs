using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgePassive : Spell
{
    new private BurgePassiveData spellData;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgePassiveData) base.spellData;
        IBasicAttack ba = GetComponent<IBasicAttack>();
        Debug.Log(ba);
        ba.basicAttackHitCallback += Passive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Passive(IUnit hit, IUnit from){
        Debug.Log("BASIC HIT CALLBACK");
    }
}
