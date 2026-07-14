#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Menu: Tidehook > Build HUD
// Cria toda a hierarquia de UI na cena ativa com base nas references.
// Rode uma vez; depois ajuste sprites e referências no Inspector.
public static class UISceneBuilder
{
    // ── Palette ──────────────────────────────────────────────────────────────
    static readonly Color PanelBg      = Hex("D4A96A");
    static readonly Color PanelBorder  = Hex("7A4A2A");
    static readonly Color PanelInner   = Hex("E8C99A");
    static readonly Color TextDark     = Hex("3A2010");
    static readonly Color XpBarFill    = Hex("4CA3E0");
    static readonly Color XpBarBg      = Hex("8B6040");
    static readonly Color CoinGold     = Hex("F0C040");
    static readonly Color SpineBg      = Hex("5C3820");
    static readonly Color BookLeft     = Hex("D8B87A");
    static readonly Color BookRight    = Hex("E4CDA0");
    static readonly Color RarityBadge  = Hex("60A050");
    static readonly Color ButtonNormal = Hex("C8904A");
    static readonly Color White        = Color.white;
    static readonly Color Clear        = new Color(0, 0, 0, 0);

    // ── Entry point ───────────────────────────────────────────────────────────
    [MenuItem("Tidehook/Build HUD")]
    static void Build()
    {
        if (!EditorUtility.DisplayDialog("Build HUD",
            "Isso criará um novo Canvas na cena ativa.\nContinuar?", "Sim", "Cancelar"))
            return;

        var canvas = CreateCanvas();
        var root   = canvas.transform;

        // ── Left column ──────────────────────────────────────────────────────
        BuildPlayerCard(root);
        BuildBagButton(root);
        BuildBaitSlot(root);

        // ── Right overlay panels ──────────────────────────────────────────────
        BuildInventoryPanel(root);
        BuildMainSidePanel(root);

        // ── Bottom HUD ───────────────────────────────────────────────────────
        BuildBookButton(root);
        BuildFishingPopup(root);
        BuildCatchNotification(root);

        Selection.activeGameObject = canvas;
        EditorUtility.SetDirty(canvas);
        Debug.Log("[UISceneBuilder] HUD criado. Arraste os ScriptableObjects e referências no Inspector.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CANVAS
    // ─────────────────────────────────────────────────────────────────────────
    static GameObject CreateCanvas()
    {
        var go = new GameObject("Canvas_HUD");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        go.AddComponent<GraphicRaycaster>();
        EnsureEventSystem();
        return go;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PLAYER CARD  (top-left)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildPlayerCard(Transform root)
    {
        // Outer border
        var card = Panel("PlayerCard", root, new Vector2(300, 150),
            pivot: new Vector2(0, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pos: new Vector2(10, -10), color: PanelBorder);

        // Inner background
        var inner = Panel("BG", card.transform, new Vector2(292, 142),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: PanelBg);

        // Avatar
        var avatarBorder = Panel("AvatarBorder", inner.transform, new Vector2(90, 90),
            pivot: new Vector2(0, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pos: new Vector2(8, -8), color: PanelBorder);
        var avatar = Panel("AvatarImage", avatarBorder.transform, new Vector2(82, 82),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: new Color(0.5f, 0.7f, 0.9f)); // placeholder sky-blue
        avatar.AddComponent<Image>(); // second image so HUDController can reference it

        // Name text
        var nameGo = new GameObject("NameText");
        nameGo.transform.SetParent(inner.transform, false);
        var nameText = nameGo.AddComponent<TextMeshProUGUI>();
        nameText.text = "FISHERMAN";
        nameText.fontSize = 18;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = TextDark;
        nameText.alignment = TextAlignmentOptions.Left;
        var nameRt = nameGo.GetComponent<RectTransform>();
        nameRt.pivot = new Vector2(0, 1);
        nameRt.anchorMin = new Vector2(0, 1);
        nameRt.anchorMax = new Vector2(1, 1);
        nameRt.offsetMin = new Vector2(106, -18);
        nameRt.offsetMax = new Vector2(-6, -4);
        nameRt.sizeDelta = new Vector2(0, 22);

        // Level text
        var levelGo = Label("LevelText", inner.transform, "Lv. 1", 14, TextDark);
        var levelRt = levelGo.GetComponent<RectTransform>();
        levelRt.pivot = new Vector2(0, 1);
        levelRt.anchorMin = new Vector2(0, 1);
        levelRt.anchorMax = new Vector2(1, 1);
        levelRt.offsetMin = new Vector2(106, -38);
        levelRt.offsetMax = new Vector2(-6, -22);
        levelRt.sizeDelta = new Vector2(0, 16);

        // XP Bar
        var xpBarGo = BuildSlider("XPBar", inner.transform,
            new Vector2(180, 14), new Vector2(106, -58), XpBarBg, XpBarFill);

        // XP text
        var xpTextGo = Label("XPText", inner.transform, "0 / 100", 11, TextDark);
        var xpTextRt = xpTextGo.GetComponent<RectTransform>();
        xpTextRt.pivot = new Vector2(0, 1);
        xpTextRt.anchorMin = new Vector2(0, 1);
        xpTextRt.anchorMax = new Vector2(1, 1);
        xpTextRt.offsetMin = new Vector2(106, -76);
        xpTextRt.offsetMax = new Vector2(-6, -60);
        xpTextRt.sizeDelta = new Vector2(0, 14);

        // Coin row
        var coinRow = Panel("CoinRow", inner.transform, new Vector2(120, 22),
            pivot: new Vector2(0, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pos: new Vector2(8, -100), color: Clear);
        var coinIcon = Panel("CoinIcon", coinRow.transform, new Vector2(20, 20),
            pivot: new Vector2(0, 0.5f), anchorMin: new Vector2(0, 0.5f), anchorMax: new Vector2(0, 0.5f),
            pos: Vector2.zero, color: CoinGold);
        var coinTextGo = Label("CoinText", coinRow.transform, "0", 14, TextDark);
        var coinRt = coinTextGo.GetComponent<RectTransform>();
        coinRt.pivot = new Vector2(0, 0.5f);
        coinRt.anchorMin = new Vector2(0, 0.5f);
        coinRt.anchorMax = new Vector2(1, 0.5f);
        coinRt.offsetMin = new Vector2(26, -11);
        coinRt.offsetMax = new Vector2(0, 11);
        coinRt.sizeDelta = new Vector2(-26, 22);

        // Attach HUDController script
        var hud = card.AddComponent<HUDController>();
        Debug.Log("[UISceneBuilder] PlayerCard criado. Arraste as referências no HUDController.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BAG BUTTON  (left side, below player card)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildBagButton(Transform root)
    {
        var btn = Panel("BagButton", root, new Vector2(110, 110),
            pivot: new Vector2(0, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pos: new Vector2(10, -170), color: PanelBorder);

        var inner = Panel("BG", btn.transform, new Vector2(102, 102),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: PanelBg);

        var iconGo = Panel("BagIcon", inner.transform, new Vector2(64, 64),
            pivot: Vector2.one * 0.5f, anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f),
            pos: new Vector2(0, 6), color: new Color(0.6f, 0.4f, 0.2f));

        var lbl = Label("Label", inner.transform, "BAG", 11, TextDark);
        var lblRt = lbl.GetComponent<RectTransform>();
        lblRt.pivot = new Vector2(0.5f, 0);
        lblRt.anchorMin = new Vector2(0, 0);
        lblRt.anchorMax = new Vector2(1, 0);
        lblRt.offsetMin = new Vector2(0, 4);
        lblRt.offsetMax = new Vector2(0, 16);

        var button = btn.AddComponent<Button>();
        button.targetGraphic = btn.GetComponent<Image>();
        // Wire up in Inspector: onClick -> HUDController.OpenInventory
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BAIT SLOT  (left side, below bag button)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildBaitSlot(Transform root)
    {
        var btn = Panel("BaitSlotButton", root, new Vector2(110, 110),
            pivot: new Vector2(0, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pos: new Vector2(10, -290), color: PanelBorder);

        var inner = Panel("BG", btn.transform, new Vector2(102, 102),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: PanelBg);

        var iconGo = Panel("BaitIcon", inner.transform, new Vector2(60, 60),
            pivot: Vector2.one * 0.5f, anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f),
            pos: new Vector2(0, 6), color: new Color(0.7f, 0.5f, 0.3f));
        iconGo.AddComponent<Image>(); // HUDController.activeBaitIcon

        var countGo = Label("BaitCount", inner.transform, "0", 13, TextDark);
        countGo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        var countRt = countGo.GetComponent<RectTransform>();
        countRt.pivot = new Vector2(0.5f, 0);
        countRt.anchorMin = new Vector2(0, 0);
        countRt.anchorMax = new Vector2(1, 0);
        countRt.offsetMin = new Vector2(0, 4);
        countRt.offsetMax = new Vector2(0, 18);

        var lbl = Label("Label", inner.transform, "BAIT", 11, TextDark);
        var lblRt = lbl.GetComponent<RectTransform>();
        lblRt.pivot = new Vector2(0.5f, 1);
        lblRt.anchorMin = new Vector2(0, 1);
        lblRt.anchorMax = new Vector2(1, 1);
        lblRt.offsetMin = new Vector2(0, -16);
        lblRt.offsetMax = new Vector2(0, -4);

        var button = btn.AddComponent<Button>();
        button.targetGraphic = btn.GetComponent<Image>();
        // Wire up in Inspector: onClick -> HUDController.ToggleBaitPanel
    }

    // ─────────────────────────────────────────────────────────────────────────
    // INVENTORY PANEL  (top-right)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildInventoryPanel(Transform root)
    {
        var panel = Panel("InventoryPanel", root, new Vector2(460, 340),
            pivot: new Vector2(1, 1), anchorMin: new Vector2(1, 1), anchorMax: new Vector2(1, 1),
            pos: new Vector2(-10, -10), color: PanelBorder);

        var inner = Panel("BG", panel.transform, new Vector2(452, 332),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: PanelBg);

        // Title bar
        var titleBar = Panel("TitleBar", inner.transform, new Vector2(452, 34),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            pos: Vector2.zero, color: PanelBorder);

        var iconGo = Panel("TitleIcon", titleBar.transform, new Vector2(24, 24),
            pivot: new Vector2(0, 0.5f), anchorMin: new Vector2(0, 0.5f), anchorMax: new Vector2(0, 0.5f),
            pos: new Vector2(6, 0), color: CoinGold);

        var titleText = Label("TitleText", titleBar.transform, "INVENTORY", 16, White);
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        var tRt = titleText.GetComponent<RectTransform>();
        tRt.pivot = new Vector2(0, 0.5f);
        tRt.anchorMin = new Vector2(0, 0);
        tRt.anchorMax = new Vector2(1, 1);
        tRt.offsetMin = new Vector2(38, 0);
        tRt.offsetMax = new Vector2(-40, 0);

        var closeBtn = BuildCloseButton("CloseBtn", titleBar.transform, new Vector2(-4, 0));

        // Grid (ScrollView)
        var scrollGo = BuildScrollGrid("ItemGrid", inner.transform,
            new Vector2(440, 288), new Vector2(0, -34),
            cellSize: new Vector2(72, 72), spacing: new Vector2(4, 4), columns: 6);

        var inventoryComp = panel.AddComponent<InventoryPanel>();
        // Assign panelRoot=panel, gridParent=scrollGo content, closeButton=closeBtn in Inspector
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MAIN SIDE PANEL  (center-right: Fish / Bait / Upgrades tabs)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildMainSidePanel(Transform root)
    {
        var panel = Panel("MainSidePanel", root, new Vector2(700, 470),
            pivot: new Vector2(1, 0.5f), anchorMin: new Vector2(1, 0.5f), anchorMax: new Vector2(1, 0.5f),
            pos: new Vector2(-10, 0), color: Clear);

        // ── Spine (tab buttons on the left) ──────────────────────────────────
        var spine = Panel("Spine", panel.transform, new Vector2(60, 190),
            pivot: new Vector2(1, 0.5f), anchorMin: new Vector2(0, 0.5f), anchorMax: new Vector2(0, 0.5f),
            pos: new Vector2(0, 0), color: SpineBg);

        GameObject fishTabBtn   = BuildSpineButton("FishTabBtn",   spine.transform, new Vector2(0, 70),  "F");
        GameObject baitTabBtn   = BuildSpineButton("BaitTabBtn",   spine.transform, new Vector2(0, 0),   "B");
        GameObject upgradeTabBtn= BuildSpineButton("UpgradeTabBtn",spine.transform, new Vector2(0, -70), "U");

        // ── Book panel ───────────────────────────────────────────────────────
        var bookPanel = Panel("BookPanel", panel.transform, new Vector2(636, 470),
            pivot: new Vector2(0, 0.5f), anchorMin: new Vector2(0, 0.5f), anchorMax: new Vector2(0, 0.5f),
            pos: new Vector2(64, 0), color: PanelBorder);

        var bookInner = Panel("BookBG", bookPanel.transform, new Vector2(628, 462),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: BookLeft);

        // ── Tab: Fishpedia ────────────────────────────────────────────────────
        var fishpediaTab = BuildFishpediaTab(bookInner.transform);

        // ── Tab: Bait Selector ────────────────────────────────────────────────
        var baitTab = BuildBaitSelectorTab(bookInner.transform);
        baitTab.SetActive(false);

        // ── Tab: Upgrades ─────────────────────────────────────────────────────
        var upgradesTab = BuildUpgradesTab(bookInner.transform);
        upgradesTab.SetActive(false);

        // Attach MainSidePanel script
        var comp = panel.AddComponent<MainSidePanel>();
        panel.SetActive(false);
        // Assign all references in Inspector
    }

    // ─── Fishpedia Tab ────────────────────────────────────────────────────────
    static GameObject BuildFishpediaTab(Transform parent)
    {
        var tab = Panel("FishpediaTab", parent, Vector2.zero,
            pivot: Vector2.zero, anchorMin: Vector2.zero, anchorMax: Vector2.one,
            pos: Vector2.zero, color: Clear);
        SetStretch(tab.GetComponent<RectTransform>(), 0, 0, 0, 0);

        // Title
        var title = Label("Title", tab.transform, "FISH ENCYCLOPEDIA", 18, TextDark);
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        SetTopBar(title.GetComponent<RectTransform>(), 36, 8);

        // Left page  – fish grid
        var leftPage = Panel("LeftPage", tab.transform, Vector2.zero,
            pivot: Vector2.zero, anchorMin: new Vector2(0, 0), anchorMax: new Vector2(0.5f, 1),
            pos: Vector2.zero, color: BookLeft);
        SetStretch(leftPage.GetComponent<RectTransform>(), 8, 44, 4, 40);

        var gridScroll = BuildScrollGrid("FishGrid", leftPage.transform,
            Vector2.zero, Vector2.zero,
            cellSize: new Vector2(80, 80), spacing: new Vector2(6, 6), columns: 3);
        SetStretch(gridScroll.GetComponent<RectTransform>(), 4, 4, 4, 4);

        // Pagination row
        var pagRow = Panel("PaginationRow", leftPage.transform, new Vector2(0, 36),
            pivot: new Vector2(0.5f, 0), anchorMin: new Vector2(0, 0), anchorMax: new Vector2(1, 0),
            pos: Vector2.zero, color: Clear);
        SetStretch(pagRow.GetComponent<RectTransform>(), 4, -36, 4, 0);
        pagRow.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 36);
        pagRow.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
        pagRow.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        pagRow.GetComponent<RectTransform>().offsetMin = new Vector2(4, 4);
        pagRow.GetComponent<RectTransform>().offsetMax = new Vector2(-4, 40);

        var prevBtn = BuildArrowButton("PrevBtn", pagRow.transform, "◀", new Vector2(0.5f, 0.5f), new Vector2(-80, 0));
        var pageText = Label("PageText", pagRow.transform, "1/1", 14, TextDark);
        pageText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        var pageTRt = pageText.GetComponent<RectTransform>();
        pageTRt.anchorMin = new Vector2(0.5f, 0);
        pageTRt.anchorMax = new Vector2(0.5f, 1);
        pageTRt.sizeDelta = new Vector2(60, 0);
        pageTRt.anchoredPosition = Vector2.zero;
        var nextBtn = BuildArrowButton("NextBtn", pagRow.transform, "▶", new Vector2(0.5f, 0.5f), new Vector2(80, 0));

        // Right page – fish detail
        var rightPage = Panel("RightPage", tab.transform, Vector2.zero,
            pivot: Vector2.zero, anchorMin: new Vector2(0.5f, 0), anchorMax: Vector2.one,
            pos: Vector2.zero, color: BookRight);
        SetStretch(rightPage.GetComponent<RectTransform>(), 4, 44, 8, 8);

        BuildFishDetailPage(rightPage.transform);

        // Attach FishpediaScreen
        var fs = tab.AddComponent<FishpediaScreen>();
        // Assign references in Inspector

        return tab;
    }

    static void BuildFishDetailPage(Transform parent)
    {
        // Fish name
        var nameGo = Label("FishName", parent, "BLUEGILL", 20, TextDark);
        nameGo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        nameGo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        var nameRt = nameGo.GetComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0, 1);
        nameRt.anchorMax = new Vector2(1, 1);
        nameRt.pivot = new Vector2(0.5f, 1);
        nameRt.offsetMin = new Vector2(8, -40);
        nameRt.offsetMax = new Vector2(-8, -8);

        // Fish image
        var fishImg = Panel("FishImage", parent, new Vector2(110, 110),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1),
            pos: new Vector2(0, -50), color: Clear);
        fishImg.AddComponent<Image>();

        // Rarity badge
        var badge = Panel("RarityBadge", parent, new Vector2(100, 26),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1),
            pos: new Vector2(0, -168), color: RarityBadge);
        var rarityText = Label("RarityText", badge.transform, "COMMON", 13, White);
        rarityText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        rarityText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        SetStretch(rarityText.GetComponent<RectTransform>(), 0, 0, 0, 0);

        // Description
        var descGo = Label("Description", parent, "A small and hardy fish\nfound in lakes and rivers.", 13, TextDark);
        descGo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        var descRt = descGo.GetComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0, 1);
        descRt.anchorMax = new Vector2(1, 1);
        descRt.pivot = new Vector2(0.5f, 1);
        descRt.offsetMin = new Vector2(8, -260);
        descRt.offsetMax = new Vector2(-8, -200);

        // Found At label + image
        var foundRow = Panel("FoundAtRow", parent, new Vector2(0, 80),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            pos: Vector2.zero, color: Clear);
        foundRow.GetComponent<RectTransform>().offsetMin = new Vector2(8, -360);
        foundRow.GetComponent<RectTransform>().offsetMax = new Vector2(-8, -268);

        var foundLbl = Label("FoundAtLabel", foundRow.transform, "FOUND AT:", 12, TextDark);
        foundLbl.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        var flRt = foundLbl.GetComponent<RectTransform>();
        flRt.anchorMin = new Vector2(0, 1); flRt.anchorMax = new Vector2(1, 1);
        flRt.pivot = new Vector2(0.5f, 1);
        flRt.offsetMin = new Vector2(0, -18); flRt.offsetMax = new Vector2(0, 0);

        var locImg = Panel("LocationImage", foundRow.transform, new Vector2(0, 56),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            pos: Vector2.zero, color: new Color(0.3f, 0.6f, 0.4f));
        locImg.GetComponent<RectTransform>().offsetMin = new Vector2(0, -74);
        locImg.GetComponent<RectTransform>().offsetMax = new Vector2(0, -18);
        locImg.AddComponent<Image>();
    }

    // ─── Bait Selector Tab ────────────────────────────────────────────────────
    static GameObject BuildBaitSelectorTab(Transform parent)
    {
        var tab = Panel("BaitSelectorTab", parent, Vector2.zero,
            pivot: Vector2.zero, anchorMin: Vector2.zero, anchorMax: Vector2.one,
            pos: Vector2.zero, color: Clear);
        SetStretch(tab.GetComponent<RectTransform>(), 0, 0, 0, 0);

        var title = Label("Title", tab.transform, "FISHING BAIT OPTIONS", 18, TextDark);
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        SetTopBar(title.GetComponent<RectTransform>(), 36, 8);

        // Horizontal bait row
        var row = Panel("BaitRow", tab.transform, Vector2.zero,
            pivot: Vector2.zero, anchorMin: new Vector2(0, 0.5f), anchorMax: new Vector2(1, 0.5f),
            pos: Vector2.zero, color: Clear);
        row.GetComponent<RectTransform>().offsetMin = new Vector2(12, -60);
        row.GetComponent<RectTransform>().offsetMax = new Vector2(-12, 60);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        // placeholder bait slots (5)
        for (int i = 0; i < 5; i++)
            BuildBaitOptionSlot($"BaitSlot_{i}", row.transform);

        // BaitScreen component
        tab.AddComponent<BaitScreen>();
        // Assign baits array, listParent=row, etc. in Inspector
        return tab;
    }

    static void BuildBaitOptionSlot(string name, Transform parent)
    {
        var slot = Panel(name, parent, new Vector2(96, 96), Vector2.one * 0.5f,
            Vector2.zero, Vector2.zero, Vector2.zero, PanelBorder);
        var inner = Panel("BG", slot.transform, new Vector2(88, 88),
            Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f,
            Vector2.zero, PanelInner);
        var icon = Panel("Icon", inner.transform, new Vector2(60, 60),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(0, -6), new Color(0.7f, 0.5f, 0.3f));
        var cnt = Label("Count", inner.transform, "0", 13, TextDark);
        cnt.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        cnt.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        var cRt = cnt.GetComponent<RectTransform>();
        cRt.pivot = new Vector2(0.5f, 0);
        cRt.anchorMin = new Vector2(0, 0); cRt.anchorMax = new Vector2(1, 0);
        cRt.offsetMin = new Vector2(0, 4); cRt.offsetMax = new Vector2(0, 18);
        slot.AddComponent<Button>().targetGraphic = slot.GetComponent<Image>();
    }

    // ─── Upgrades Tab ─────────────────────────────────────────────────────────
    static GameObject BuildUpgradesTab(Transform parent)
    {
        var tab = Panel("UpgradesTab", parent, Vector2.zero,
            pivot: Vector2.zero, anchorMin: Vector2.zero, anchorMax: Vector2.one,
            pos: Vector2.zero, color: Clear);
        SetStretch(tab.GetComponent<RectTransform>(), 0, 0, 0, 0);

        var title = Label("Title", tab.transform, "UPGRADES", 18, TextDark);
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        SetTopBar(title.GetComponent<RectTransform>(), 36, 8);

        var scrollGo = BuildScrollGrid("UpgradeList", tab.transform,
            Vector2.zero, Vector2.zero,
            cellSize: new Vector2(0, 80), spacing: new Vector2(0, 4), columns: 1);
        SetStretch(scrollGo.GetComponent<RectTransform>(), 8, 44, 8, 8);
        scrollGo.GetComponentInChildren<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        tab.AddComponent<UpgradeScreen>();
        return tab;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BOOK BUTTON  (bottom-right corner)
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildBookButton(Transform root)
    {
        var btn = Panel("BookButton", root, new Vector2(80, 80),
            pivot: new Vector2(1, 0), anchorMin: new Vector2(1, 0), anchorMax: new Vector2(1, 0),
            pos: new Vector2(-14, 14), color: new Color(0.2f, 0.3f, 0.7f));

        var lbl = Label("Label", btn.transform, "📖", 28, White);
        lbl.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretch(lbl.GetComponent<RectTransform>(), 0, 0, 0, 0);

        var button = btn.AddComponent<Button>();
        button.targetGraphic = btn.GetComponent<Image>();
        // Wire up onClick -> HUDController.ToggleFishpedia in Inspector
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FISHING POPUP
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildFishingPopup(Transform root)
    {
        var popup = Panel("FishingPopup", root, new Vector2(260, 120),
            pivot: Vector2.one * 0.5f, anchorMin: Vector2.one * 0.5f, anchorMax: Vector2.one * 0.5f,
            pos: Vector2.zero, color: PanelBorder);

        var inner = Panel("BG", popup.transform, new Vector2(252, 112),
            Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f,
            Vector2.zero, PanelBg);

        var catchBtn = Panel("CatchButton", inner.transform, new Vector2(200, 60),
            Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f,
            Vector2.zero, ButtonNormal);
        var catchBtnComp = catchBtn.AddComponent<Button>();
        catchBtnComp.targetGraphic = catchBtn.GetComponent<Image>();

        var catchLbl = Label("CatchLabel", catchBtn.transform, "CATCH!", 20, White);
        catchLbl.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        catchLbl.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretch(catchLbl.GetComponent<RectTransform>(), 0, 0, 0, 0);

        popup.AddComponent<FishingPopup>();
        popup.SetActive(false);
        // Assign popupRoot=popup, catchButton=catchBtnComp in Inspector
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATCH NOTIFICATION
    // ─────────────────────────────────────────────────────────────────────────
    static void BuildCatchNotification(Transform root)
    {
        var notif = Panel("CatchNotification", root, new Vector2(320, 80),
            pivot: new Vector2(0.5f, 1), anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1),
            pos: new Vector2(0, -10), color: PanelBorder);

        var inner = Panel("BG", notif.transform, new Vector2(312, 72),
            Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f,
            Vector2.zero, PanelBg);

        var fishIcon = Panel("FishIcon", inner.transform, new Vector2(56, 56),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f),
            new Vector2(8, 0), Clear);
        fishIcon.AddComponent<Image>();

        var rewardText = Label("RewardText", inner.transform, "+0  +0xp", 16, TextDark);
        rewardText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        var rRt = rewardText.GetComponent<RectTransform>();
        rRt.anchorMin = new Vector2(0, 0);
        rRt.anchorMax = new Vector2(1, 1);
        rRt.offsetMin = new Vector2(72, 0);
        rRt.offsetMax = new Vector2(-8, 0);

        notif.AddComponent<CatchNotification>();
        notif.SetActive(false);
        // Assign notificationRoot=notif, fishIcon, rewardText in Inspector
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    static GameObject Panel(string name, Transform parent, Vector2 size,
        Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.pivot = pivot;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        return go;
    }

    static GameObject Label(string name, Transform parent, string text, float size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        return go;
    }

    static GameObject BuildSlider(string name, Transform parent, Vector2 size, Vector2 pos,
        Color bgColor, Color fillColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.pivot = new Vector2(0, 1);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        var bgImg = go.AddComponent<Image>();
        bgImg.color = bgColor;

        var fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(go.transform, false);
        var faRt = fillArea.AddComponent<RectTransform>();
        faRt.anchorMin = Vector2.zero;
        faRt.anchorMax = Vector2.one;
        faRt.offsetMin = Vector2.zero;
        faRt.offsetMax = Vector2.zero;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = fillColor;
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0.5f, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var slider = go.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.5f;
        slider.interactable = false;

        return go;
    }

    static GameObject BuildScrollGrid(string name, Transform parent, Vector2 size, Vector2 pos,
        Vector2 cellSize, Vector2 spacing, int columns)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var sr = go.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(go.transform, false);
        var vpRt = viewport.AddComponent<RectTransform>();
        vpRt.anchorMin = Vector2.zero;
        vpRt.anchorMax = Vector2.one;
        vpRt.offsetMin = Vector2.zero;
        vpRt.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = Clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var cRt = content.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0, 1);
        cRt.anchorMax = new Vector2(1, 1);
        cRt.pivot = new Vector2(0.5f, 1);
        cRt.offsetMin = Vector2.zero;
        cRt.offsetMax = Vector2.zero;

        var glg = content.AddComponent<GridLayoutGroup>();
        glg.cellSize = cellSize;
        glg.spacing = spacing;
        glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
        glg.startAxis = GridLayoutGroup.Axis.Horizontal;
        glg.childAlignment = TextAnchor.UpperLeft;
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = columns;

        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.viewport = vpRt;
        sr.content = cRt;

        return go;
    }

    static GameObject BuildCloseButton(string name, Transform parent, Vector2 pos)
    {
        var go = Panel(name, parent, new Vector2(28, 28),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f),
            pos, new Color(0.8f, 0.2f, 0.1f));
        var lbl = Label("X", go.transform, "✕", 14, White);
        lbl.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretch(lbl.GetComponent<RectTransform>(), 0, 0, 0, 0);
        go.AddComponent<Button>().targetGraphic = go.GetComponent<Image>();
        return go;
    }

    static GameObject BuildSpineButton(string name, Transform parent, Vector2 pos, string label)
    {
        var go = Panel(name, parent, new Vector2(54, 54),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            pos, new Color(0.5f, 0.3f, 0.15f));
        var lbl = Label("Label", go.transform, label, 16, White);
        lbl.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        lbl.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        SetStretch(lbl.GetComponent<RectTransform>(), 0, 0, 0, 0);
        go.AddComponent<Button>().targetGraphic = go.GetComponent<Image>();
        return go;
    }

    static GameObject BuildArrowButton(string name, Transform parent, string arrow,
        Vector2 pivot, Vector2 pos)
    {
        var go = Panel(name, parent, new Vector2(36, 28),
            pivot, pivot, pivot, pos, ButtonNormal);
        var lbl = Label("Arrow", go.transform, arrow, 14, White);
        lbl.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        SetStretch(lbl.GetComponent<RectTransform>(), 0, 0, 0, 0);
        go.AddComponent<Button>().targetGraphic = go.GetComponent<Image>();
        return go;
    }

    static void SetStretch(RectTransform rt, float l, float t, float r, float b)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(l, b);
        rt.offsetMax = new Vector2(-r, -t);
    }

    static void SetTopBar(RectTransform rt, float height, float margin)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.offsetMin = new Vector2(margin, -margin - height);
        rt.offsetMax = new Vector2(-margin, -margin);
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        return c;
    }
}
#endif
