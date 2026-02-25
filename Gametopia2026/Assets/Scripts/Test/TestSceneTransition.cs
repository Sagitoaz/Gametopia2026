using UnityEngine;
using CoderGoHappy.Scene;

public class TestSceneTransition : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneController sceneController = FindFirstObjectByType<SceneController>();
        
        if (sceneController != null)
        {
            Debug.Log("[TEST] Transitioning to Level02...");
            sceneController.TransitionToScene("Level02", 0);
        }
        else
        {
            Debug.LogError("[TEST] SceneController not found!");
        }
    }
}