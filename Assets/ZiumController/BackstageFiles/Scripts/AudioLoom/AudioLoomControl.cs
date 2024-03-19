using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoomControl : MonoBehaviour
{
    AudioLoom myAudioLoom;
    public bool launchAtStart;
    public bool randomChanges;
    public MinMaxValue delayChange;
    public float timeBeforeChange = 99999f;

    // Use this for initialization
    void Start ()
    {

        myAudioLoom = GetComponent<AudioLoom>();

        if (myAudioLoom == null)
            return;

        if(launchAtStart)
        {
            myAudioLoom.Activate(true);
            myAudioLoom.ChangeAdvanced();
            myAudioLoom.FadeTracksAdvanced();
        }
        delayChange.CheckValues(0f, float.PositiveInfinity);
        timeBeforeChange = delayChange.GetRandomValueFloat();


	}
	
	// Update is called once per frame
	void Update ()
    {
        if (myAudioLoom == null)
            return;

        if (randomChanges)
        {
            timeBeforeChange -= Time.deltaTime;
            if(timeBeforeChange<=0f)
            {
                timeBeforeChange = delayChange.GetRandomValueFloat();
                myAudioLoom.ChangeCurrentIndex((int)Random.Range(0, myAudioLoom.rangeProperties.Length));
            }
        }
    }



}
