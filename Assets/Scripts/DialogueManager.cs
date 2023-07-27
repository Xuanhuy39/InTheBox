using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Image actorImage;
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backgroundBox;
    public AudioSource messageSound;
    public GameObject popup;
    public GameObject closeButton; // Reference to the button to be closed
    public float popupAppearDelay = 2f;
    public float popupEaseDuration = 0.5f;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        Debug.Log("Started convo! Loaded messages:" + messages.Length);
        DisplayMessage();

        // Hide the closeButton if it exists
        if (closeButton != null)
        {
            closeButton.SetActive(false);
        }

        backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();
    }

    void DisplayMessage()
    {
        if (activeMessage < currentMessages.Length)
        {
            Message messageToDisplay = currentMessages[activeMessage];
            messageText.text = messageToDisplay.message;

            Actor actorToDisplay = currentActors[messageToDisplay.actorId];
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;

            // Play the sound for the current actor
            if (actorToDisplay.sound != null)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.PlayOneShot(actorToDisplay.sound);
                Debug.Log("Playing sound for actor: " + actorToDisplay.name);
            }
            else
            {
                Debug.Log("No sound assigned for actor: " + actorToDisplay.name);
            }

            AnimateTextColor();

            // Play the message sound effect
            if (messageSound != null && messageSound.clip != null)
            {
                messageSound.Play();
            }
        }
        else
        {
            Debug.Log("Conversation ended!");
            //backgroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
            isActive = false;

            // Delay the popup appearance and add easing effect
            StartCoroutine(ShowPopupWithDelay());
        }
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            // End the dialogue
            EndDialogue();
        }
    }

    void AnimateTextColor()
    {
        Color originalColor = messageText.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        messageText.color = transparentColor;
        LeanTween.value(gameObject, UpdateTextColor, transparentColor, originalColor, 0.5f)
            .setOnUpdate((Color color) => messageText.color = color)
            .setEase(LeanTweenType.easeInOutExpo);
    }

    void UpdateTextColor(Color color)
    {
        messageText.color = color;
    }

    void ShowPopup()
    {
        // Show the popup
        popup.SetActive(true);
    }

    IEnumerator ShowPopupWithDelay()
    {
        // Wait for the specified delay before showing the popup
        yield return new WaitForSeconds(popupAppearDelay);

        // Show the popup with easing effect
        ShowPopup();
        popup.transform.localScale = Vector3.zero; // Start with zero scale
        popup.LeanScale(Vector3.one, popupEaseDuration).setEase(LeanTweenType.easeInOutExpo);
    }

    void EndDialogue()
    {
        // Hide dialogue elements
        actorImage.gameObject.SetActive(false);
        actorName.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        backgroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();

        // Show the popup
        StartCoroutine(ShowPopupWithDelay());
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        popup.SetActive(false); // Hide the popup at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive == true)
        {
            NextMessage();
        }
    }
}
