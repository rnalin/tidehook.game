# Tidehook Roadmap (MVP -> Play Store)

Atualizado em: 2026-05-29
Horizonte: 8-10 semanas
Meta: publicar o MVP de Tidehook na Google Play com qualidade de primeira release.

## 1) Visao geral

- Escopo do MVP: loop idle de pesca, progressao 1-10, 3 iscas, 5 upgrades de casa, save/load, rewarded ads.
- Fora do MVP: multiplayer, cloud save, eventos sazonais, battle pass, loja IAP complexa.
- Plataforma-alvo inicial: Android (Play Store).

## 2) Estado atual (snapshot)

- Base de scripts principal ja existe (Core, Fishing, Progression, UI, House, Ads).
- Cena de build ainda aponta para `SampleScene`.
- Configuracoes de publicacao Android ainda precisam alinhamento (bundle id final, SDK target, etc).
- Integracao real de AdMob ainda esta em TODO.

## 3) Marcos e fases

## Fase A - Fundacao e configuracao (Semana 1)

Objetivo: deixar o projeto pronto para evolucao segura e builds frequentes.

Checklist:
- [ ] Renomear/mover cena principal para GameScene e atualizar Build Settings.
- [ ] Definir Company Name e Bundle ID final.
- [ ] Fixar orientacao Portrait e validar em 2 resolucoes.
- [ ] Definir convencao de versionamento (`versionName` e `versionCode`).
- [ ] Criar checklist de smoke test rapido (5 minutos por build).

Criterio de saida:
- [ ] Build Development instala e abre sem erros em 2 dispositivos Android.

## Fase B - Gameplay MVP completo (Semanas 2-3)

Objetivo: fechar o loop principal e progressao completa do produto.

Checklist:
- [ ] Finalizar tabelas de drop por isca (incluindo raridades finais do MVP).
- [ ] Validar formula de XP e cap de nivel 10.
- [ ] Completar upgrades T1-T5 com bloqueios por nivel/moedas.
- [ ] Garantir troca visual da casa por tier comprado.
- [ ] Fechar save/load de moedas, XP, nivel, isca ativa e upgrades.

Criterio de saida:
- [ ] Sessao de 15 min sem bug bloqueante, sem perda de progresso.

## Fase C - UX, conteudo e balanceamento (Semanas 4-5)

Objetivo: melhorar legibilidade, satisfacao e ritmo de progressao.

Checklist:
- [ ] Polir notificacoes de captura, level up e estados de isca.
- [ ] Integrar audio base (loop ambiente + SFX de acao/recompensa).
- [ ] Ajustar economia (ganho de XP/moedas vs custos de upgrade).
- [ ] Revisar onboarding implicito (primeiros 3 minutos sem confusao).
- [ ] Rodar playtests curtos com feedback estruturado.

Criterio de saida:
- [ ] Usuarios de teste entendem o loop sem explicacao externa.

## Fase D - Monetizacao e observabilidade (Semana 6)

Objetivo: habilitar operacao real e monitorar saude da release.

Checklist:
- [ ] Integrar AdMob rewarded real (test IDs e production IDs separados).
- [ ] Tratar falhas de carregamento de anuncio sem quebrar fluxo.
- [ ] Limitar exibicao por sessao e validar regras de recompensa.
- [ ] Integrar analytics basico (inicio sessao, peixe capturado, upgrade comprado, ad assistido).
- [ ] Integrar crash reporting.

Criterio de saida:
- [ ] Rewarded funciona em dispositivo real com telemetria confirmada.

## Fase E - QA e performance mobile (Semana 7)

Objetivo: reduzir risco de rejeicao e de reviews negativas no lancamento.

Checklist:
- [ ] Testar em matriz de dispositivos (Android 9+ e telas pequenas/grandes).
- [ ] Validar comportamento com app em pause/resume e sem internet.
- [ ] Revisar uso de memoria, draw calls e stutter.
- [ ] Corrigir bugs P0/P1 e congelar escopo.

Criterio de saida:
- [ ] Zero bug bloqueante aberto.

## Fase F - Preparacao Play Store (Semana 8)

Objetivo: deixar pacote, metadados e compliance prontos para submissao.

Checklist:
- [ ] Gerar AAB assinado com keystore segura.
- [ ] Preparar assets da loja (icone, feature graphic, screenshots).
- [ ] Escrever descricao curta/longa PT-BR (e EN se desejado).
- [ ] Publicar politica de privacidade.
- [ ] Preencher Data Safety corretamente.
- [ ] Configurar teste fechado na Play Console.

Criterio de saida:
- [ ] App pronto para submissao sem pendencias de compliance.

## Fase G - Teste fechado e publicacao (Semanas 9-10)

Objetivo: validar com usuarios reais e publicar com risco controlado.

Checklist:
- [ ] Rodar teste fechado com grupo minimo de testers.
- [ ] Priorizar e corrigir bugs criticos.
- [ ] Ajustar balanceamento final com base em dados.
- [ ] Submeter para review.
- [ ] Publicar e monitorar primeira semana.

Criterio de saida:
- [ ] App publicado e com plano de hotfix pronto.

## 4) Definicao de pronto por entrega

Cada tarefa so fecha quando tiver:
- [ ] Implementacao concluida.
- [ ] Teste manual minimo executado em device.
- [ ] Sem regressao evidente no loop principal.
- [ ] Evidencia rapida (print/video/log) anexada no board.

## 5) Backlog pos-lancamento (nao entra no MVP)

- [ ] Colecao/Pedia completa de peixes.
- [ ] Novos biomas/cenarios.
- [ ] Mais iscas e sistema de raridade expandido.
- [ ] Eventos temporarios.
- [ ] Localizacao adicional.

## 6) Riscos principais e mitigacao

1. Risco: atrasos por polimento infinito.
   - Mitigacao: congelar escopo no fim da Semana 7.
2. Risco: problemas com anuncios em producao.
   - Mitigacao: validar test IDs cedo e fallback sem travar gameplay.
3. Risco: rejeicao por compliance da Play.
   - Mitigacao: preparar politica/data safety antes da submissao.
4. Risco: bugs em aparelhos de entrada.
   - Mitigacao: QA em dispositivo fraco desde Semana 5.

## 7) Cadencia sugerida de trabalho

- Segunda: planejamento semanal + metas fechadas.
- Terca a Quinta: implementacao focada por fase.
- Sexta: build, smoke test, revisao de riscos e replanejamento.

---

Se quiser, o proximo passo e transformar este roadmap em um board de execucao (Sprint 1 a Sprint 5), com responsavel, estimativa e dependencia por tarefa.
