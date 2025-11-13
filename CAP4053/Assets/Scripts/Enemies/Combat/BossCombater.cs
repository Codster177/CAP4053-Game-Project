using UnityEngine;

public class BossCombater : ChaserCombater
{
    protected BossController bossController;

    void Awake()
    {
        bossController = chaserController as BossController;
    }
    void Update()
    {
        return;
    }

}
