using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using System;

[Serializable]
[ProtoContract]
public class ExampleContainer
{
    [ProtoMember(1)]
    public List<ExampleData> examples = new List<ExampleData>();

}