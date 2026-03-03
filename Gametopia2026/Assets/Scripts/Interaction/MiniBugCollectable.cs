using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using CoderGoHappy.Core;
using CoderGoHappy.Events;
using CoderGoHappy.Level;

namespace CoderGoHappy.Interaction
{
    /// <summary>
    /// Attach to bug GameObjects to make them clickable and collectable.
    ///
    /// Usage:
    ///   1. Add a Collider2D (or Collider for 3D) so Unity can detect mouse/touch input.
    ///   2. Attach this script.
    ///   3. Optionally assign a unique bugID; if left empty it auto-generates one.
    ///
    /// Flow:
    ///   Player clicks bug → miniBugsCollected++ in GameStateData
    ///                     → GameEvents.MiniBugCollected published
    ///                     → LevelManager hears it and fires BugCounterUpdate
    ///                     → BugCounterUI receives BugCounterUpdate and refreshes "X/Y" display
    ///                     → Bug plays collect animation then destroys itself
    /// </summary>
    public class MiniBugCollectable : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Bug Identity")]
        [Tooltip("Unique ID for this bug. Leave empty to auto-generate.")]
        [SerializeField] private string bugID = "";

        [Header("Hover Animation")]
        [Tooltip("Scale multiplier when mouse hovers over bug")]
        [SerializeField] private float hoverScale = 1.25f;

        [Tooltip("Duration of hover scale animation (seconds)")]
        [SerializeField] private float hoverDuration = 0.15f;

        [Header("Collect Animation")]
        [Tooltip("Duration of the full collect animation before the bug is destroyed")]
        [SerializeField] private float collectAnimDuration = 0.5f;

        [Tooltip("Optional particle/VFX prefab spawned at bug position on collect")]
        [SerializeField] private GameObject collectVFXPrefab;

        [Header("Audio")]
        [Tooltip("Sound played when the bug is collected")]
        [SerializeField] private AudioClip collectSound;

        #endregion

        #region Private State

        private bool isCollected = false;
        private Vector3 originalScale;
        private int currentTotal = 10; // fallback when LevelManager absent

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            originalScale = transform.localScale;

            // Auto-generate ID if not set
            if (string.IsNullOrEmpty(bugID))
            {
                bugID = "bug_" + GetInstanceID();
            }
        }

        private void Start()
        {
            // Cache total from LevelManager if available
            if (LevelManager.Instance != null)
            {
                currentTotal = LevelManager.Instance.GetMiniBugProgress().total;
            }

            // If this bug was already collected in a previous visit, destroy it immediately
            if (GameStateData.Instance != null)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                if (GameStateData.Instance.IsBugCollected(sceneName, bugID))
                {
                    Debug.Log($"[MiniBugCollectable] Bug '{bugID}' already collected — removing.");
                    Destroy(gameObject);
                }
            }
        }

        private void OnMouseDown()
        {
            Collect();
        }

        private void OnMouseEnter()
        {
            if (isCollected) return;
            transform.DOKill();
            transform.DOScale(originalScale * hoverScale, hoverDuration).SetEase(Ease.OutBack);
        }

        private void OnMouseExit()
        {
            if (isCollected) return;
            transform.DOKill();
            transform.DOScale(originalScale, hoverDuration).SetEase(Ease.InBack);
        }

        #endregion

        #region Collection Logic

        /// <summary>
        /// Call this to collect the bug programmatically (e.g., from touch input).
        /// Safe to call multiple times — only executes once.
        /// </summary>
        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            Debug.Log($"[MiniBugCollectable] Bug '{bugID}' collected!");

            // 1. Persistent save using scene-qualified ID (prevents respawn, tracks per-scene count)
            string sceneName = SceneManager.GetActiveScene().name;
            if (GameStateData.Instance != null)
            {
                GameStateData.Instance.MarkBugCollected(sceneName, bugID);
            }
            else
            {
                Debug.LogWarning("[MiniBugCollectable] GameStateData.Instance is null — bug not saved!");
            }

            // 2a. Publish MiniBugCollected (LevelManager listens to this)
            EventManager.Instance?.Publish(GameEvents.MiniBugCollected, bugID);

            // 2b. Also publish BugCounterUpdate directly so UI refreshes even without LevelManager in scene
            if (EventManager.Instance != null && GameStateData.Instance != null)
            {
                int totalBugs = LevelManager.Instance != null
                    ? LevelManager.Instance.GetMiniBugProgress().total
                    : currentTotal;
                EventManager.Instance.Publish("BugCounterUpdate", new BugCountData
                {
                    collected = GameStateData.Instance.miniBugsCollected, // global total
                    total = totalBugs
                });
            }

            // 3. Play audio
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // 4. Spawn VFX at collect position
            if (collectVFXPrefab != null)
            {
                Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
            }

            // 5. Animate then destroy
            PlayCollectAnimation();
        }

        /// <summary>
        /// Punch scale up → shrink to zero → destroy GameObject
        /// </summary>
        private void PlayCollectAnimation()
        {
            transform.DOKill();

            // Step 1: punch scale
            transform.DOPunchScale(Vector3.one * 0.5f, collectAnimDuration * 0.5f, 5, 0.5f)
                .OnComplete(() =>
                {
                    // Step 2: shrink to nothing
                    transform.DOScale(Vector3.zero, collectAnimDuration * 0.5f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => Destroy(gameObject));
                });
        }

        #endregion

        #region Public API

        /// <summary>
        /// Get the unique ID of this bug.
        /// </summary>
        public string GetBugID() => bugID;

        /// <summary>
        /// Returns true if this bug has already been collected.
        /// </summary>
        public bool IsCollected() => isCollected;

        #endregion
    }
}
