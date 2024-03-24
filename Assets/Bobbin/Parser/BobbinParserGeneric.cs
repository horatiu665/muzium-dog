using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using ToyBoxHHH;

public class BobbinParserGeneric : MonoBehaviour
{
    public string pathToFile = "Assets/Bobbin/FromGoogleSheets/BodyParts.tsv";

    [DebugButton]
    public virtual void ReadEmAll()
    {
        var sr = new StreamReader(pathToFile);
        var all = sr.ReadToEnd();
        sr.Close();

        var index = 0;

        // get all lines
        List<string> lines = new List<string>();
        string newLine = "";

        // if 2 spaces we put a newline. HACK.
        var numSpacesHack = 0;
        while (index < all.Length)
        {
            var newChar = all[index];

            // merge newlines for lyrics and things

            // newChar is space. if we find 3 spaces, we need to include a fake newline....
            var doSpaceHack = false;
            if (doSpaceHack)
            {
                if (newChar == ' ')
                {
                    if (numSpacesHack == 0)
                    {
                        // do nothing at first space.
                    }
                    else if (numSpacesHack == 1)
                    {
                        // we found 2 spaces. that means replace them with a double newline...???!?!?
                        newLine = newLine.Substring(0, newLine.Length - 1) + "\n\n";
                    }
                    else
                    {
                        // any more than 2 consecutive spaces we don't add at all...
                        newLine = newLine.Substring(0, newLine.Length - 1);
                    }
                    numSpacesHack++;
                }
                else
                {
                    numSpacesHack = 0;
                }
            }

            // newchar is a n newline char
            if (newChar == '\n')
            {
                newLine = newLine + "\n";
            }
            // newchar is a r newline char
            else if (newChar == '\r')
            {
                if (all.Length > index + 1 && all[index + 1] == '\n')
                {
                    // we want a new entry
                    lines.Add(newLine);
                    newLine = "";
                    index++;
                } 
                else // it's just a \r alone, carriage return. it means add a new line to this entry.
                {
                    newLine += "\n";
                }

                // old code apparently made a new entry when it found \r.
                //lines.Add(newLine);
                //newLine = "";
                //index++;
            }
            else
            {
                newLine = newLine + newChar;
            }

            index++;
        }

        if (!string.IsNullOrWhiteSpace(newLine))
        {
            lines.Add(newLine);
        }


        // read cards and put them in a data struct
        int rowNumber = 0;
        foreach (var line in lines)
        {
            var dokLine = line;

            // a $ represents a simple newline.
            dokLine = dokLine.Replace('$', '\n');

            // last part of double space hack...
            dokLine = dokLine.Replace("\n ", "\n");

            var splitLine = dokLine.Split('\t');

            // now each line is properly split with tabs.

            ParsingLine(rowNumber, splitLine);

            // Here is an example how to get the data into a data structure of your choice.
            // if (false)
            // {
            //     var id = splitLine[0];
            //     var name = splitLine[1];
            //     var cardType = splitLine[2];
            //     var count = 0;
            //     if (!int.TryParse(splitLine[3], out count))
            //     {
            //         count = -1;
            //         Debug.Log("Error on parsing count at column " + 3 + " row " + rowNumber);
            //     }
            // }

            rowNumber += 1;
        }

    }

    public virtual void ParsingLine(int rowNumber, string[] splitLine)
    {
        // override this and parse the lines however u want :)

    }
}
