using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParserPoems : BobbinParserGeneric
{

    public List<string> poems = new List<string>();
    public bool autoAddToPoemSystem = true;

    public override void ParsingLine(int rowNumber, string[] splitLine)
    {
        if (rowNumber == 0)
            return;

        var poem = splitLine[0];

        // remove everything after [date]
        poem = poem.Split('[')[0];


        poems.Add(poem);

    }

    public override void ReadEmAll()
    {
        poems.Clear();
        
        base.ReadEmAll();

        if (autoAddToPoemSystem)
        {
            PoemSystem.instance.allPoems.Clear();
            PoemSystem.instance.allPoems.AddRange(poems);
        }


    }



}
