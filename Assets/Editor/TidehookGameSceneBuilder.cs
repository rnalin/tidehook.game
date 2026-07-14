using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class TidehookGameSceneBuilder
{
    const string ScenePath = "Assets/Scenes/GameScene.unity";
    const string AutoCreateMarkerPath = "Assets/Editor/TidehookCreateGameScene.once.txt";
    const string ScriptableFolder = "Assets/_Project/ScriptableObjects";
    const string PrefabFolder = "Assets/_Project/Prefabs/UI";

    [InitializeOnLoadMethod]
    static void AutoCreateGameSceneWhenRequested()
    {
        if (!File.Exists(AutoCreateMarkerPath)) return;

        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(AutoCreateMarkerPath)) return;

            File.Delete(AutoCreateMarkerPath);
            var metaPath = AutoCreateMarkerPath + ".meta";
            if (File.Exists(metaPath)) File.Delete(metaPath);
            AssetDatabase.Refresh();

            CreateGameScene();
        };
    }

    [MenuItem("Tidehook/Create GameScene")]
    public static void CreateGameScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets/_Project", "ScriptableObjects");
        EnsureFolder("Assets/_Project", "Prefabs");
        EnsureFolder("Assets/_Project/Prefabs", "UI");

        var fish = CreateOrUpdateFishData();
        var baits = CreateOrUpdateBaitData(fish);
        var upgrades = CreateOrUpdateUpgradeData();
        var baitCardPrefab = EnsureBaitCardPrefab();
        var upgradeItemPrefab = EnsureUpgradeItemPrefab();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "GameScene";

        var systems = CreateRoot("Systems");
        var world = CreateRoot("World");
        var ui = CreateRoot("UI");

        var fishingController = CreateSystem<FishingController>(systems.transform, "FishingController");
        SetObjectArray(fishingController, "baits", baits);

        var xpSystem = CreateSystem<XPSystem>(systems.transform, "XPSystem");
        var currencySystem = CreateSystem<CurrencySystem>(systems.transform, "CurrencySystem");
        var houseController = CreateSystem<HouseController>(systems.transform, "HouseController");
        var adManager = CreateSystem<AdManager>(systems.transform, "AdManager");
        var gameManager = CreateSystem<GameManager>(systems.transform, "GameManager");

        _ = adManager;
        var houseTiers = CreateWorld(world.transform);

        SetObject(gameManager, "xpSystem", xpSystem);
        SetObject(gameManager, "currencySystem", currencySystem);
        SetObject(gameManager, "fishingController", fishingController);
        SetObject(gameManager, "houseController", houseController);
        SetObjectArray(houseController, "upgrades", upgrades);
        SetObjectArray(houseController, "houseTiers", houseTiers);

        CreateCamera();
        CreateUI(ui.transform, fishingController, houseController, baits, baitCardPrefab, upgradeItemPrefab);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Tidehook GameScene created at " + ScenePath);
    }

    static GameObject[] CreateWorld(Transform parent)
    {
        var background = CreateChild(parent, "Background");
        CreateSprite(background.transform, "SkyOcean", "Assets/_Project/Art/Home/sky ocean.png", new Vector3(0f, 1.2f, 5f), new Vector3(5.6f, 5.6f, 1f), 0);
        CreateSprite(background.transform, "Horizon", "Assets/_Project/Art/Home/horizon.png", new Vector3(0f, 0.7f, 4f), new Vector3(5.6f, 5.6f, 1f), 1);
        CreateSprite(background.transform, "Clouds", "Assets/_Project/Art/Home/clouds.png", new Vector3(0f, 3.05f, 3f), new Vector3(5.2f, 5.2f, 1f), 2);
        CreateSprite(background.transform, "CloudReflection", "Assets/_Project/Art/Home/clouds w reflect.png", new Vector3(0f, 1.55f, 3f), new Vector3(5.2f, 5.2f, 1f), 2);

        CreateSprite(parent, "Deck", "Assets/_Project/Art/Home/deck 1.png", new Vector3(0f, -3.25f, 0f), new Vector3(5.5f, 5.5f, 1f), 6);

        var player = CreateChild(parent, "Player");
        var character = CreateSprite(player.transform, "FishingCharacter", "Assets/_Project/Art/Entities/char fishing sprite sheet.png", new Vector3(-1.25f, -2.45f, 0f), new Vector3(3.3f, 3.3f, 1f), 7);
        var animator = character.AddComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_Project/Animations/char fishing sprite sheet_0.controller");
        var characterAnimator = character.AddComponent<FishingCharacterAnimator>();
        SetObject(characterAnimator, "animator", animator);

        var house = CreateChild(parent, "House");
        var tiers = new GameObject[5];
        var colors = new[]
        {
            Color.white,
            new Color(1f, 0.92f, 0.78f),
            new Color(0.82f, 1f, 0.9f),
            new Color(0.78f, 0.92f, 1f),
            new Color(1f, 0.84f, 0.95f),
        };

        for (int i = 0; i < tiers.Length; i++)
        {
            var tier = CreateSprite(house.transform, "HouseTier_" + (i + 1).ToString("00"), "Assets/_Project/Art/Home/shack 1.png", new Vector3(1.35f, -2.25f, 0f), new Vector3(3.6f + i * 0.12f, 3.6f + i * 0.12f, 1f), 5 + i);
            tier.GetComponent<SpriteRenderer>().color = colors[i];
            tier.SetActive(i == 0);
            tiers[i] = tier;
        }

        return tiers;
    }

    static void CreateCamera()
    {
        var cameraGo = new GameObject("Camera");
        cameraGo.tag = "MainCamera";
        cameraGo.transform.position = new Vector3(0f, 0f, -10f);
        var camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.4f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.42f, 0.78f, 0.95f);
        cameraGo.AddComponent<AudioListener>();
    }

    static void CreateUI(Transform parent, FishingController fishingController, HouseController houseController, BaitData[] baits, GameObject baitCardPrefab, GameObject upgradeItemPrefab)
    {
        var canvasGo = CreateChild(parent, "Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<GraphicRaycaster>();

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        var hud = CreateHUD(canvasGo.transform, fishingController);
        var fishingPopup = CreateFishingPopup(canvasGo.transform, fishingController);
        var catchNotification = CreateCatchNotification(canvasGo.transform);
        var baitScreen = CreateBaitScreen(canvasGo.transform, fishingController, baits, baitCardPrefab);
        var upgradeScreen = CreateUpgradeScreen(canvasGo.transform, houseController, upgradeItemPrefab);
        CreateBottomMenu(canvasGo.transform, baitScreen, upgradeScreen);
        CreateDebugPanel(canvasGo.transform);

        _ = hud;
        _ = fishingPopup;
        _ = catchNotification;

        var eventSystem = CreateChild(parent, "EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }

    static HUDController CreateHUD(Transform parent, FishingController fishingController)
    {
        var hud = CreateUIObject(parent, "HUD", new Vector2(0f, 0f), new Vector2(0f, 0f));
        Stretch(hud.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var controller = hud.AddComponent<HUDController>();

        var topBar = CreatePanel(hud.transform, "TopBar", new Color(0f, 0f, 0f, 0.28f));
        Anchor(topBar.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(980f, 150f), new Vector2(0f, -95f));

        var levelText = CreateText(topBar.transform, "LevelText", "Lv 1", 38f, TextAlignmentOptions.Left);
        Anchor(levelText.rectTransform, new Vector2(0f, 0.5f), new Vector2(190f, 70f), new Vector2(70f, 25f));

        var coinsText = CreateText(topBar.transform, "CoinsText", "0", 38f, TextAlignmentOptions.Right);
        Anchor(coinsText.rectTransform, new Vector2(1f, 0.5f), new Vector2(280f, 70f), new Vector2(-160f, 25f));

        var xpSliderGo = CreateUIObject(topBar.transform, "XPBar", Vector2.zero, new Vector2(580f, 34f));
        Anchor(xpSliderGo.GetComponent<RectTransform>(), new Vector2(0.5f, 0.18f), new Vector2(620f, 34f), Vector2.zero);
        var xpSlider = xpSliderGo.AddComponent<Slider>();
        xpSlider.transition = Selectable.Transition.None;
        xpSlider.interactable = false;
        CreateSliderVisuals(xpSliderGo.transform, xpSlider);

        var doubleButton = CreateButton(hud.transform, "DoubleButton", "2x", new Color(0.95f, 0.62f, 0.22f));
        Anchor(doubleButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(160f, 88f), new Vector2(-120f, -215f));

        var doubleIndicator = CreatePanel(hud.transform, "DoubleIndicator", new Color(0.95f, 0.62f, 0.22f, 0.85f));
        Anchor(doubleIndicator.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(180f, 58f), new Vector2(-128f, -305f));
        var doubleTimerText = CreateText(doubleIndicator.transform, "DoubleTimerText", "180s", 30f, TextAlignmentOptions.Center);
        Stretch(doubleTimerText.rectTransform, 0f, 0f, 0f, 0f);
        doubleIndicator.SetActive(false);

        SetObject(controller, "levelText", levelText);
        SetObject(controller, "coinsText", coinsText);
        SetObject(controller, "xpSlider", xpSlider);
        SetObject(controller, "doubleIndicator", doubleIndicator);
        SetObject(controller, "doubleTimerText", doubleTimerText);
        SetObject(controller, "doubleButton", doubleButton.GetComponent<Button>());
        SetObject(controller, "fishingController", fishingController);

        return controller;
    }

    static FishingPopup CreateFishingPopup(Transform parent, FishingController fishingController)
    {
        var root = CreateUIObject(parent, "FishingPopup", Vector2.zero, Vector2.zero);
        Stretch(root.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var controller = root.AddComponent<FishingPopup>();

        var popupRoot = CreatePanel(root.transform, "PopupRoot", new Color(0.05f, 0.18f, 0.22f, 0.86f));
        Anchor(popupRoot.GetComponent<RectTransform>(), new Vector2(0.5f, 0.42f), new Vector2(520f, 260f), Vector2.zero);

        var title = CreateText(popupRoot.transform, "TitleText", "Fisgou!", 52f, TextAlignmentOptions.Center);
        Anchor(title.rectTransform, new Vector2(0.5f, 0.72f), new Vector2(420f, 80f), Vector2.zero);

        var catchButton = CreateButton(popupRoot.transform, "CatchButton", "Puxar", new Color(0.22f, 0.74f, 0.58f));
        Anchor(catchButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.28f), new Vector2(300f, 92f), Vector2.zero);

        SetObject(controller, "popupRoot", popupRoot);
        SetObject(controller, "catchButton", catchButton.GetComponent<Button>());
        SetObject(controller, "fishingController", fishingController);

        popupRoot.SetActive(false);
        return controller;
    }

    static CatchNotification CreateCatchNotification(Transform parent)
    {
        var root = CreateUIObject(parent, "CatchNotification", Vector2.zero, Vector2.zero);
        Stretch(root.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var controller = root.AddComponent<CatchNotification>();

        var notificationRoot = CreatePanel(root.transform, "NotificationRoot", new Color(0.06f, 0.1f, 0.12f, 0.88f));
        Anchor(notificationRoot.GetComponent<RectTransform>(), new Vector2(0.5f, 0.78f), new Vector2(560f, 120f), Vector2.zero);

        var fishIcon = CreateImage(notificationRoot.transform, "FishIcon", LoadSprite("Assets/_Project/Art/UI Components/Fishes/fish 1 small.png"), Color.white);
        Anchor(fishIcon.rectTransform, new Vector2(0f, 0.5f), new Vector2(92f, 92f), new Vector2(74f, 0f));

        var rewardText = CreateText(notificationRoot.transform, "RewardText", "+0  +0xp", 34f, TextAlignmentOptions.Left);
        Anchor(rewardText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(360f, 76f), new Vector2(74f, 0f));

        SetObject(controller, "notificationRoot", notificationRoot);
        SetObject(controller, "fishIcon", fishIcon);
        SetObject(controller, "rewardText", rewardText);

        notificationRoot.SetActive(false);
        return controller;
    }

    static BaitScreen CreateBaitScreen(Transform parent, FishingController fishingController, BaitData[] baits, GameObject baitCardPrefab)
    {
        var root = CreateUIObject(parent, "BaitScreen", Vector2.zero, Vector2.zero);
        Stretch(root.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var controller = root.AddComponent<BaitScreen>();

        var screenRoot = CreatePanel(root.transform, "ScreenRoot", new Color(0.03f, 0.11f, 0.15f, 0.94f));
        Stretch(screenRoot.GetComponent<RectTransform>(), 72f, 230f, 72f, 220f);

        var title = CreateText(screenRoot.transform, "Header", "Iscas", 48f, TextAlignmentOptions.Left);
        Anchor(title.rectTransform, new Vector2(0f, 1f), new Vector2(420f, 90f), new Vector2(70f, -70f));

        var closeButton = CreateButton(screenRoot.transform, "CloseButton", "X", new Color(0.84f, 0.22f, 0.22f));
        Anchor(closeButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(82f, 82f), new Vector2(-70f, -70f));

        var scrollView = CreateScrollView(screenRoot.transform, "ScrollView", out var listParent);
        Stretch(scrollView.GetComponent<RectTransform>(), 46f, 150f, 46f, 50f);

        SetObject(controller, "screenRoot", screenRoot);
        SetObject(controller, "listParent", listParent);
        SetObject(controller, "baitCardPrefab", baitCardPrefab);
        SetObject(controller, "closeButton", closeButton.GetComponent<Button>());
        SetObject(controller, "fishingController", fishingController);
        SetObjectArray(controller, "baits", baits);

        screenRoot.SetActive(false);
        return controller;
    }

    static UpgradeScreen CreateUpgradeScreen(Transform parent, HouseController houseController, GameObject upgradeItemPrefab)
    {
        var root = CreateUIObject(parent, "UpgradeScreen", Vector2.zero, Vector2.zero);
        Stretch(root.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var controller = root.AddComponent<UpgradeScreen>();

        var screenRoot = CreatePanel(root.transform, "ScreenRoot", new Color(0.04f, 0.09f, 0.12f, 0.94f));
        Stretch(screenRoot.GetComponent<RectTransform>(), 72f, 230f, 72f, 220f);

        var title = CreateText(screenRoot.transform, "Header", "Casa", 48f, TextAlignmentOptions.Left);
        Anchor(title.rectTransform, new Vector2(0f, 1f), new Vector2(420f, 90f), new Vector2(70f, -70f));

        var closeButton = CreateButton(screenRoot.transform, "CloseButton", "X", new Color(0.84f, 0.22f, 0.22f));
        Anchor(closeButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(82f, 82f), new Vector2(-70f, -70f));

        var scrollView = CreateScrollView(screenRoot.transform, "ScrollView", out var listParent);
        Stretch(scrollView.GetComponent<RectTransform>(), 46f, 150f, 46f, 50f);

        SetObject(controller, "screenRoot", screenRoot);
        SetObject(controller, "listParent", listParent);
        SetObject(controller, "upgradeItemPrefab", upgradeItemPrefab);
        SetObject(controller, "closeButton", closeButton.GetComponent<Button>());
        SetObject(controller, "houseController", houseController);

        screenRoot.SetActive(false);
        return controller;
    }

    static void CreateBottomMenu(Transform parent, BaitScreen baitScreen, UpgradeScreen upgradeScreen)
    {
        var menu = CreatePanel(parent, "BottomMenu", new Color(0.02f, 0.08f, 0.1f, 0.86f));
        Anchor(menu.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(940f, 138f), new Vector2(0f, 95f));

        var layout = menu.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(24, 24, 18, 18);
        layout.spacing = 18f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        var baitButton = CreateButton(menu.transform, "BaitButton", "Iscas", new Color(0.17f, 0.45f, 0.66f));
        var upgradeButton = CreateButton(menu.transform, "UpgradeButton", "Casa", new Color(0.34f, 0.56f, 0.36f));
        CreateButton(menu.transform, "PediaButton", "Pedia", new Color(0.35f, 0.34f, 0.56f));
        CreateButton(menu.transform, "BagButton", "Bolsa", new Color(0.54f, 0.4f, 0.24f));

        UnityEventTools.AddPersistentListener(baitButton.GetComponent<Button>().onClick, baitScreen.Open);
        UnityEventTools.AddPersistentListener(upgradeButton.GetComponent<Button>().onClick, upgradeScreen.Open);
    }

    static void CreateDebugPanel(Transform parent)
    {
        var panel = CreatePanel(parent, "DebugPanel", new Color(0f, 0f, 0f, 0.22f));
        Anchor(panel.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(360f, 82f), new Vector2(210f, 70f));
        var text = CreateText(panel.transform, "DebugText", "Debug", 26f, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, 0f, 0f, 0f, 0f);
        panel.SetActive(false);
    }

    static GameObject EnsureBaitCardPrefab()
    {
        var path = PrefabFolder + "/BaitCard.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var root = CreateUIObject(null, "BaitCard", Vector2.zero, new Vector2(840f, 150f));
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.09f, 0.2f, 0.24f, 1f);
        root.AddComponent<Button>();
        var card = root.AddComponent<BaitCardUI>();

        var icon = CreateImage(root.transform, "BaitIcon", LoadSprite("Assets/_Project/Art/UI Components/baits/1 basic bait.png"), Color.white);
        Anchor(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(96f, 96f), new Vector2(78f, 0f));

        var nameText = CreateText(root.transform, "NameText", "Isca", 32f, TextAlignmentOptions.Left);
        Anchor(nameText.rectTransform, new Vector2(0f, 0.62f), new Vector2(420f, 54f), new Vector2(170f, 0f));

        var lockText = CreateText(root.transform, "LockText", "Nivel 1", 24f, TextAlignmentOptions.Left);
        Anchor(lockText.rectTransform, new Vector2(0f, 0.33f), new Vector2(360f, 48f), new Vector2(170f, 0f));

        var selectButton = CreateButton(root.transform, "SelectButton", "Usar", new Color(0.22f, 0.62f, 0.52f));
        Anchor(selectButton.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(150f, 74f), new Vector2(-102f, 0f));

        var lockedOverlay = CreatePanel(root.transform, "LockedOverlay", new Color(0f, 0f, 0f, 0.54f));
        Stretch(lockedOverlay.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);

        var activeIndicator = CreatePanel(root.transform, "ActiveIndicator", new Color(0.95f, 0.82f, 0.18f, 1f));
        Anchor(activeIndicator.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(18f, 150f), Vector2.zero);

        SetObject(card, "baitIcon", icon);
        SetObject(card, "nameText", nameText);
        SetObject(card, "lockText", lockText);
        SetObject(card, "selectButton", selectButton.GetComponent<Button>());
        SetObject(card, "lockedOverlay", lockedOverlay);
        SetObject(card, "activeIndicator", activeIndicator);

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static GameObject EnsureUpgradeItemPrefab()
    {
        var path = PrefabFolder + "/UpgradeItem.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var root = CreateUIObject(null, "UpgradeItem", Vector2.zero, new Vector2(840f, 150f));
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.18f, 0.14f, 1f);
        var canvasGroup = root.AddComponent<CanvasGroup>();
        var item = root.AddComponent<UpgradeItemUI>();

        var nameText = CreateText(root.transform, "NameText", "Upgrade", 32f, TextAlignmentOptions.Left);
        Anchor(nameText.rectTransform, new Vector2(0f, 0.62f), new Vector2(420f, 54f), new Vector2(44f, 0f));

        var costText = CreateText(root.transform, "CostText", "0 moedas", 26f, TextAlignmentOptions.Left);
        Anchor(costText.rectTransform, new Vector2(0f, 0.33f), new Vector2(420f, 48f), new Vector2(44f, 0f));

        var buyButton = CreateButton(root.transform, "BuyButton", "Comprar", new Color(0.36f, 0.64f, 0.34f));
        Anchor(buyButton.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(190f, 74f), new Vector2(-124f, 0f));

        var purchasedBadge = CreatePanel(root.transform, "PurchasedBadge", new Color(0.95f, 0.82f, 0.18f, 1f));
        Anchor(purchasedBadge.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(120f, 38f), new Vector2(-84f, -26f));
        var badgeText = CreateText(purchasedBadge.transform, "Text", "OK", 20f, TextAlignmentOptions.Center);
        Stretch(badgeText.rectTransform, 0f, 0f, 0f, 0f);

        SetObject(item, "nameText", nameText);
        SetObject(item, "costText", costText);
        SetObject(item, "buyButton", buyButton.GetComponent<Button>());
        SetObject(item, "purchasedBadge", purchasedBadge);
        SetObject(item, "canvasGroup", canvasGroup);

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static FishData[] CreateOrUpdateFishData()
    {
        var fish1 = CreateOrUpdateAsset<FishData>(ScriptableFolder + "/Fish_Peixinho.asset");
        fish1.fishName = "Peixinho";
        fish1.rarity = Rarity.Common;
        fish1.xpReward = 5;
        fish1.coinReward = 5;
        fish1.sprite = LoadSprite("Assets/_Project/Art/UI Components/Fishes/fish 1 small.png");

        var fish2 = CreateOrUpdateAsset<FishData>(ScriptableFolder + "/Fish_Sardinha.asset");
        fish2.fishName = "Sardinha";
        fish2.rarity = Rarity.Uncommon;
        fish2.xpReward = 10;
        fish2.coinReward = 15;
        fish2.sprite = LoadSprite("Assets/_Project/Art/UI Components/Fishes/fish 2 small.png");

        var fish3 = CreateOrUpdateAsset<FishData>(ScriptableFolder + "/Fish_Dourado.asset");
        fish3.fishName = "Dourado";
        fish3.rarity = Rarity.Rare;
        fish3.xpReward = 25;
        fish3.coinReward = 60;
        fish3.sprite = LoadSprite("Assets/_Project/Art/UI Components/Fishes/fish 1 large.png");

        EditorUtility.SetDirty(fish1);
        EditorUtility.SetDirty(fish2);
        EditorUtility.SetDirty(fish3);
        return new[] { fish1, fish2, fish3 };
    }

    static BaitData[] CreateOrUpdateBaitData(FishData[] fish)
    {
        var bait1 = CreateOrUpdateAsset<BaitData>(ScriptableFolder + "/Bait_Basica.asset");
        bait1.baitName = "Isca Basica";
        bait1.unlockLevel = 1;
        bait1.sprite = LoadSprite("Assets/_Project/Art/UI Components/baits/1 basic bait.png");
        bait1.dropTable = new[]
        {
            new FishDropEntry { fish = fish[0], weight = 60f },
            new FishDropEntry { fish = fish[1], weight = 30f },
            new FishDropEntry { fish = fish[2], weight = 10f },
        };

        var bait2 = CreateOrUpdateAsset<BaitData>(ScriptableFolder + "/Bait_Minhoca.asset");
        bait2.baitName = "Isca Minhoca";
        bait2.unlockLevel = 2;
        bait2.sprite = LoadSprite("Assets/_Project/Art/UI Components/baits/2 worm bait.png");
        bait2.dropTable = new[]
        {
            new FishDropEntry { fish = fish[0], weight = 42f },
            new FishDropEntry { fish = fish[1], weight = 40f },
            new FishDropEntry { fish = fish[2], weight = 18f },
        };

        var bait3 = CreateOrUpdateAsset<BaitData>(ScriptableFolder + "/Bait_Brilhante.asset");
        bait3.baitName = "Isca Brilhante";
        bait3.unlockLevel = 3;
        bait3.sprite = LoadSprite("Assets/_Project/Art/UI Components/baits/3 glowing bait.png");
        bait3.dropTable = new[]
        {
            new FishDropEntry { fish = fish[0], weight = 25f },
            new FishDropEntry { fish = fish[1], weight = 43f },
            new FishDropEntry { fish = fish[2], weight = 32f },
        };

        EditorUtility.SetDirty(bait1);
        EditorUtility.SetDirty(bait2);
        EditorUtility.SetDirty(bait3);
        return new[] { bait1, bait2, bait3 };
    }

    static UpgradeData[] CreateOrUpdateUpgradeData()
    {
        var upgrades = new UpgradeData[5];
        for (int i = 0; i < upgrades.Length; i++)
        {
            var tier = i + 1;
            var upgrade = CreateOrUpdateAsset<UpgradeData>(ScriptableFolder + "/Upgrade_T" + tier + ".asset");
            upgrade.upgradeName = "Cabana T" + tier;
            upgrade.tier = tier;
            upgrade.cost = i == 0 ? 0 : 75 * tier * tier;
            upgrade.requiredLevel = tier;
            upgrade.description = i == 0 ? "Cabana inicial." : "Melhoria visual da casa.";
            upgrade.previewSprite = LoadSprite("Assets/_Project/Art/Home/shack 1.png");
            upgrades[i] = upgrade;
            EditorUtility.SetDirty(upgrade);
        }

        return upgrades;
    }

    static T CreateOrUpdateAsset<T>(string path) where T : ScriptableObject
    {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null) return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    static GameObject CreateScrollView(Transform parent, string name, out Transform listParent)
    {
        var scrollView = CreateUIObject(parent, name, Vector2.zero, Vector2.zero);
        scrollView.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.14f);
        var scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        var viewport = CreateUIObject(scrollView.transform, "Viewport", Vector2.zero, Vector2.zero);
        Stretch(viewport.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
        var maskImage = viewport.AddComponent<Image>();
        maskImage.color = new Color(1f, 1f, 1f, 0.02f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        var content = CreateUIObject(viewport.transform, "Content", Vector2.zero, Vector2.zero);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);

        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 18f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        var fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;
        listParent = content.transform;
        return scrollView;
    }

    static void CreateSliderVisuals(Transform parent, Slider slider)
    {
        var background = CreateImage(parent, "Background", null, new Color(0.06f, 0.16f, 0.18f, 1f));
        Stretch(background.rectTransform, 0f, 0f, 0f, 0f);

        var fillArea = CreateUIObject(parent, "Fill Area", Vector2.zero, Vector2.zero);
        Stretch(fillArea.GetComponent<RectTransform>(), 6f, 6f, 6f, 6f);

        var fill = CreateImage(fillArea.transform, "Fill", null, new Color(0.16f, 0.72f, 0.88f, 1f));
        Stretch(fill.rectTransform, 0f, 0f, 0f, 0f);

        slider.fillRect = fill.rectTransform;
        slider.targetGraphic = fill;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
    }

    static T CreateSystem<T>(Transform parent, string name) where T : Component
    {
        var go = CreateChild(parent, name);
        return go.AddComponent<T>();
    }

    static GameObject CreateRoot(string name)
    {
        return new GameObject(name);
    }

    static GameObject CreateChild(Transform parent, string name)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        return go;
    }

    static GameObject CreateSprite(Transform parent, string name, string spritePath, Vector3 position, Vector3 scale, int sortingOrder)
    {
        var go = CreateChild(parent, name);
        go.transform.position = position;
        go.transform.localScale = scale;
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(spritePath);
        renderer.sortingOrder = sortingOrder;
        return go;
    }

    static GameObject CreateUIObject(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        if (parent != null) go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        return go;
    }

    static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go = CreateUIObject(parent, name, Vector2.zero, Vector2.zero);
        var image = go.AddComponent<Image>();
        image.color = color;
        return go;
    }

    static Image CreateImage(Transform parent, string name, Sprite sprite, Color color)
    {
        var go = CreateUIObject(parent, name, Vector2.zero, Vector2.zero);
        var image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.preserveAspect = sprite != null;
        return image;
    }

    static TextMeshProUGUI CreateText(Transform parent, string name, string text, float size, TextAlignmentOptions alignment)
    {
        var go = CreateUIObject(parent, name, Vector2.zero, Vector2.zero);
        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.alignment = alignment;
        label.color = Color.white;
        label.enableWordWrapping = false;
        return label;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Color color)
    {
        var buttonGo = CreateUIObject(parent, name, Vector2.zero, new Vector2(180f, 76f));
        var image = buttonGo.AddComponent<Image>();
        image.color = color;
        var button = buttonGo.AddComponent<Button>();
        button.targetGraphic = image;

        var text = CreateText(buttonGo.transform, "Text", label, 30f, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, 8f, 6f, 8f, 6f);
        return buttonGo;
    }

    static void Anchor(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 position)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }

    static void Stretch(RectTransform rect, float left, float top, float right, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    static Sprite LoadSprite(string path)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null) return sprite;

        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
    }

    static void EnsureFolder(string parent, string child)
    {
        var path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }

    static void SetObject(UnityEngine.Object target, string field, UnityEngine.Object value)
    {
        var so = new SerializedObject(target);
        var property = so.FindProperty(field);
        if (property == null)
        {
            Debug.LogWarning("Missing serialized field " + field + " on " + target.name);
            return;
        }

        property.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetObjectArray(UnityEngine.Object target, string field, UnityEngine.Object[] values)
    {
        var so = new SerializedObject(target);
        var property = so.FindProperty(field);
        if (property == null)
        {
            Debug.LogWarning("Missing serialized field " + field + " on " + target.name);
            return;
        }

        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
