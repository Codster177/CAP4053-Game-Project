using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button upgradeOption1;
    [SerializeField] private Button upgradeOption2;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField, Range(1f, 200f)] private float attackBoostPercent = 20f;
    [SerializeField, Range(1f, 200f)] private float speedBoostPercent = 20f;

    private void Start()
    {
        
        upgradeOption1.onClick.AddListener(() => ChooseUpgrade(0));
        upgradeOption2.onClick.AddListener(() => ChooseUpgrade(1));
    }

    public void ShowUpgradeOptions()
    {
        if (upgradePanel == null)
        {
       
            return;
        }

        upgradePanel.SetActive(true);

    }

    private void ChooseUpgrade(int option)
    {
        if (option == 0)
        {
            ApplyUpgrade(PlayerUpgradeType.AttackBoost);
        }
        else
        {
            ApplyUpgrade(PlayerUpgradeType.SpeedBoost);
        }

        upgradePanel.SetActive(false);
        RoomProgressionManager.Instance.ResumeGame();
    }

    private void ApplyUpgrade(PlayerUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case PlayerUpgradeType.AttackBoost:
                playerAttack.IncreaseDamageByPercentage(attackBoostPercent);
                break;

            case PlayerUpgradeType.SpeedBoost:
                playerController.IncreaseMoveSpeedByPercentage(speedBoostPercent);
                break;
        }
    }

}

public enum PlayerUpgradeType { AttackBoost, SpeedBoost }
