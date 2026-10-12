using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UtilityButton : MonoBehaviour
{
    // ============================================================
    // BUTTONS, PANELS & CLOSE BUTTONS
    // ============================================================

    [Header("===== UTILITY BUTTONS =====")]

    [Tooltip("Buttons ko unke matching panels ke same index par assign karo.")]
    public Button[] utilityButtons;

    [Tooltip("Har button ka matching panel.")]
    public GameObject[] utilityPanels;

    [Tooltip("Har panel ka matching close button.")]
    public Button[] closeButtons;


    // ============================================================
    // INSTAGRAM BUTTON
    // ============================================================

    [Header("===== INSTAGRAM =====")]

    [Tooltip("Instagram button. Isse dabane par profile browser/app me khulegi.")]
    public Button instagramButton;

    [Tooltip("Instagram profile ka poora link. Example: https://www.instagram.com/yourusername/")]
    public string instagramUrl = "https://www.instagram.com/yourusername/";


    // ============================================================
    // EXIT BUTTON & CONFIRMATION PANEL
    // ============================================================

    [Header("===== EXIT CONFIRMATION =====")]

    [Tooltip("Exit button. Isse dabane par confirmation panel button se nikalega.")]
    public Button exitButton;

    [Tooltip("Exit confirmation panel (Are you sure?).")]
    public GameObject exitPanel;

    [Tooltip("Yes button - game band karega.")]
    public Button exitYesButton;

    [Tooltip("No button - panel wapas exit button me chala jayega.")]
    public Button exitNoButton;

    [Tooltip("On rakho to exit panel khulne par Objects To Hide bhi hide honge.")]
    public bool hideObjectsOnExitPanel = false;


    // ============================================================
    // OBJECTS TO HIDE
    // ============================================================

    [Header("===== OBJECTS TO HIDE =====")]

    [Tooltip("Panel open hone par hide hone wale Coins, Gems, UI Buttons etc.")]
    public GameObject[] objectsToHide;


    // ============================================================
    // ANIMATION SETTINGS
    // ============================================================

    [Header("===== ANIMATION SETTINGS =====")]

    [Tooltip("Panel open aur close animation ki duration.")]
    [Min(0.05f)]
    public float animationDuration = 0.3f;

    [Tooltip("Button ki position se panel kitne scale par start hoga.")]
    [Range(0.01f, 1f)]
    public float startScale = 0.1f;


    // ============================================================
    // PRIVATE VARIABLES
    // ============================================================

    // Utility panels + exit panel (last index) ek saath
    private GameObject[] panels;
    private Button[] openButtons;
    private Button[] closeButtonsAll;

    private int exitIndex = -1;

    private RectTransform[] panelRects;
    private RectTransform[] buttonRects;

    private Vector2[] originalPositions;
    private Vector3[] originalScales;
    private Quaternion[] originalRotations;

    private bool[] originalActiveStates;
    private bool objectsHidden = false;

    private int currentPanelIndex = -1;

    private bool isAnimating = false;

    private Coroutine animationCoroutine;


    // ============================================================
    // INITIALIZATION
    // ============================================================

    private void Awake()
    {
        // ----- Utility panels + exit panel ko ek list me jodo -----

        int baseCount = utilityPanels != null ? utilityPanels.Length : 0;

        bool hasExit = exitPanel != null;

        int panelCount = baseCount + (hasExit ? 1 : 0);

        exitIndex = hasExit ? baseCount : -1;

        panels = new GameObject[panelCount];
        openButtons = new Button[panelCount];
        closeButtonsAll = new Button[panelCount];

        for (int i = 0; i < baseCount; i++)
        {
            panels[i] = utilityPanels[i];

            if (utilityButtons != null && i < utilityButtons.Length)
                openButtons[i] = utilityButtons[i];

            if (closeButtons != null && i < closeButtons.Length)
                closeButtonsAll[i] = closeButtons[i];
        }

        if (hasExit)
        {
            panels[exitIndex] = exitPanel;
            openButtons[exitIndex] = exitButton;
            closeButtonsAll[exitIndex] = exitNoButton;

            // Exit panel shuru me band rahe
            exitPanel.SetActive(false);
        }

        // ----- Cache -----

        panelRects = new RectTransform[panelCount];
        originalPositions = new Vector2[panelCount];
        originalScales = new Vector3[panelCount];
        originalRotations = new Quaternion[panelCount];

        buttonRects = new RectTransform[panelCount];

        int hideCount = objectsToHide != null ? objectsToHide.Length : 0;

        originalActiveStates = new bool[hideCount];

        // Cache objects' original active states.
        for (int i = 0; i < hideCount; i++)
        {
            if (objectsToHide[i] != null)
            {
                originalActiveStates[i] =
                    objectsToHide[i].activeSelf;
            }
        }

        // Cache panel positions and scales.
        for (int i = 0; i < panelCount; i++)
        {
            if (panels[i] == null)
                continue;

            panelRects[i] =
                panels[i].GetComponent<RectTransform>();

            if (panelRects[i] != null)
            {
                originalPositions[i] =
                    panelRects[i].anchoredPosition;

                originalScales[i] =
                    panelRects[i].localScale;

                originalRotations[i] =
                    panelRects[i].localRotation;
            }

            if (openButtons[i] != null)
            {
                buttonRects[i] =
                    openButtons[i].GetComponent<RectTransform>();

                int index = i;

                openButtons[i].onClick.AddListener(
                    () => OpenPanel(index)
                );
            }

            if (closeButtonsAll[i] != null)
            {
                int index = i;

                closeButtonsAll[i].onClick.AddListener(
                    () => ClosePanel(index)
                );
            }
        }

        // ----- Exit YES button -----

        if (exitYesButton != null)
        {
            exitYesButton.onClick.AddListener(ConfirmExit);
        }

        // ----- Instagram button -----

        if (instagramButton != null)
        {
            instagramButton.onClick.AddListener(OpenInstagram);
        }
    }

    private void OnDestroy()
    {
        if (instagramButton != null)
        {
            instagramButton.onClick.RemoveListener(OpenInstagram);
        }
    }


    // ============================================================
    // INSTAGRAM
    // ============================================================

    public void OpenInstagram()
    {
        if (string.IsNullOrWhiteSpace(instagramUrl))
        {
            Debug.LogWarning(
                "UtilityButton: Instagram URL khali hai. Inspector me link daalo."
            );
            return;
        }

        Application.OpenURL(instagramUrl.Trim());
    }


    // ============================================================
    // CONFIRM EXIT
    // ============================================================

    public void ConfirmExit()
    {
        // Sirf tab quit ho jab exit panel poora khula ho
        if (exitIndex == -1 ||
            currentPanelIndex != exitIndex ||
            isAnimating)
        {
            return;
        }

        Time.timeScale = 1f;

        Debug.Log("UtilityButton: Exit Game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    // ============================================================
    // OPEN PANEL
    // ============================================================

    public void OpenPanel(int index)
    {
        if (isAnimating)
            return;

        if (index < 0 || index >= panels.Length)
            return;

        if (panels[index] == null ||
            panelRects[index] == null ||
            buttonRects[index] == null)
        {
            Debug.LogWarning(
                "UtilityButton: Button ya Panel assign nahi hai."
            );
            return;
        }

        if (currentPanelIndex == index)
            return;

        // Close any previously open panel.
        if (currentPanelIndex != -1)
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            int previousIndex = currentPanelIndex;

            RestorePanel(previousIndex);
            currentPanelIndex = -1;
        }

        // Hide coins, gems and other assigned objects.
        bool shouldHide =
            index != exitIndex || hideObjectsOnExitPanel;

        if (shouldHide)
        {
            HideObjects();
        }
        else if (objectsHidden)
        {
            ShowObjects();
        }

        currentPanelIndex = index;

        RectTransform panel = panelRects[index];

        // Activate and bring panel to the front.
        panels[index].SetActive(true);
        panel.SetAsLastSibling();

        // Get the button's screen position.
        Vector3 buttonScreenPosition =
            buttonRects[index].position;

        Canvas canvas =
            panel.GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            Canvas rootCanvas = canvas.rootCanvas;

            Camera uiCamera =
                rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : rootCanvas.worldCamera;

            RectTransform parentRect =
                panel.parent as RectTransform;

            if (parentRect != null)
            {
                Vector2 localPoint;

                if (RectTransformUtility
                    .ScreenPointToLocalPointInRectangle(
                        parentRect,
                        RectTransformUtility.WorldToScreenPoint(
                            uiCamera,
                            buttonScreenPosition
                        ),
                        uiCamera,
                        out localPoint))
                {
                    panel.anchoredPosition = localPoint;
                }
            }
        }

        panel.localScale =
            originalScales[index] * startScale;

        panel.localRotation = originalRotations[index];

        animationCoroutine = StartCoroutine(
            AnimatePanel(
                index,
                panel.anchoredPosition,
                originalPositions[index],
                panel.localScale,
                originalScales[index],
                true
            )
        );
    }


    // ============================================================
    // CLOSE PANEL
    // ============================================================

    public void ClosePanel(int index)
    {
        if (isAnimating)
            return;

        if (currentPanelIndex != index)
            return;

        if (index < 0 || index >= panelRects.Length)
            return;

        if (panelRects[index] == null)
            return;

        RectTransform panel = panelRects[index];

        Vector2 buttonPosition = panel.anchoredPosition;

        if (buttonRects[index] != null)
        {
            Canvas canvas =
                panel.GetComponentInParent<Canvas>();

            if (canvas != null)
            {
                Canvas rootCanvas = canvas.rootCanvas;

                Camera uiCamera =
                    rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : rootCanvas.worldCamera;

                RectTransform parentRect =
                    panel.parent as RectTransform;

                if (parentRect != null)
                {
                    Vector2 localPoint;

                    Vector2 screenPoint =
                        RectTransformUtility.WorldToScreenPoint(
                            uiCamera,
                            buttonRects[index].position
                        );

                    if (RectTransformUtility
                        .ScreenPointToLocalPointInRectangle(
                            parentRect,
                            screenPoint,
                            uiCamera,
                            out localPoint))
                    {
                        buttonPosition = localPoint;
                    }
                }
            }
        }

        animationCoroutine = StartCoroutine(
            AnimatePanel(
                index,
                panel.anchoredPosition,
                buttonPosition,
                panel.localScale,
                originalScales[index] * startScale,
                false
            )
        );
    }


    // ============================================================
    // PANEL ANIMATION
    // ============================================================

    private IEnumerator AnimatePanel(
        int index,
        Vector2 fromPosition,
        Vector2 toPosition,
        Vector3 fromScale,
        Vector3 toScale,
        bool opening)
    {
        isAnimating = true;

        RectTransform panel = panelRects[index];

        float elapsed = 0f;
        float duration = Mathf.Max(0.05f, animationDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth ease animation.
            float smoothT = t * t * (3f - 2f * t);

            panel.anchoredPosition = Vector2.Lerp(
                fromPosition,
                toPosition,
                smoothT
            );

            panel.localScale = Vector3.Lerp(
                fromScale,
                toScale,
                smoothT
            );

            yield return null;
        }

        panel.anchoredPosition = toPosition;
        panel.localScale = toScale;

        if (opening)
        {
            panel.localRotation = originalRotations[index];
        }
        else
        {
            RestorePanel(index);

            currentPanelIndex = -1;

            // Restore coins, gems and other UI objects.
            if (objectsHidden)
            {
                ShowObjects();
            }
        }

        isAnimating = false;
        animationCoroutine = null;
    }


    // ============================================================
    // HIDE / SHOW OBJECTS
    // ============================================================

    private void HideObjects()
    {
        if (objectsToHide == null)
            return;

        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
            {
                objectsToHide[i].SetActive(false);
            }
        }

        objectsHidden = true;
    }

    private void ShowObjects()
    {
        if (objectsToHide == null)
            return;

        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
            {
                objectsToHide[i].SetActive(
                    originalActiveStates[i]
                );
            }
        }

        objectsHidden = false;
    }


    // ============================================================
    // RESTORE PANEL
    // ============================================================

    private void RestorePanel(int index)
    {
        if (index < 0 || index >= panelRects.Length)
            return;

        if (panelRects[index] == null)
            return;

        panelRects[index].anchoredPosition =
            originalPositions[index];

        panelRects[index].localScale =
            originalScales[index];

        panelRects[index].localRotation =
            originalRotations[index];

        panels[index].SetActive(false);
    }
}