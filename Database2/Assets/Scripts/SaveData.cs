using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ProtoBuf;

public class SaveData
{

    public static ExampleContainer exampleContainer = new ExampleContainer();
    public static ExampleContainer noInfExamplecontainer = new ExampleContainer();

    public static void addExampleData(ExampleData data)
    {
        exampleContainer.examples.Add(data);
    }

    public static void addNoInfExampleData(ExampleData data)
    {
        noInfExamplecontainer.examples.Add(data);
    }

    public static void save(string path, DatabaseWrapper sortedExampleConfigurations)
    {
        saveExamples(path, sortedExampleConfigurations);
    }

    private static void saveExamples(string path, DatabaseWrapper sortedExampleConfigurations)
    {
        //XmlSerializer serializer = new XmlSerializer(typeof(ExampleContainer));
        try
        {
            FileStream stream = new FileStream(path, FileMode.CreateNew);
            Serializer.Serialize<DatabaseWrapper>(stream, sortedExampleConfigurations);
            stream.Close();
        }
        catch (IOException e)
        {
            FileStream stream = new FileStream(path, FileMode.Truncate);
            Serializer.Serialize<DatabaseWrapper>(stream, sortedExampleConfigurations);
            stream.Close();
        }

    }
}
