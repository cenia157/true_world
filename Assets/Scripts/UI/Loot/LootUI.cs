using UnityEngine;
using TMPro;

public class LootUI : MonoBehaviour
{
    [SerializeField] private GameObject lootPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private LootSlotUI[] slots;

    [Header("Auto Close")]
    [SerializeField] private Transform player;
    [SerializeField] private float closeDistance = 3.5f;

    private CorpseLoot currentCorpse;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        Close();
    }

    private void Update()
    {
        if (currentCorpse == null) return;
        if (player == null) return;

        float distance = Vector3.Distance(player.position, currentCorpse.transform.position);

        if (distance > closeDistance)
        {
            Close();
        }
    }

    public void Open(CorpseLoot corpseLoot)
    {
        if (corpseLoot == null) return;

        currentCorpse = corpseLoot;

        if (lootPanel != null)
        {
            lootPanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = $"{corpseLoot.gameObject.name} Loot";
        }

        Refresh();
    }

    public void Close()
    {
        currentCorpse = null;

        if (lootPanel != null)
        {
            lootPanel.SetActive(false);
        }

        ClearSlots();
    }

    public void Refresh()
    {
        ClearSlots();

        if (currentCorpse == null) return;

        int index = 0;

        foreach (LootResult loot in currentCorpse.CurrentLoot)
        {
            if (index >= slots.Length)
                break;

            if (loot == null || loot.itemData == null)
                continue;

            slots[index].SetData(loot);
            index++;
        }
    }

    private void ClearSlots()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].Clear();
            }
        }
    }
}