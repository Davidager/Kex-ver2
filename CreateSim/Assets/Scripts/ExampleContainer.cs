using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot("ExampleCollection")]
public class ExampleContainer {
    [XmlArray("Examples")]
    [XmlArrayItem("Example")]
    public List<ExampleData> examples = new List<ExampleData>();
	
}
