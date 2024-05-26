using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class DogGameManager : MonoBehaviour
{

    [DebugButton]
    public void SetNormalMode()
    {
        DogCastleReferences.instance.dogControlPanel.startDogEnabled = false;
        DogCastleReferences.instance.dogControlPanel.skipIntro = false;
        DogCastleReferences.instance.dogConcert.dogConcertHideShow.initConcertState = DogConcertHideShow.ConcertState.Playing;
        DogCastleReferences.instance.dogConcert.useCheat = false;
        DogCastleReferences.instance.dogConcert.cheatSkipSecondsOnKey = false;
        DogCastleReferences.instance.dogConcert.candleSystem.useCheatToCompleteArtworks = false;
        PoemSystem.instance.togglePoemWithO = false;

        WriteLog(false);
        SetDirty();

    }

    [DebugButton]
    public void SetConcertDebugMode()
    {

        DogCastleReferences.instance.dogControlPanel.startDogEnabled = false;
        DogCastleReferences.instance.dogControlPanel.skipIntro = false;
        DogCastleReferences.instance.dogConcert.dogConcertHideShow.initConcertState = DogConcertHideShow.ConcertState.Playing;
        DogCastleReferences.instance.dogConcert.useCheat = true;
        DogCastleReferences.instance.dogConcert.cheatSkipSecondsOnKey = true;
        DogCastleReferences.instance.dogConcert.candleSystem.useCheatToCompleteArtworks = true;
        PoemSystem.instance.togglePoemWithO = false;

        WriteLog(true);
        SetDirty();
    }

    [DebugButton]
    public void SetDebugMode()
    {

        DogCastleReferences.instance.dogControlPanel.startDogEnabled = true;
        DogCastleReferences.instance.dogControlPanel.skipIntro = true;
        DogCastleReferences.instance.dogConcert.dogConcertHideShow.initConcertState = DogConcertHideShow.ConcertState.Hidden;
        DogCastleReferences.instance.dogConcert.useCheat = true;
        DogCastleReferences.instance.dogConcert.cheatSkipSecondsOnKey = true;
        DogCastleReferences.instance.dogConcert.candleSystem.useCheatToCompleteArtworks = true;
        PoemSystem.instance.togglePoemWithO = true;

        WriteLog(true);
        SetDirty();

    }

    private void WriteLog(bool debugMode)
    {
        Debug.Log("Debug mode: " + debugMode, this);

        Debug.Log("Start dog enabled: " + DogCastleReferences.instance.dogControlPanel.startDogEnabled, DogCastleReferences.instance.dogControlPanel.gameObject);
        Debug.Log("Skip intro: " + DogCastleReferences.instance.dogControlPanel.skipIntro, DogCastleReferences.instance.dogControlPanel.gameObject);
        Debug.Log("Concert state: " + DogCastleReferences.instance.dogConcert.dogConcertHideShow.initConcertState, DogCastleReferences.instance.dogConcert.gameObject);
        Debug.Log("Concert cheat: " + DogCastleReferences.instance.dogConcert.useCheat, DogCastleReferences.instance.dogConcert.gameObject);
        Debug.Log("Poem toggle with O: " + PoemSystem.instance.togglePoemWithO, PoemSystem.instance.gameObject);
        Debug.Log("Candle cheat: " + DogCastleReferences.instance.dogConcert.candleSystem.useCheatToCompleteArtworks, DogCastleReferences.instance.dogConcert.candleSystem.gameObject);

    }

    private void SetDirty()
    {

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(DogCastleReferences.instance.dogControlPanel);
        UnityEditor.EditorUtility.SetDirty(DogCastleReferences.instance.dogConcert.dogConcertHideShow);
        UnityEditor.EditorUtility.SetDirty(DogCastleReferences.instance.dogConcert);
        UnityEditor.EditorUtility.SetDirty(PoemSystem.instance);
        UnityEditor.EditorUtility.SetDirty(DogCastleReferences.instance.dogConcert.candleSystem);

#endif

    }
}
