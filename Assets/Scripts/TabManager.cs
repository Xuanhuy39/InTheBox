using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject[] tabContentPanels;
    public Button[] tabButtons;
    public GameObject selectedTabIndicator;
    public int defaultTab = 0;

    private int activeTabIndex = -1;

    private void Start()
    {
        // Initialize the tabs, set the default tab as active
        ShowTab(defaultTab);

        // Add button click listeners to each tab button
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // Store the index in a local variable to avoid closure issues
            tabButtons[i].onClick.AddListener(() => ShowTab(index));
        }
    }

    private void ShowTab(int index)
    {
        // Deactivate the currently active tab content
        if (activeTabIndex >= 0)
        {
            tabContentPanels[activeTabIndex].SetActive(false);
        }

        // Activate the selected tab content
        tabContentPanels[index].SetActive(true);

        // Move the selected tab indicator to the position of the selected tab button
        Vector2 indicatorPosition = tabButtons[index].transform.position;
        selectedTabIndicator.transform.position = indicatorPosition;

        // Update the activeTabIndex to the new index
        activeTabIndex = index;
    }
}
