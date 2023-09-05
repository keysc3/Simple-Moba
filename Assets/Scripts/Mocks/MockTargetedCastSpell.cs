using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockTargetedCastSpell : MockSpell, IHasTargetedCast
{
    public void Cast(IUnit unit){
        SpellNum = "Targeted spell casted";
    }
}
