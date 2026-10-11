
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

    private RectTransform[] panelRects;
    private RectTransform[] buttonRects;

    private Vector2[] originalPositions;
    private Vector3[] originalScales;
    private Quaternion[] originalRotations;

    private bool[] originalActiveStates;

    private int currentPanelIndex = -1;

    private bool isAnimating = false;

    private Coroutine animationCoroutine;


    // ============================================================
    // INITIALIZATION
    // ============================================================

    private void Awake()
    {
        int panelCount = utilityPanels.Length;

        panelRects = new RectTransform[panelCount];
        originalPositions = new Vector2[panelCount];
        originalScales = new Vector3[panelCount];
        originalRotations = new Quaternion[panelCount];

        buttonRects = new RectTransform[panelCount];

        originalActiveStates = new bool[objectsToHide.Length];

        // Cache objects' original active states.
        for (int i = 0; i < objectsToHide.Length; i++)
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
            if (utilityPanels[i] == null)
                continue;

            panelRects[i] =
                utilityPanels[i].GetComponent<RectTransform>();

            if (panelRects[i] != null)
            {
                originalPositions[i] =
                    panelRects[i].anchoredPosition;

                originalScales[i] =
                    panelRects[i].localScale;

                originalRotations[i] =
                    panelRects[i].localRotation;
            }

            if (i < utilityButtons.Length &&
                utilityButtons[i] != null)
            {
                buttonRects[i] =
                    utilityButtons[i].GetComponent<RectTransform>();

                int index = i;

                utilityButtons[i].onClick.AddListener(
                    () => OpenPanel(index)
                );
            }

            if (i < closeButtons.Length &&
                closeButtons[i] != null)
            {
                int index = i;

                closeButtons[i].onClick.AddListener(
                    () => ClosePanel(index)
                );
            }
        }
    }


    // ============================================================
    // OPEN PANEL
    // ============================================================

    public void OpenPanel(int index)
    {
        if (isAnimating)
            return;

        if (index < 0 || index >= utilityPanels.Length)
            return;

        if (utilityPanels[index] == null ||
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
        HideObjects();

        currentPanelIndex = index;

        RectTransform panel = panelRects[index];

        // Activate and bring panel to the front.
        utilityPanels[index].SetActive(true);
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
            ShowObjects();
        }

        isAnimating = false;
        animationCoroutine = null;
    }


    // ============================================================
    // HIDE / SHOW OBJECTS
    // ============================================================

    private void HideObjects()
    {
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
            {
                objectsToHide[i].SetActive(false);
            }
        }
    }

    private void ShowObjects()
    {
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
            {
                objectsToHide[i].SetActive(
                    originalActiveStates[i]
                );
            }
        }
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

        utilityPanels[index].SetActive(false);
    }
}
