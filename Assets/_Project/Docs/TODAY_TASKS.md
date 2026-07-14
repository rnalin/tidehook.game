# Hoje — Tarefas Prioritárias (1 semana MVP Windows)

Data: 2026-05-29
Objetivo: entregar MVP jogável Windows em 1 semana — começar agora.

## Checklist rápido (ordem)

1. Cena e Build
- [ ] Confirmar cena ativa no Build: `Assets/Scenes/SampleScene.unity` (temporário) ou renomear para `GameScene`.
- [ ] Abrir `File > Build Settings` e garantir a cena única marcada.

2. Atribuições no Inspector (P0)
- [ ] `GameManager` — atribuir: `xpSystem`, `currencySystem`, `fishingController`, `houseController`.
- [ ] `HUDController` — atribuir: `levelText`, `coinsText`, `xpSlider`, `doubleIndicator`, `doubleTimerText`, `doubleButton`, `fishingController`.
- [ ] `FishingPopup` — `popupRoot`, `catchButton`, `fishingController`.
- [ ] `BaitScreen` — `screenRoot`, `listParent`, `baitCardPrefab`, `closeButton`, `fishingController`, `baits`.
- [ ] `UpgradeScreen` — `screenRoot`, `listParent`, `upgradeItemPrefab`, `closeButton`, `houseController`.

3. Conteúdo mínimo
- [ ] Verificar existência de 3 `FishData` e 3 `BaitData` em `Assets/_Project/ScriptableObjects` (se não existir, criar placeholders).
- [ ] Verificar `UpgradeData` para T1..T5 (pelo menos T1 e T2 para demo).

4. Teste rápido no Editor (15 min)
- [ ] Rodar o jogo no Editor por 15 minutos e cobrir fluxo: popup → clique → recompensa → compra upgrade → salvar → fechar Editor → reabrir e verificar progresso.
- [ ] Anotar qualquer NullReferenceException ou comportamento bloqueante.

5. Build Windows Dev
- [ ] Gerar build `Development` e testar o `.exe` fora do Editor.

## Instrucoes de build (exemplo)
Se preferir usar o Unity em linha de comando (ajuste o caminho do Unity):

```powershell
"C:\Program Files\Unity\Hub\Editor\6.0.4f1\Editor\Unity.exe" -projectPath "C:\Users\nalin\Projects\tidehook.game" -quit -batchmode -buildWindows64Player "C:\Users\nalin\Projects\tidehook.game\Builds\Tidehook_Windows\Tidehook.exe" -logFile "build_log.txt"
```

Observacao: o comando acima é um exemplo; confirme o caminho do Editor e ajuste se necessario.

## Critérios de aceite para hoje
- [ ] Jogo roda no Editor e no `.exe` sem NullReferenceException por 15 minutos.
- [ ] Save/Load preserva moedas, XP e upgrades.

---

Se quiser que eu crie assets placeholders (3 `FishData`, 3 `BaitData`, 2 `UpgradeData`) posso gerá-los como arquivos `.asset`, mas isso exige abrir o Unity — posso criar instrucoes e scripts de editor para automatizar, se desejar.