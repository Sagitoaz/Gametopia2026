using UnityEngine;
using DG.Tweening;

public class TestDOTween : MonoBehaviour
{
    void Start()
    {
        Debug.Log("DOTween Test - Pulsing...");
        transform.DOScale(1.5f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}