using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [SerializeField] private GameObject fovDisplay;
    public GameObject reticle;
    public GameObject soundTriggerText;
    public GameObject plaqueText;
    [SerializeField] private GameObject pauseMenu;
    public bool paused;

    [SerializeField] private GameObject firstLayout;
    [SerializeField] private GameObject secondLayout;

    [SerializeField] private TextMeshProUGUI catalogueMsg;
    [SerializeField] private GameObject[] pages;
    private int pageIndex;
    public List<ArtworkTile> discoveredArtworks;
    public int artworkIndex;
    public int listCount;
    
    void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name != "MainMenu") 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        for (int i=0; i<100; i++) discoveredArtworks.Add(null);
        
    }

    float fovDisplayTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused) pauseMenu.SetActive(true);
            else {
                pauseMenu.SetActive(false);
                SetActiveAllChildren(secondLayout.transform, false);
                firstLayout.SetActive(true);
            }
            paused = !paused;
        }

        if (paused || SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        fovDisplayTimer -= Time.deltaTime;
        if (fovDisplayTimer < 0) fovDisplay.SetActive(false);
        fovDisplay.GetComponent<TextMeshProUGUI>().color = new Color(255,255,255,Mathf.Lerp(0, 255, fovDisplayTimer));
    }

    public void ResumeGame()
    {
        paused = false;
        pauseMenu.SetActive(false);
    }
    public void DisplayFov(float fov)
    {
        fovDisplayTimer = 1.4f;
        fovDisplay.SetActive(true);

        fovDisplay.GetComponent<TextMeshProUGUI>().text = "fov : " + Mathf.Round(fov);
    }

    public void GoBack()
    {
        if (!firstLayout.activeSelf)
        {
            SetActiveAllChildren(secondLayout.transform, false);
            firstLayout.SetActive(true);
        }
        else if (firstLayout.activeSelf)
        {
            SetActiveAllChildren(secondLayout.transform, false);
            ResumeGame();
        }
    }

    public void CloseMenu()
    {

        GoBack();
        GoBack();

    }

    public void CatalogueUpdateMessage()
    {
        
        StartCoroutine(CatalogueMsg(catalogueMsg));

        
    }

    IEnumerator CatalogueMsg(TextMeshProUGUI messageText)
    {
        pauseMenu.transform.localScale = Vector3.zero;
        pauseMenu.SetActive(true);
        yield return new WaitForSeconds(0.0001f);
        pauseMenu.SetActive(false);
        pauseMenu.transform.localScale = Vector3.one;
        StartCoroutine(FadeInText(catalogueMsg));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeOutText(catalogueMsg));
    }

    private IEnumerator FadeInText(TextMeshProUGUI messageText)
    {
        messageText.gameObject.SetActive(true);
        float rate = 1.0f / 0.5f;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = messageText.color;
            messageText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(0, 1, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FadeOutText(TextMeshProUGUI messageText)
    {
        float startAlpha = messageText.color.a;
        float rate = 1.0f / 0.5f;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = messageText.color;
            messageText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        messageText.gameObject.SetActive(false);
    }

    private void SetActiveAllChildren(Transform transform, bool value) { foreach (Transform child in transform) { child.gameObject.SetActive(value); } }

    public void NextPage()
    {
        if (!firstLayout.activeSelf)
        {
            
            if (artworkIndex < listCount) {
                artworkIndex ++;
                for (int i=artworkIndex; i < listCount; i++)
                {
                    if (discoveredArtworks[i] != null) break;
                    else artworkIndex ++;
                }
            }
            else artworkIndex = 0;
            SetActiveAllChildren(secondLayout.transform, false);
            discoveredArtworks[artworkIndex].OpenDetailsFromScript();

        }
        else 
        {
            if (pageIndex < pages.Length - 1) pageIndex ++;
            else pageIndex = 0;

            UpdatePage();
        }
    }

        

    public void PreviousPage()
    {
        if (!firstLayout.activeSelf)
        {
            
            
            if (artworkIndex > 0) {
                artworkIndex --;
                for (int i=artworkIndex; i > 0; i--)
                {
                    if (discoveredArtworks[i] != null) break;
                    else artworkIndex --;
                }
            }
            else artworkIndex = listCount;
            SetActiveAllChildren(secondLayout.transform, false);
            discoveredArtworks[artworkIndex].OpenDetailsFromScript();

        }
        else 
        {
            if (pageIndex > 0) pageIndex --;
            else pageIndex = pages.Length - 1;

            UpdatePage();
        }
    }

    void UpdatePage()
    {
        for (int i=0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[pageIndex].SetActive(true);
    }
}
