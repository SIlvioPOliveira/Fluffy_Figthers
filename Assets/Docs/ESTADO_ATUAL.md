# Fluffy Fighters — resumo rápido do projeto

**Atualizado em:** 12/11/2026. **Referência:** integração do jogador no commit `4c4667f`.
Este arquivo é um ponto de entrada curto para conversas e tarefas futuras. Ele não substitui os GDDs, o `ROADMAP.md` ou as instruções de `AGENTS.md`. Confirme o estado atual no Git e na Unity: este resumo pode ficar desatualizado.

## Projeto e estado

- Jogo 3D para PC/Windows, single-player, feito na Unity `6000.3.11f1`.
- Cenas da build: `Assets/Scenes/MenuInicial.unity` e `Assets/Scenes/RegiaoAguaPrototipo.unity` (as quatro regiões estão na segunda cena).
- O jogador usa `CharacterController`, WASD relativo à câmera e câmera de terceira pessoa. O modelo animado foi integrado; o usuário testou e aprovou. A animação atual usa parado e caminhada para a frente. Movimento e câmera receberam suavização configurável; a sensação dos controles aguarda teste manual.
- Os 20 modelos de Pets estão importados. Há inventário, equipe de até três, fusão, combate, recompensas e Gachas nas regiões Água, Fogo, Lava e Gelo.
- Há três barreiras, quatro Bosses com padrões progressivos, tutorial, menus, HUD e salvamento local. A tela de vitória permite continuar jogando.
- O usuário relatou teste satisfatório do ciclo completo até o Boss final. Isso não substitui testes específicos de desempenho, reinício e recuperação de salvamento.
- Os 24 inimigos comuns usam quatro modelos regionais fornecidos pelo usuário; os fortes são 30% maiores que os fracos. Os quatro Bosses também usam os modelos fornecidos. Seleção, dano e reaparecimento foram verificados programaticamente; aparência, ataques e desempenho aguardam teste manual.
- Inventário, Gacha, HUD, menus, barreiras e vitória têm apresentação cartoon. Há uma build de demonstração anterior em `Builds/`; ela precede a integração do jogador animado e não representa necessariamente o estado mais recente.

## Decisões e cuidados

- Probabilidades do Gacha confirmadas: Comum **68,9%**, Raro **25%**, Épico **5%**, Lendário **1%**, Secreto **0,1%**. São configuráveis; não adicionar pity ao MVP.
- Barreiras custam **400 / 1.200 / 4.000** moedas. O GDD específico de mapa permite comprá-las sem derrotar o Boss; o resumo geral antigo é conflitante. Não alterar essa regra sem decisão do usuário.
- O jogo salva localmente moedas, Pets, equipamentos, progresso e barreiras. Não apagar nem sobrescrever o save do usuário durante testes; preferir cena sem persistência ou salvamento de teste separado.
- No Editor, a tecla **M** adiciona 1.000 moedas por padrão e pode alterar o save ativo. O atalho é exclusivo do Editor, fora das builds.
- O usuário prefere **uma nova branch para cada tarefa** e validação na própria Unity. Não gerar build para testes de rotina; gerar quando ele pedir. Commit e publish só quando solicitados.
- Usar o MCP da Unity quando conectado; verificar compilação, Console e funcionamento. Relatar claramente o que foi testado e o que depende de teste manual.
- O usuário gosta de receber uma sugestão objetiva do próximo passo ao final.

## Onde procurar

Leia `Assets/Docs/AGENTS.md` e consulte os documentos de `Assets/Docs/` conforme a tarefa. Para começar uma tarefa, use este mapa para localizar os GDDs mais diretamente relacionados; confira os demais quando houver dependências ou conflitos.

| Assunto | Documento principal |
|---|---|
| Visão geral e loop | `GDD - Geral.md` |
| Pets, dano, economia e taxas | `GDD - ProgBalPets.md` |
| Inventário, equipe e fusão | `GDD - SistemaInventarioEquipamentoFusao.md` |
| Gacha | `GDD - SistemaGacha.md` |
| Mapa, inimigos, Bosses e barreiras | `GDD - MapaInimigosBoss.md` |
| HUD, menus, tutorial e save | `GDD - TutorialHudMenusSalvamento.md` |
| Visual das interfaces | `GUIA_INTERFACE.md` |
| Estado detalhado e pendências | `ROADMAP.md` e `../../IMPLEMENTATION_PLAN.md` |
| Arte 3D | `Art/AGENTS.md`, `Art/VISUAL_IDENTITY.md`, `Art/CHARACTER_MODELING_RULES.md` e `Art/PET_CATALOG.md` |

O `ROADMAP.md` contém itens gerais antigos ainda desmarcados ao lado de implementações posteriores; leia os detalhes e confirme no projeto antes de concluir que uma funcionalidade não existe.

## Próxima etapa sugerida

Validar manualmente os inimigos comuns e Bosses nas quatro regiões, incluindo aparência, seleção, combate e desempenho.

Ao concluir uma etapa relevante, atualizar este resumo de forma concisa junto do `ROADMAP.md`, registrando o que foi confirmado e o que ainda precisa de validação.
