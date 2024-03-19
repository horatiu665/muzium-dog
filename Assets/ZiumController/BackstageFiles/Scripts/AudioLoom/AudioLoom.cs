using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MinMaxValue
{
    public float min = 0f;
    public float max = 0f;

    public float GetRandomValueFloat()
    {
        return Random.Range(min, max);
    }

    public int GetRandomValueInt()
    {
        return (int)Random.Range((int)min, (int)max);
    }

    public void CheckValues(float minMin, float maxMax)
    {
        if (min < minMin)
            min = minMin;
        max = Mathf.Clamp(max, min, maxMax);
    }
}

[System.Serializable]
public class RangeProperties
{
    [HideInInspector]
    public int myIndex = 0;
    public Track[] playableTracks;

    public MinMaxValue amountPlayingTracks;

    //Values for the 'inverter' (invert the state of a given amount of tracks, but keep the current amount of active tracks)
    public MinMaxValue amountTracksToInvertSimple;
    public MinMaxValue intervalBetweenChangesSimple;

    //Values for the regular changes (inversion + assure a new correct amount of active tracks)

    public MinMaxValue amountTracksToInvertAdvanced;
    public MinMaxValue intervalBetweenChangesAdvanced;

    public MinMaxValue fadeDuration;

    [Range(0f, 1f)]
    public float revertProbability;

    public void AffectPlayableTracks()
    {
        for (int i =0;i<playableTracks.Length;i++)
        {
            playableTracks[i].AddPlayableState(myIndex);
        }
    }
}

public class AudioLoom : MonoBehaviour
{
    public int currentStateIndex = 0;
    public RangeProperties[] rangeProperties;
    [HideInInspector]
    public bool timersGo = false;
    
    Texture2D tex;
    bool isOn = false;
    [HideInInspector]
    public List<Track> tracks;
    float timerChangeTracks = 100f;
    float timerInvertTracksSimple = 100f;
    GUIStyle guiStyle = new GUIStyle();
    [Range(0f,1f)]
    public float globalVolume = 1f;
    public bool displayGUIVolumes;
    public RangeProperties CurrentRangeProperties()
    {
        if (currentStateIndex < 0)
          return null;
        return rangeProperties[currentStateIndex];
    }

    void Start ()
    {
        tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tracks = GetComponentsInChildren<Track>().ToList();
        for (int i = 0; i < rangeProperties.Length;i++)
        {
            rangeProperties[i].myIndex = i;
            rangeProperties[i].AffectPlayableTracks();
        }
        CheckMinMaxValues();
        timerChangeTracks = CurrentRangeProperties().intervalBetweenChangesAdvanced.GetRandomValueFloat();
	}

    void CheckMinMaxValues()
    {
        foreach(RangeProperties rp in rangeProperties)
        {
            rp.amountPlayingTracks.CheckValues(0f, rp.playableTracks.Length);
            rp.amountTracksToInvertAdvanced.CheckValues(0f, rp.playableTracks.Length);
            rp.intervalBetweenChangesAdvanced.CheckValues(0f, float.MaxValue);
            rp.amountTracksToInvertSimple.CheckValues(0f, rp.playableTracks.Length);
            rp.intervalBetweenChangesSimple.CheckValues(0f, float.MaxValue);
            rp.fadeDuration.CheckValues(0f, float.MaxValue);
        }
    }

    void OnGUI()
    {
        if (!displayGUIVolumes || !isOn)
            return;

        for (int i = 0; i < tracks.Count; i++)
        {
            GUI.color = Color.grey;
            GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)i, 200f, 10f), tex);

            if(tracks[i].track.pitch>0f)
                GUI.color = Color.green;
            else
                GUI.color = Color.red;
   
            GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)i, tracks[i].track.volume * 200f, 10f), tex);
        }

        GUI.color = Color.grey;
        GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)tracks.Count+45f, 200f, 10f), tex);
        GUI.color = Color.blue;
        GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)tracks.Count + 45f, MyHelper.Map(timerChangeTracks,0f, CurrentRangeProperties().intervalBetweenChangesAdvanced.max,0f,200f), 10f), tex);
        GUI.TextArea(new Rect(10f + 205f, 10f + 15f * (float)tracks.Count + 45f, 200f, 10f), "TIMER GREATER CHANGE", guiStyle);

        GUI.color = Color.grey;
        GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)tracks.Count + 60f, 200f, 10f), tex);
        GUI.color = Color.blue;
        GUI.DrawTexture(new Rect(10f, 10f + 15f * (float)tracks.Count + 60f, MyHelper.Map(timerInvertTracksSimple, 0f, CurrentRangeProperties().intervalBetweenChangesSimple.max, 0f, 200f), 10f), tex);
        GUI.TextArea(new Rect(10f + 205f, 10f + 15f * (float)tracks.Count + 60f, 200f, 10f), "TIMER SIMPLE INVERSION", guiStyle);
    }

    void Update ()
    {
        if (isOn)
        {
            if(timersGo)
            {
                timerChangeTracks -= Time.deltaTime;
                timerInvertTracksSimple -= Time.deltaTime;

                if (timerChangeTracks <= 0f)
                {
                    ChangeAdvanced();
                }

                if (timerInvertTracksSimple <= 0f)
                {
                    ChangeSimple();
                }
            }
        }
    }

    public void ChangeAdvanced()
    {
        timerChangeTracks = CurrentRangeProperties().intervalBetweenChangesAdvanced.GetRandomValueFloat();
        timerInvertTracksSimple = CurrentRangeProperties().intervalBetweenChangesSimple.GetRandomValueFloat();
        FadeTracksAdvanced();
    }

    public void ChangeSimple()
    {
        timerInvertTracksSimple = CurrentRangeProperties().intervalBetweenChangesSimple.GetRandomValueFloat();
        FadeTracks(CurrentRangeProperties().amountTracksToInvertSimple.GetRandomValueInt(), GetAmountOfActiveTracks());
    }

    public void Activate(bool state)
    {
        if(state)
        {
            isOn = true;
            timersGo = true;
            timerChangeTracks = CurrentRangeProperties().amountTracksToInvertAdvanced.GetRandomValueFloat();
            timerInvertTracksSimple = CurrentRangeProperties().amountTracksToInvertSimple.GetRandomValueFloat();
        }
        else
        {
            isOn = false;
            timersGo = false;
        }
    }
   
    public int GetAmountOfActiveTracks()
    {
        int playing = 0;
        foreach (Track t in tracks)
            if (t.isPlayingOrFadingIn())
                playing++;
        return playing;
    }
    
    public List<Vector2Int> ExchangeStates(List<Vector2Int> listStates, int amount)
    {
        amount = Mathf.Min(amount, listStates.Count);
        for (int i=0;i< amount; i++)
        {
            if (listStates[i][1] > 0)
                listStates[i] = new Vector2Int(listStates[i][0], -1);
            else if(tracks[(int)listStates[i][0]].CheckDepthCondition(currentStateIndex))
                listStates[i] = new Vector2Int(listStates[i][0], 1);
            else
                Debug.Log("unavailable");
        }

        
        return listStates;
    }

    public List<Vector2Int> CorrectAmountOfTracks(List<Vector2Int> listStates, int correctAmount)
    {
        correctAmount = Mathf.Min(correctAmount, listStates.Count);
        int currentAmountActiveTracks = 0;
        for (int i = 0; i < listStates.Count; i++)
        {
            if (listStates[i][1]>0)
                currentAmountActiveTracks++;
        }

        if (currentAmountActiveTracks != correctAmount)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                int newValue = listStates[i][1];
                if (listStates[i][1] < 0 && currentAmountActiveTracks < correctAmount && tracks[(int)listStates[i][0]].CheckDepthCondition(currentStateIndex))
                    newValue = 1;
                else if (listStates[i][1] > 0 && currentAmountActiveTracks > correctAmount)
                    newValue = -1;
                

                if(listStates[i][1]!=newValue)
                {
                    listStates[i] = new Vector2Int(listStates[i][0], newValue);
                    currentAmountActiveTracks += newValue;
                    if (currentAmountActiveTracks == correctAmount)
                        break;
                }
            }
        }
        return listStates;
    }



    public void FadeTracks(int amountToExchange, int nextAmount)
    {
        List<Vector2Int> listStates = new List<Vector2Int>();


        for(int i=0;i<tracks.Count;i++)
        {
            if(tracks[i].CheckDepthCondition(currentStateIndex))
            {
                int val = tracks[i].isPlayingOrFadingIn() ? 1 : -1;
                listStates.Add(new Vector2Int(i, val));
            }
            else
            {
                tracks[i].FadeOut(1f);
            }
        }
            
        MyHelper.Shuffle(listStates);
        listStates = new List<Vector2Int>(ExchangeStates(listStates, amountToExchange));

        MyHelper.Shuffle(listStates);
        listStates = new List<Vector2Int>(CorrectAmountOfTracks(listStates, nextAmount));

        for (int i = 0; i < listStates.Count; i++)
        {
            float thisFadeDuration = CurrentRangeProperties().fadeDuration.GetRandomValueFloat();
            if (listStates[i][1] > 0)
                tracks[(int)listStates[i][0]].FadeIn(thisFadeDuration);
            else
                tracks[(int)listStates[i][0]].FadeOut(thisFadeDuration);
        }
    }

    public void FadeTracksAdvanced()
    {
        FadeTracks(CurrentRangeProperties().amountTracksToInvertAdvanced.GetRandomValueInt(), CurrentRangeProperties().amountPlayingTracks.GetRandomValueInt());
    }

    public void FullFadeIn(float duration = -1f)
    {
        foreach(Track t in tracks)
        {
            t.FadeIn(duration);
        }
    }

    public void FullFadeOut(float duration = -1f)
    {
        foreach (Track t in tracks)
        {
            t.FadeOut(duration,true);
        }
    }

    public void ChangeCurrentIndex(int val)
    {
        if (val < 0 || val > rangeProperties.Length - 1)
            return;

        currentStateIndex = val;
        ChangeAdvanced();
    }
}
