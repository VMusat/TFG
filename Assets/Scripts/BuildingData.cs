using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData
{
    private string _code;

    public BuildingData(string code)
    {
        _code = code;
    }

    public string Code { get => _code; }

}
