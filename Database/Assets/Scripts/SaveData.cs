using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ProtoBuf;

public class SaveData
{

    public static ExampleContainer exampleContainer = new ExampleContainer();

    public static void addExampleData(ExampleData data)
    {
        exampleContainer.examples.Add(data);
    }

    public static void save(string path, ExampleContainer examples)
    {
        saveExamples(path, examples);
    }

    private static void saveExamples(string path, ExampleContainer examples)
    {
        //XmlSerializer serializer = new XmlSerializer(typeof(ExampleContainer));
        try
        {
            FileStream stream = new FileStream(path, FileMode.CreateNew);
            Serializer.Serialize<ExampleContainer>(stream, examples);
            stream.Close();
        }
        catch (IOException e)
        {
            FileStream stream = new FileStream(path, FileMode.Truncate);
            Serializer.Serialize<ExampleContainer>(stream, examples);
            stream.Close();
        }

    }
}
