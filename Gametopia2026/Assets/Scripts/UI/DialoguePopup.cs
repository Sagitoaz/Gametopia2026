using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CoderGoHappy.Events;
using CoderGoHappy.Core;

namespace CoderGoHappy.UI
{
    /// <summary>
    /// DialoguePopup — shows examine/clue/hint text as a popup panel.
    /// Listens to "ShowDialogue" event (data = string message).
    /// Place ONE instance in each scene's Canvas.
    /// 
    /// Setup:
    ///   Canvas
    ///   └── DialoguePopup (this script)
    ///       ├── Background (Image, semi-transparent dark)
    ///       ├── Panel (Image, content box)
    ///       │   ├── MessageText (TextMeshProUGUI)
    ///       │   └── CloseButton (Button)
    ///       └── (optional) IconImage
    /// </summary>
    public class DialoguePopup : MonoBehaviour
    {
        #region Inspector Fields

        [Header("UI References")]
        [Tooltip("The root panel to show/hide (has CanvasGroup)")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Tooltip("TextMeshProUGUI to display the message")]
        [SerializeField] private TextMeshProUGUI messageText;

        [Tooltip("Close/OK button")]
        [SerializeField] private Button closeButton;

        [Tooltip("Optional icon image (hidden when no icon)")]
        [SerializeField] private Image iconImage;

        [Header("Animation")]
        [Tooltip("Duration of fade-in animation")]
        [SerializeField] private float fadeInDuration = 0.25f;

        [Tooltip("Duration of fade-out animation")]
        [SerializeField] private float fadeOutDuration = 0.2f;

        [Tooltip("Scale-up punch when showing")]
        [SerializeField] private Vector3 showScale = Vector3.one;

        [Tooltip("Auto-close after this many seconds (0 = manual only)")]
        [SerializeField] private float autoCloseDelay = 0f;

        [Header("Behaviour")]
        [Tooltip("Block hotspot clicks while popup is open")]
        [SerializeField] private bool blockInteractionWhileOpen = true;

        #endregion

        #region State

        private bool isVisible = false;
        private Tween autoCloseTween;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Validate references
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogError("[DialoguePopup] CanvasGroup not found. Add it to the root GameObject.", this);
                return;
            }

            // Start hidden
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Subscribe to ShowDialogue event
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe("ShowDialogue", OnShowDialogue);
                EventManager.Instance.Subscribe("HideDialogue", OnHideDialogue);
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe("ShowDialogue", OnShowDialogue);
                EventManager.Instance.Unsubscribe("HideDialogue", OnHideDialogue);
            }

            // Clean up any auto-close tween
            autoCloseTween?.Kill();
        }

        private void Start()
        {
            // Wire close button
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
        }

        #endregion

        #region Show / Hide

        /// <summary>
        /// Show popup with the given message text.
        /// </summary>
        public void Show(string message)
        {
            if (messageText == null) return;

            messageText.text = message;

            // Activate and animate in
            gameObject.SetActive(true);
            isVisible = true;

            // Kill previous tweens
            canvasGroup.DOKill();
            transform.DOKill();

            // Fade in + scale punch
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * 0.85f;

            canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
            transform.DOScale(showScale, fadeInDuration).SetEase(Ease.OutBack);

            // Enable interaction
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = blockInteractionWhileOpen;

            // Auto-close timer
            autoCloseTween?.Kill();
            if (autoCloseDelay > 0f)
            {
                autoCloseTween = DOVirtual.DelayedCall(autoCloseDelay, Hide);
            }

            Debug.Log($"[DialoguePopup] Showing: {message}");
        }

        /// <summary>
        /// Hide the popup with fade-out animation.
        /// </summary>
        public void Hide()
        {
            if (!isVisible) return;

            isVisible = false;
            autoCloseTween?.Kill();

            // Disable interaction immediately
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Fade out
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));
        }

        #endregion

        #region Optional: Show with Icon

        /// <summary>
        /// Show popup with message + icon sprite.
        /// </summary>
        public void Show(string message, Sprite icon)
        {
            if (iconImage != null)
            {
                if (icon != null)
                {
                    iconImage.sprite = icon;
                    iconImage.gameObject.SetActive(true);
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }

            Show(message);
        }

        #endregion

        #region Event Handlers

        private void OnShowDialogue(object data)
        {
            if (data is string message)
            {
                Show(message);
            }
        }

        private void OnHideDialogue(object data)
        {
            Hide();
        }

        #endregion

        #region Public Accessors

        public bool IsVisible => isVisible;

        #endregion
    }
}
