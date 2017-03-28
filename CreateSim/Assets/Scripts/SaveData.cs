using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class SaveData {

    public static ExampleContainer exampleContainer = new ExampleContainer();

    public static void addExampleData(ExampleData data) {
        exampleContainer.examples.Add(data);
    }

    public static void save(string path, ExampleContainer examples)
    {
        saveExamples(path, examples);
    }

    private static void saveExamples(string path, ExampleContainer examples)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ExampleContainer));
        FileStream stream = new FileStream(path, FileMode.Truncate);
        serializer.Serialize(stream, examples);
        stream.Close();
    }
}
