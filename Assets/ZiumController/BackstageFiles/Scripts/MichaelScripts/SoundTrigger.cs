using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SoundTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip soundToPlay;
    [SerializeField]
    private AudioSource audioSource;

    [Header("Control Settings")]
    public KeyCode triggerKey = KeyCode.N; // Default key, changeable in the editor
    public bool autoPlayAudio = false; // Control for auto-play functionality
    public bool stopAudioOnExit = true; // Added: Control for stopping audio on exit

    [Header("UI Settings")]
    public bool bypassUIPopup = false; // Control for UI popup
    [SerializeField]
    private TextMeshProUGUI uiText;

    [Header("Game Object Requirements")]
    public GameObject requiredGameObject; // The specific GameObject to check for
    public bool isCarryingRequired; // Check this if carrying the object is required

    [Header("Object Interaction")]
    public GameObject objectToDisappear; // The object that will disappear after audio finishes

    private bool isPlayerInTrigger = false;
    private bool hasAudioPlayed = false; // Flag to track if audio has been played

    void Start()
    {
        audioSource = SoundManager.Instance._musicSource;
        uiText = UserInterface.Instance.soundTriggerText.GetComponent<TextMeshProUGUI>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("SoundTrigger Error: AudioSource component not found on the GameObject.");
            }
        }

        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("SoundTrigger Error: TextMeshPro UI Text component not assigned.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Key for stopping sound
        {
            StopSound();
        }

        if (isPlayerInTrigger && Input.GetKeyDown(triggerKey) && (!isCarryingRequired || (isCarryingRequired && requiredGameObject != null)))
        {
            if (audioSource.isPlaying)
            {
                StopSound();
            }
            else
            {
                PlaySound();
            }
        }

        if (hasAudioPlayed && audioSource != null && !audioSource.isPlaying && objectToDisappear != null)
        {
            objectToDisappear.SetActive(false);
            hasAudioPlayed = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            if (autoPlayAudio && (!isCarryingRequired || (isCarryingRequired && requiredGameObject != null)))
            {
                PlaySound();
            }
            
            if (!bypassUIPopup)
            {
                UpdateUI();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            if (stopAudioOnExit)
            {
                StopSound();
            }

            if (!bypassUIPopup)
            {
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if (uiText != null)
        {
            bool shouldShowUI = isPlayerInTrigger && (!isCarryingRequired || (isCarryingRequired && requiredGameObject != null));
            uiText.gameObject.SetActive(shouldShowUI);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && soundToPlay != null && !audioSource.isPlaying)
        {
            audioSource.clip = soundToPlay;
            audioSource.Play();
            hasAudioPlayed = true;
        }
    }

    private void StopSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
