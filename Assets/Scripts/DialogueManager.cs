using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TabManager tabManager;
    public Image actorImage;
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backgroundBox;
    public AudioSource messageSound;
    public GameObject characterImage;
    public GameObject darkBackground;
    public GameObject popup;
    public GameObject secondPopup;
    public GameObject hideButton;
    public GameObject closeButton;
    public GameObject convoPanel;
    public float popupAppearDelay = 2f;
    public float popupEaseDuration = 0.5f;
    public float characterImageAppearDelay = 1f;

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
        if (hideButton != null)
        {
            hideButton.SetActive(false);
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
        darkBackground.SetActive(true);
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

    void OnClickCloseButton()
    {
        // Hide the first popup
        popup.SetActive(false);

        //Turn off the dark background
        darkBackground.SetActive(false);

        // Show the second popup after the specified delay
        StartCoroutine(ShowSecondPopupWithDelay(popupAppearDelay));

        // Show the tab buttons and content panels after the second popup appears
        StartCoroutine(ShowTabsAfterDelay(popupAppearDelay + popupEaseDuration));
    }

    IEnumerator ShowTabsAfterDelay(float delay)
    {
        // Wait for the specified delay before showing the tabs
        yield return new WaitForSeconds(delay);

        // Call the ShowTab method in the TabManager script to show the tabs
        tabManager.ShowTab(0); // Change the index (0) to the desired default tab index
    }

    IEnumerator ShowHideButtonAfterDelay(float delay)
    {
        // Wait for the specified delay before showing the hideButton
        yield return new WaitForSeconds(delay);

        // Show the hideButton
        hideButton.SetActive(true);
    }

    IEnumerator ShowCharacterImageAfterDelay(float delay)
    {
        // Wait for the specified delay before showing the second image
        yield return new WaitForSeconds(delay);

        // Show the second image with easing effect
        characterImage.SetActive(true);
        characterImage.transform.localScale = Vector3.zero; // Start with zero scale
        characterImage.LeanScale(new Vector3(0.35f, 0.35f, 0.35f), popupEaseDuration).setEase(LeanTweenType.easeInOutExpo);
    }
    IEnumerator ShowSecondPopupWithDelay(float delay)
    {
        //Hide the convo panel
        convoPanel.transform.localScale = Vector3.zero;

        // Wait for the specified delay before showing the second popup
        yield return new WaitForSeconds(delay);

        // Show the second popup with easing effect
        secondPopup.SetActive(true);
        secondPopup.transform.localScale = Vector3.zero; // Start with zero scale
        secondPopup.LeanScale(Vector3.one, popupEaseDuration).setEase(LeanTweenType.easeInOutExpo);
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
        popup.SetActive(false); // Hide the popup at the start
        secondPopup.SetActive(false); // Hide the second popup at the start
        characterImage.SetActive(false); //Hide the image at the start
        hideButton.SetActive(false); // Hide the hideButton at the start
        darkBackground.SetActive(false); // Hide the dark background at the start

        // Add a click event to the button on the first popup
        closeButton.GetComponent<Button>().onClick.AddListener(OnClickCloseButton);

        // Call the method to show the second image after the specified delay
        StartCoroutine(ShowCharacterImageAfterDelay(characterImageAppearDelay));

        // Call the method to show the hideButton after the specified delay
        StartCoroutine(ShowHideButtonAfterDelay(characterImageAppearDelay + popupAppearDelay));

        // Debug log to indicate that TabManager is initialized
        Debug.Log("TabManager is initialized.");

        // Access defaultTab value from the TabManager script
        tabManager.defaultTab = 4; // Replace '0' with the desired default tab index.
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
