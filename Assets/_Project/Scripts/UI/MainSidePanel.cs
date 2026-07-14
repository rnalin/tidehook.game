using UnityEngine;
using UnityEngine.UI;

// The tabbed right-side panel (Fish Encyclopedia / Bait Selector / Upgrades).
// Opened by the book button in the HUD; tabs controlled by the spine buttons.
public class MainSidePanel : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;

    [Header("Tab Content")]
    [SerializeField] GameObject fishpediaTab;
    [SerializeField] GameObject baitTab;
    [SerializeField] GameObject upgradesTab;

    [Header("Spine Tab Buttons")]
    [SerializeField] Button fishButton;
    [SerializeField] Button baitButton;
    [SerializeField] Button upgradesButton;

    [Header("Active Tab Indicators")]
    [SerializeField] Image fishTabBg;
    [SerializeField] Image baitTabBg;
    [SerializeField] Image upgradesTabBg;

    public enum Tab { Fish = 0, Bait = 1, Upgrades = 2 }

    static readonly Color TabActive   = new Color(0.85f, 0.65f, 0.35f);
    static readonly Color TabInactive = new Color(0.60f, 0.42f, 0.22f);

    Tab currentTab = Tab.Fish;

    void Start()
    {
        fishButton?.onClick.AddListener(() => SwitchTab(Tab.Fish));
        baitButton?.onClick.AddListener(() => SwitchTab(Tab.Bait));
        upgradesButton?.onClick.AddListener(() => SwitchTab(Tab.Upgrades));
        panelRoot.SetActive(false);
    }

    public void Open(Tab tab = Tab.Fish)
    {
        panelRoot.SetActive(true);
        SwitchTab(tab);
    }

    public void Toggle(Tab tab = Tab.Fish)
    {
        if (panelRoot.activeSelf && currentTab == tab)
            panelRoot.SetActive(false);
        else
            Open(tab);
    }

    void SwitchTab(Tab tab)
    {
        currentTab = tab;
        fishpediaTab?.SetActive(tab == Tab.Fish);
        baitTab?.SetActive(tab == Tab.Bait);
        upgradesTab?.SetActive(tab == Tab.Upgrades);
        SetTabHighlight(fishTabBg,     tab == Tab.Fish);
        SetTabHighlight(baitTabBg,     tab == Tab.Bait);
        SetTabHighlight(upgradesTabBg, tab == Tab.Upgrades);
    }

    static void SetTabHighlight(Image img, bool active)
    {
        if (img != null) img.color = active ? TabActive : TabInactive;
    }
}
