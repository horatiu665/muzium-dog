using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PoemSystem : MonoBehaviour
{
    private static PoemSystem _instance;
    public static PoemSystem instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoemSystem>();
            }
            return _instance;
        }
    }

    private bool poemVisible = false;

    public PlayableDirector timeline;

    public TimelineAsset poemShow;
    public TimelineAsset poemHide;

    private RectTransform dogBoneParent => (RectTransform)dogBone.parent;
    public RectTransform dogBone;
    public float dogBoneAnimDuration = 0.5f;
    public AnimationCurve dogBoneAnimScale = AnimationCurve.EaseInOut(0, 1, 1, 1.5f);
    public AnimationCurve dogBoneAnimColor = AnimationCurve.EaseInOut(0, 1, 1, 1.5f);

    public List<string> allPoems = new List<string>();
    private List<int> usedPoems = new List<int>();

    public TextMeshProUGUI poemText;

    public bool togglePoemWithO = false;

    public bool closePoemWithP = false;

    public event System.Action OnPoemHidden;


    [DebugButton]
    public void SetRandomDogBone()
    {
        dogBone.anchoredPosition = new Vector2(Random.Range(-dogBoneParent.rect.width / 2, dogBoneParent.rect.width / 2), Random.Range(-dogBoneParent.rect.height / 2, dogBoneParent.rect.height / 2));
        var finalRotation = Quaternion.Euler(0, 0, Random.Range(-70, 130));
        var initRotation = finalRotation * Quaternion.Euler(0, 0, Random.Range(-180, 180));
        dogBone.rotation = initRotation;

        Image dogBoneImage = dogBone.GetComponent<Image>();

        if (Application.isPlaying)
        {
            StartCoroutine(pTween.To(dogBoneAnimDuration, t =>
            {
                var tt = t * t;
                dogBone.rotation = Quaternion.Slerp(initRotation, finalRotation, tt);
                dogBone.localScale = Vector3.one * dogBoneAnimScale.Evaluate(t);
                var c = dogBoneImage.color;
                c.a = dogBoneAnimColor.Evaluate(t);
                dogBoneImage.color = c;
            }));
        }
    }

    private void OnEnable()
    {
        // hide poem instantly
        timeline.playableAsset = poemShow;
        timeline.Play();
        // pause in that unity glitchy way
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(0);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && togglePoemWithO)
        {
            TogglePoemVisible();
        }

        if (poemVisible && Input.GetKeyDown(KeyCode.P) && closePoemWithP)
        {
            SetPoemVisible(false);
        }
    }

    public void ShowCustomText(string text, bool visible)
    {
        poemVisible = visible;

        if (visible)
        {
            // hide bone.
            Image dogBoneImage = dogBone.GetComponent<Image>();
            dogBoneImage.color = new Color(1, 1, 1, 0);

            poemText.text = text;
            timeline.playableAsset = poemShow;
        }
        else
        {
            timeline.playableAsset = poemHide;

            OnPoemHidden?.Invoke();
        }
        timeline.Play();
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);


    }

    public void SetPoemVisible(bool visible)
    {
        var unusedPoemIndex = GetUnusedPoemIndex();
        var fullText = allPoems[unusedPoemIndex];
        // todays date
        fullText += "\n  " + System.DateTime.Now.ToString("dd-MM-yyyy");
        fullText += "\n    " + "DOG";
        //poemText.text = fullText;

        ShowCustomText(fullText, visible);
    }

    private int GetUnusedPoemIndex()
    {
        if (usedPoems.Count == allPoems.Count)
        {
            usedPoems.Clear();
        }
        var unusedPoemIndex = Random.Range(0, allPoems.Count);
        var monteCarlo = 1000;
        while (usedPoems.Contains(unusedPoemIndex) && monteCarlo-- > 0)
        {
            unusedPoemIndex = Random.Range(0, allPoems.Count);
        }
        usedPoems.Add(unusedPoemIndex);
        return unusedPoemIndex;
    }

    [DebugButton]
    public void TogglePoemVisible()
    {
        SetPoemVisible(!poemVisible);
    }

}
