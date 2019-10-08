using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyMaster
{
    int keyCount { get; set; }
    int GetFacing();
}