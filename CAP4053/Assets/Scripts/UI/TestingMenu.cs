using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

public class TestingMenu : MonoBehaviour
{
    [SerializeField] private Button testOp1, testOp2, testOp3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Time.timeScale = 0f;
        testOp1.onClick.AddListener(() => ChooseTestMode(1));
        testOp2.onClick.AddListener(() => ChooseTestMode(2));
        testOp3.onClick.AddListener(() => ChooseTestMode(3));
    }
    private void ChooseTestMode(int testMode)
    {
        GameManager.publicGameManager.ChooseTestMode(testMode);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
