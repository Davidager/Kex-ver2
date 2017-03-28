using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using ProtoBuf;

public class ReadDatabase{


    //public ExampleContainer exampleContainer;
    public static void readDatabase() {
        /*XmlSerializer serializer = new XmlSerializer(typeof(ExampleContainer));
        Stream fileReader = new FileStream(@"C:\Users\David\Documents\GitHub\Kex\Database\xmlTest.txt", FileMode.Open);
        ExampleContainer exampleContainer;
        exampleContainer = serializer.Deserialize(fileReader) as ExampleContainer;
        fileReader.Close();
        Debug.Log(exampleContainer.examples[10].exampleNumber);
        Debug.Log(exampleContainer.examples.Count);*/
        ExampleContainer exampleContainer = Serializer.Deserialize<ExampleContainer>(
            new FileStream(@"C:\Users\David\Documents\GitHub\Kex\DatabaseTest2\xmlTest.proto", FileMode.Open, FileAccess.Read));

        /*var doc = XDocument.Load(@"C:\Users\David\Documents\GitHub\Kex\Database\xmlTest.txt");
        var authors = doc.Root.Elements().Select(x => x.Element("Example"));
        Debug.Log(authors);*/

        /*using (XmlReader reader = XmlReader.Create(new StringReader(System.IO.File.ReadAllText(@"C:\Users\David\Documents\GitHub\Kex\Database\xmlTest.txt"))))
        {
            reader.ReadToFollowing("Example");
            reader.MoveToFirstAttribute();
            string genre = reader.Value;
            //output.AppendLine("The genre value: " + genre);
            Debug.Log(genre);
            //reader.ReadToFollowing("title");
            //output.AppendLine("Content of the title element: " + reader.ReadElementContentAsString());

        }*/


    }
}



/*
 * public XmlSerializer serializer = new XmlSerializer(typeof(ExampleContainer));
        public Stream filereader = new FileStream(@"D:\KEX\Kex - master\Database", FileMode.Open);
        ExampleContainer exampleContainer;
        exampleContainer = serializer.Deserialize(filereader) as ExampleContainer;
        filereader.Close(); */
        

