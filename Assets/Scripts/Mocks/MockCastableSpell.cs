using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockCastableSpell : MockSpell, IHasCast
{
    public void Cast(){
        SpellNum = "Casted";
    }
}
