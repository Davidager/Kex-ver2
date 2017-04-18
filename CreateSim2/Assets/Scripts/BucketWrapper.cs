
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class BucketWrapper {
    [ProtoMember(1)]
    public List<Configuration> bucket;

    public BucketWrapper(List<Configuration> bucket)
    {
        this.bucket = bucket;
    }
    public BucketWrapper()
    {
        this.bucket = new List<Configuration>();
    }
}

[Serializable]
[ProtoContract]
public class DatabaseWrapper
{
    [ProtoMember(1)]
    public List<BucketWrapper> sortedExampleConfigurations;

    [ProtoMember(2)]
    public Configuration[] exampleConfigurations;
}
