using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;

    private void Start()
    {
        if (playerStat == null)
        {
            playerStat = FindFirstObjectByType<PlayerStat>();
        }

        if (playerStat == null)
        {
            Debug.LogError("PlayerStatusUI: PlayerStat을 찾지 못했습니다.");
            return;
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = playerStat.GetMaxHp();
            hpSlider.value = playerStat.GetCurrentHp();
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = playerStat.GetMaxStamina();
            staminaSlider.value = playerStat.GetCurrentStamina();
        }
    }

    private void Update()
    {
        if (playerStat == null) return;

        if (hpSlider != null)
        {
            hpSlider.value = playerStat.GetCurrentHp();
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = playerStat.GetCurrentStamina();
        }
    }
}