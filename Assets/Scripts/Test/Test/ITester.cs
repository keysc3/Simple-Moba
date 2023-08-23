using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Testing();

public interface ITester
{
    Testing testing { get; set; }
    float value1 { get; set; }
}
