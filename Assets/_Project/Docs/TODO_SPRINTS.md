# Tidehook TODO por Sprint (Execucao)

Atualizado em: 2026-05-29
Base: ROADMAP_PLAY_STORE.md
Formato: 5 sprints de 2 semanas (10 semanas)

## Regras de execucao

- Prioridade: P0 (bloqueante), P1 (alta), P2 (media).
- Status sugerido no board: Todo, Doing, Review, Done.
- Definicao de pronto por tarefa:
  - [ ] Implementado
  - [ ] Testado em dispositivo
  - [ ] Sem regressao do loop principal
  - [ ] Evidencia anexada (print/video/log)

## Sprint 1 (Semanas 1-2) - Fundacao + Loop estavel

Objetivo: consolidar base tecnica e garantir loop principal estavel no Android.

### Tarefas

1. [ ] [P0] Cena principal e Build Settings
- Estimativa: 4h
- Dependencia: nenhuma
- Entrega: cena GameScene ativa no build e fluxo inicial funcionando.

2. [ ] [P0] Identidade do app (Company + Bundle ID)
- Estimativa: 2h
- Dependencia: nenhuma
- Entrega: identificadores finais definidos para Android.

3. [ ] [P0] Orientacao portrait + Canvas scaler validado
- Estimativa: 3h
- Dependencia: cena principal pronta
- Entrega: UI legivel em 2 resolucoes de tela.

4. [ ] [P0] Save/Load de estado critico revisado
- Estimativa: 6h
- Dependencia: loop principal ativo
- Entrega: moedas, XP, nivel, isca ativa e upgrades persistem apos fechar/reabrir.

5. [ ] [P1] Smoke test padrao (roteiro 5 min)
- Estimativa: 2h
- Dependencia: build development
- Entrega: checklist de validacao rapida por build.

### Criterio de aceite da sprint
- [ ] Build Development instala em 2 dispositivos Android e roda por 10 min sem travamento.

## Sprint 2 (Semanas 3-4) - Progressao e conteudo MVP

Objetivo: fechar progressao 1-10 e conteudo principal (peixes, iscas, upgrades).

### Tarefas

1. [ ] [P0] Completar FishData/BaitData finais
- Estimativa: 8h
- Dependencia: nenhum bloqueio tecnico
- Entrega: peixes e pesos de drop finais definidos para MVP.

2. [ ] [P0] Validar formula XP e cap nivel 10
- Estimativa: 4h
- Dependencia: sistema XP ativo
- Entrega: progressao sem saltos anormais e estado max level coerente.

3. [ ] [P0] Upgrades T1-T5 completos
- Estimativa: 10h
- Dependencia: dados de upgrade definidos
- Entrega: compra, bloqueio por nivel/moedas e feedback de estado funcionando.

4. [ ] [P1] House tiers com troca visual correta
- Estimativa: 6h
- Dependencia: upgrades completos
- Entrega: visual da casa reflete progresso real do jogador.

5. [ ] [P1] Revisao de UX de popup/catch feedback
- Estimativa: 5h
- Dependencia: loop de pesca estavel
- Entrega: captura e recompensa claramente compreensiveis.

### Criterio de aceite da sprint
- [ ] Sessao de 15 min sem bug bloqueante e com progressao consistente.

## Sprint 3 (Semanas 5-6) - Polimento, audio e balanceamento

Objetivo: melhorar retencao da primeira sessao e sensacao de qualidade.

### Tarefas

1. [ ] [P1] Integrar trilha ambiente + SFX essenciais
- Estimativa: 6h
- Dependencia: eventos de gameplay estaveis
- Entrega: audio de acao e recompensa implementado e balanceado.

2. [ ] [P1] Polir feedback de level up e desbloqueios
- Estimativa: 5h
- Dependencia: sistema de progressao completo
- Entrega: jogador entende quando e por que desbloqueou algo.

3. [ ] [P0] Balancear economia (XP/moedas/custos)
- Estimativa: 10h
- Dependencia: conteudo MVP completo
- Entrega: ritmo de progressao adequado para 20-30 min de jogo inicial.

4. [ ] [P1] Playtests curtos (3-5 testers)
- Estimativa: 8h
- Dependencia: build jogavel
- Entrega: lista priorizada de friccoes da primeira sessao.

### Criterio de aceite da sprint
- [ ] Testers conseguem jogar e progredir sem explicacao externa.

## Sprint 4 (Semanas 7-8) - Monetizacao real + QA/performance

Objetivo: preparar operacao real sem comprometer estabilidade.

### Tarefas

1. [ ] [P0] Integrar AdMob Rewarded (teste + producao)
- Estimativa: 10h
- Dependencia: projeto Android configurado
- Entrega: rewarded funcional com callback de recompensa robusto.

2. [ ] [P0] Tratamento de falha de anuncio + fallback UX
- Estimativa: 4h
- Dependencia: AdMob integrado
- Entrega: jogo nao trava nem perde fluxo quando anuncio falha.

3. [ ] [P1] Integrar analytics basico de funil
- Estimativa: 6h
- Dependencia: eventos definidos
- Entrega: eventos de sessao/captura/upgrade/ad registrados.

4. [ ] [P1] Integrar crash reporting
- Estimativa: 3h
- Dependencia: servico escolhido
- Entrega: erros criticos visiveis em painel.

5. [ ] [P0] QA matriz de dispositivos (Android 9+)
- Estimativa: 12h
- Dependencia: build com monetizacao
- Entrega: relatorio de bugs P0/P1 corrigidos.

### Criterio de aceite da sprint
- [ ] Zero bug bloqueante aberto e monetizacao estavel em dispositivo real.

## Sprint 5 (Semanas 9-10) - Play Store, teste fechado e release

Objetivo: submeter, validar em teste fechado e publicar com risco controlado.

### Tarefas

1. [ ] [P0] Gerar AAB assinado e checklist de release
- Estimativa: 4h
- Dependencia: keystore e versao final
- Entrega: pacote pronto para Play Console.

2. [ ] [P0] Materiais da loja (icone, feature graphic, screenshots)
- Estimativa: 8h
- Dependencia: build visualmente estavel
- Entrega: assets aprovaveis para pagina da loja.

3. [ ] [P0] Compliance (politica de privacidade + Data Safety)
- Estimativa: 6h
- Dependencia: stack final de SDKs
- Entrega: formularios e links completos na Play Console.

4. [ ] [P1] Teste fechado com grupo minimo
- Estimativa: 10h
- Dependencia: app publicado em track fechado
- Entrega: feedback consolidado e bugs criticos tratados.

5. [ ] [P0] Submissao final e monitoramento da primeira semana
- Estimativa: 6h
- Dependencia: aprovado em teste fechado
- Entrega: app publicado com plano de hotfix ativo.

### Criterio de aceite da sprint
- [ ] App publicado na Google Play com acompanhamento diario de crash, reviews e funil.

## Quadro de riscos (operacional)

1. [ ] Risco de escopo crescer durante polimento
- Acao: congelar escopo no inicio da Sprint 4.

2. [ ] Risco de rejeicao por compliance
- Acao: revisar Data Safety e politica antes de subir AAB final.

3. [ ] Risco de performance em aparelho de entrada
- Acao: incluir device fraco obrigatorio nos testes da Sprint 4.

4. [ ] Risco de monetizacao instavel
- Acao: fallback sem recompensa duplicada e sem travar fluxo.

## KPI minimo de lancamento

- [ ] Crash-free sessions >= 98%
- [ ] Retencao D1 >= 25% (meta inicial)
- [ ] Taxa de rewarded por usuario ativo dentro de limite saudavel (sem spam)
- [ ] Avaliacao media inicial >= 4.0

---

Proximo passo recomendado: abrir este arquivo como backlog mestre e criar um board Kanban com colunas Todo/Doing/Review/Done usando os itens P0 primeiro.
