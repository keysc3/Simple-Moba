using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayable
{

    bool isDisplayed { get; }

    void DisplayCast();

    void HideCast();
}
