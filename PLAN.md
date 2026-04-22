# Tidehook — Plano de Implementação MVP

> Baseado no GDD v1.0. Nada fora deste plano entra antes do lançamento.

---

## Arquitetura de Scripts

```
Assets/_Project/Scripts/
├── Core/
│   ├── GameManager.cs          # Singleton, inicializa sistemas, controla estado global
│   └── SaveSystem.cs           # Persistência via JSON em Application.persistentDataPath
├── Data/
│   ├── FishData.cs             # ScriptableObject: nome, raridade, XP, moedas, sprite
│   ├── BaitData.cs             # ScriptableObject: nome, nível de desbloqueio, tabela de drops
│   └── UpgradeData.cs          # ScriptableObject: tier, nome, custo, nível req., prefab da casa
├── Fishing/
│   ├── FishingController.cs    # Core loop (coroutine 15s idle / 5s ativo), eventos de captura
│   └── DropTable.cs            # Weighted random: recebe BaitData, retorna FishData
├── Progression/
│   ├── PlayerProgress.cs       # Estado do jogador: XP, nível, moedas, upgrades comprados
│   ├── XPSystem.cs             # AddXP(), fórmula 100 × N^1.5, evento OnLevelUp
│   └── CurrencySystem.cs       # AddCoins(), SpendCoins(), evento OnCoinsChanged
├── House/
│   └── HouseController.cs      # Ativa/desativa GameObjects dos tiers baseado em upgrades comprados
├── UI/
│   ├── HUDController.cs        # Atualiza barra XP, nível, contador de moedas
│   ├── FishingPopup.cs         # Aparece aos 5s, botão de clique, esconde aos 15s
│   ├── CatchNotification.cs    # Notificação flutuante com sprite do peixe + valor
│   ├── UpgradeScreen.cs        # Lista os 5 tiers, habilita botão de compra
│   └── BaitScreen.cs           # 3 cards de isca, toque para ativar/desativar
└── Ads/
    └── AdManager.cs            # Wrapper AdMob: carrega rewarded ad, callback de recompensa
```

---

## Estrutura de Assets

```
Assets/_Project/
├── ScriptableObjects/
│   ├── Fish/                   # 5 FishData assets
│   ├── Baits/                  # 3 BaitData assets
│   └── Upgrades/               # 5 UpgradeData assets
├── Prefabs/
│   ├── UI/
│   │   ├── FishingPopup.prefab
│   │   ├── CatchNotification.prefab
│   │   ├── UpgradeScreen.prefab
│   │   └── BaitScreen.prefab
│   └── House/
│       ├── HouseTier1.prefab   # ...T2, T3, T4, T5
├── Art/
│   ├── Sprites/
│   │   ├── Character/          # idle.png, cast.png (placeholders → pixel art final)
│   │   ├── Fish/               # peixinho, sardinha, dourado, peixe-lua, kraken.png
│   │   ├── House/              # T1...T5, pier, agua
│   │   └── UI/                 # botões, frames, ícones
│   └── Animations/
│       ├── Character.anim
│       └── Water.anim
└── Audio/
    ├── Music/                  # ambient_loop.wav
    └── SFX/                    # splash.wav, catch_pop.wav, coin.wav
```

---

## Fase 1 — Protótipo Funcional (2 semanas)

**Objetivo:** Loop de pesca funcionando com placeholders. Sem arte final.

### Setup
- [ ] Renomear `SampleScene` → `GameScene`, mover para `Assets/_Project/Scenes/`
- [ ] Criar estrutura de pastas acima
- [ ] Configurar Company Name → "ReniusLab" em Project Settings
- [ ] Configurar orientação → Portrait (o jogo é vertical, bonequinho pescando pra baixo)

### Core Loop (`FishingController.cs`)
- [ ] Coroutine `FishingLoop()`: aguarda 15s → captura peixe automático
- [ ] Ao atingir 5s: dispara evento `OnPopupWindow()`, abre janela de 10s
- [ ] Método `OnPlayerClick()`: captura na hora e reinicia o ciclo
- [ ] Estado `IsDoubled` (para o rewarded ad): reduz intervalo de 15s → 7.5s por 3 min

### Sistema de Peixes (`FishData.cs` + `DropTable.cs`)
- [ ] `FishData`: campos `fishName`, `rarity`, `xp`, `coins`, `sprite`
- [ ] `DropTable.PickFish(BaitData)`: weighted random com `Random.Range`
- [ ] Criar 3 FishData assets para Fase 1: Peixinho (55%), Sardinha (25%), Dourado (12%)

### Progressão (`PlayerProgress.cs`, `XPSystem.cs`, `CurrencySystem.cs`)
- [ ] `XPSystem.AddXP(int amount)` → verifica level up com `100 × N^1.5`
- [ ] `CurrencySystem.AddCoins(int)` / `SpendCoins(int)` → retorna bool
- [ ] `PlayerProgress` salva estado via `SaveSystem` (JSON)

### HUD básico (`HUDController.cs`)
- [ ] TextMeshPro: nível atual, moedas
- [ ] Slider: barra de XP
- [ ] Subscribe nos eventos de `XPSystem` e `CurrencySystem`

### Cabana placeholder
- [ ] Sprite 2D simples (quad branco ou placeholder) na posição correta da cena
- [ ] `HouseController` preparado para receber os tiers (só T1 ativo)

---

## Fase 2 — Vertical Slice (3 semanas)

**Objetivo:** Jogável com arte final parcial, AdMob integrado, build Android.

### Arte & Animação
- [ ] Sprites finais: personagem idle + cast, água animada, pier, cabana T1 e T2
- [ ] Animator do personagem: `Idle` → `Cast` → `Idle` (trigger `CastFish`)
- [ ] Água: Animator com loop de onda (2 frames mínimo)
- [ ] Partícula ou sprite animado de splash ao lançar a isca

### FishingPopup (`FishingPopup.cs`)
- [ ] Canvas World Space ou Screen Space — botão grande centralizado na tela
- [ ] Animação de entrada (scale punch) ao aparecer no 5s
- [ ] Animação de saída ao chegar no 15s ou ao clicar
- [ ] Ícone do peixe capturado + "+X moedas +Y XP" flutuante

### XP + Level Up
- [ ] UI de level up: flash na tela + texto animado "LEVEL UP!"
- [ ] Desbloqueio de isca: notificação inline na BaitScreen
- [ ] Níveis 1–5 funcionando para a Fase 2

### Upgrades T1 e T2 (`UpgradeScreen.cs`)
- [ ] Lista vertical simples com `ScrollView`
- [ ] Cada item: nome, custo, botão "Comprar" (desabilitado se moedas insuficientes ou nível req. não atingido)
- [ ] Itens futuros visíveis, transparência 50%, sem interação
- [ ] `HouseController` troca sprite/ativa GameObject ao comprar T2

### AdMob (Rewarded)
- [ ] Adicionar Google AdMob SDK (Unity Package Manager ou `.unitypackage`)
- [ ] `AdManager.LoadRewardedAd()` no `Awake`
- [ ] Botão "Dobrar Pesca" na HUD → `AdManager.ShowRewardedAd()`
- [ ] Callback: `FishingController.ActivateDoubleMode(duration: 180s)`
- [ ] Contador de sessão: máximo 3 ads, reseta quando app fecha (`SessionAdCount`)

### Android Build
- [ ] Player Settings: Bundle ID `com.reniuslab.tidehook`
- [ ] Minimum API Level: Android 9 (API 28)
- [ ] Target API Level: API 35 (Play Store requirement)
- [ ] Ícone do app (placeholder 512×512 aceitável)
- [ ] Build & install em dispositivo físico para smoke test

---

## Fase 3 — MVP Completo (2 semanas)

**Objetivo:** Todos os sistemas do GDD implementados e testados.

### Peixes e Iscas completos
- [ ] Criar FishData para Peixe-Lua (6%) e Kraken Jr. (2%)
- [ ] `BaitData` Isca de Minhoca: ajustar probabilidades (Incomum +10%, Raro +8%, Comum -18%)
- [ ] `BaitData` Isca Luminosa: ajustar probabilidades (Épico +8%, Lendário +3%, Comum/Incomum reduzidos)
- [ ] `BaitScreen`: 3 cards com sprite, nome, status (ativo/travado/disponível)
- [ ] Toque na isca ativa troca `FishingController.currentBait`

### Casa completa
- [ ] Sprites T3 (Varanda), T4 (Torre), T5 (Farolim com luz animada)
- [ ] Prefabs de casa por tier, `HouseController` ativa por índice
- [ ] Animação da luz do farolim (blinking simples, 2 frames)

### Upgrades T3–T5
- [ ] UpgradeScreen lista todos os 5, scroll funcional
- [ ] Requisito de nível bloqueando corretamente

### Níveis 6–10
- [ ] Validar fórmula XP em todos os níveis
- [ ] Level 10: estado "MAX LEVEL", sem barra de XP (ou barra cheia locked)

### Save/Load completo
- [ ] `SaveSystem` persiste: moedas, XP, nível, isca ativa, upgrades comprados, `SessionAdCount`
- [ ] Load no `GameManager.Awake()`, Save em `OnApplicationPause(true)` e `OnApplicationQuit()`

### Áudio
- [ ] `AudioSource` com `ambient_loop.wav` em loop, volume 0.4
- [ ] `AudioManager` estático: `PlaySFX(AudioClip)` com pool de 3 AudioSources
- [ ] SFX: splash ao lançar, pop ao capturar, coin ao vender

### Testes em dispositivo
- [ ] Loop de 15s sem bugs por 10 minutos
- [ ] Rewarded ad não trava o jogo
- [ ] Save/load após fechar e reabrir o app
- [ ] Todos os 5 tiers da casa visivelmente diferentes

---

## Fase 4 — Lançamento (1 semana)

- [ ] Build de release assinada (Keystore `tidehook.keystore`)
- [ ] Testar APK release em 2 dispositivos diferentes
- [ ] Play Store: ícone 512×512, feature graphic 1024×500, 4 screenshots
- [ ] Descrição PT-BR (curta + longa) baseada na premissa do GDD
- [ ] Enviar para revisão Google Play
- [ ] Post de lançamento (Instagram/Twitter + grupos temáticos)

---

## Notas de Implementação

**Eventos (C# Actions):** Preferir eventos tipados a referências diretas entre sistemas.
```csharp
// Exemplo em XPSystem.cs
public static event Action<int, int> OnXPChanged;  // (current, max)
public static event Action<int> OnLevelUp;          // (newLevel)
```

**ScriptableObjects como dados:** Toda configuração de peixe/isca/upgrade vive em `.asset` — fácil de ajustar sem recompilar.

**Sem Coroutines na UI:** `FishingController` dispara eventos, UI escuta. Nenhuma coroutine no HUD.

**Orientação:** Portrait obrigatório. Canvas `CanvasScaler` → Scale With Screen Size, reference 390×844 (iPhone 14 base), match 0.5.

---

*Elaborado com Nalin & Claude | Abril 2026*
