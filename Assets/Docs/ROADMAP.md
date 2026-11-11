# Fluffy Fighters — checklist do MVP

Atualizado em 06/11/2026. Baseado nos GDDs em `Assets/Docs` e no estado do projeto na branch `fase-2-dados-economia`. O [plano de implementação](../../IMPLEMENTATION_PLAN.md) contém a ordem técnica e os critérios gerais de cada fase.

**Legenda:** `[x]` concluído no escopo descrito; `[ ]` pendente. Quando algo estiver parcial, as partes prontas e pendentes aparecem em itens separados. Marcar um item somente após implementar e verificar o comportamento indicado. Os GDDs continuam sendo a referência de detalhes e regras.

## 1. Base do projeto e personagem

- [x] Criar estrutura básica de pastas e cena independente de teste.
- [x] Adicionar personagem provisório com `CharacterController`, chão e iluminação.
- [x] Implementar movimento WASD relativo à câmera e câmera de terceira pessoa girada com botão direito.
- [x] Verificar cena e componentes pelo MCP; iniciar Play Mode sem erros e confirmar apoio no chão.
- [x] Receber confirmação do usuário de que testou a Fase 1 e fez commit. O resultado detalhado de cada controle não foi registrado.
- [ ] Confirmar e registrar em teste manual específico WASD, diagonais, colisões e câmera em diferentes ângulos.
- [ ] Bloquear movimento e câmera enquanto interfaces modais estiverem abertas.
- [ ] Implementar três pontos de vida, dano e reaparecimento na entrada da região atual.
- [ ] Substituir o personagem provisório por visual e animações adequados.

## 2. Dados e economia

- [x] Criar dados editáveis de Pet com ID, nome, raridade, região, dano, ícone e prefab.
- [x] Cadastrar os 20 tipos de Pets e as quatro regiões, com danos, custos de Roll e barreiras iniciais dos GDDs.
- [x] Criar catálogo com busca e validação de referências, IDs duplicados e valores inválidos.
- [x] Criar carteira em memória com 100 moedas iniciais, notificação de alterações e proteção contra saldo negativo e overflow.
- [x] Validar pelo MCP as 20 combinações de região/raridade, busca, duplicidade e operações da carteira; Console sem erros após compilação.
- [ ] Associar ícones e prefabs aos 20 Pets quando disponíveis.
- [ ] Integrar carteira ao estado de jogo, recompensas, Gacha, barreiras, HUD e salvamento.
- [ ] Configurar e validar demais valores de balanceamento no Inspector, inclusive vida e recompensa de inimigos e Bosses, probabilidades e multiplicadores de fusão.
- [ ] Proteger a estabilidade dos IDs usados em salvamentos e tratar IDs ausentes no carregamento.

## 3. Inventário, equipamentos e fusão — próxima etapa

- [x] Armazenar Pets por ID e nível de fusão, agrupando quantidades e distinguindo cópias equipadas em memória.
- [x] Implementar três espaços de equipe; permitir cópias repetidas sem equipar mais cópias do que as possuídas.
- [x] Criar e remover representações provisórias dos Pets equipados e calcular formação para um a três Pets; verificar pelo MCP criação, colisores e posições com atualização forçada.
- [x] Validar manualmente o acompanhamento e os controles na cena `PetEquipmentTest` (usuário relatou funcionamento satisfatório).
- [ ] Integrar a representação ao jogo e testar os prefabs definitivos.
- [x] Calcular dano dos níveis 1–3 com multiplicadores 1,00 / 1,50 / 2,25 e arredondamento de 0,5 para cima; calcular DPS da equipe.
- [x] Fundir atomicamente três cópias disponíveis do mesmo ID e nível; impedir consumo de equipados e fusão acima do nível 3.
- [x] Criar interface provisória de equipe, lista rolável e detalhes, ordenada por dano, raridade, nome e nível, com equipamento e confirmação de fusão na cena de teste.
- [ ] Criar interface final em Canvas com grade, ícones e cores de raridade; adicionar animação e feedback visual de fusão.
- [x] Validar manualmente abertura, equipamento, remoção e fusão na interface provisória (usuário confirmou).
- [ ] Validar manualmente o bloqueio de controles e a aparência da interface provisória.
- [x] Verificar pelo MCP regras de slots, duplicatas, fusão, arredondamento, eventos e limites numéricos na lógica em memória.
- [ ] Testar a integração da interface, Pets visíveis e salvamento sem perda ou duplicação.

## 4. Gacha e interação

- [ ] Criar interação por E com máquina, interface modal e pools configuráveis por região.
- [x] Montar máquina provisória da Água em `GachaTest`, com interação por E, interface modal, saldo, custo e distribuição; usa inventário inicialmente vazio e 100 moedas.
- [x] Validar manualmente a máquina provisória da Água (usuário relatou resultado satisfatório).
- [ ] Apresentar saldo, custo, cinco Pets, raridades e probabilidades da máquina.
- [x] Implementar e testar em memória a validação de saldo e pesos, seleção de um Pet e compra com entrega consistente; pools das quatro regiões configurados.
- [ ] Impedir Rolls simultâneos; implementar roleta, resultado e feedback sem depender do fim da animação para conceder o Pet.
- [x] Criar roleta provisória de 4,5 s, com desaceleração e cartões em ambos os lados do prêmio; preservar prêmio ao fechar a janela e bloquear novo Roll até confirmar o resultado. Regras verificadas pelo MCP.
- [ ] Validar manualmente fluidez, enquadramento e resultado da roleta provisória; adicionar ícones, áudio e apresentação final quando disponíveis.
- [ ] Integrar entrega ao inventário e persistência; testar saldo insuficiente, duplicatas, interrupção e repetição.
- [x] Confirmar com o usuário a distribuição 68,9% / 25% / 5% / 1% / 0,1% dos GDDs mais recentes.

## 5. Seleção, Pets e combate

- [ ] Selecionar um inimigo vivo por clique esquerdo, ignorando cliques na interface; manter apenas um alvo.
- [x] Criar protótipo de seleção em `CombatTest`: clique, Q, limite de 20 m, alvo único e bloqueio enquanto os modais de teste estão abertos.
- [ ] Aplicar limite de 20 m, aviso fora de alcance, troca de alvo, cancelamento com Q e invalidação ao morrer, desaparecer ou afastar-se.
- [ ] Mostrar marcador de alvo e barra de vida; remover indicador anterior ao trocar ou cancelar.
- [x] Criar barra provisória sobre o inimigo selecionado, com nome, vida atual/máxima e preenchimento proporcional; verificar seleção, derrota e respawn pelo MCP.
- [x] Validar manualmente a barra provisória de vida (usuário aprovou).
- [ ] Integrar a apresentação final e a barra maior de Boss.
- [ ] Implementar formação de até três Pets, acompanhamento suave e reposicionamento emergencial.
- [ ] Implementar estados FOLLOW, CHASE e ATTACK e posições de ataque ao redor do alvo.
- [ ] Aplicar golpes independentes a cada segundo, no impacto da animação; manter intervalo ao trocar de alvo.
- [x] Implementar animação genérica provisória de recuo, investida, pulo e retorno para o modelo do Pet; aplicar dano no impacto e manter intervalos individuais, inclusive ao trocar de alvo.
- [x] Validar manualmente ritmo e aparência da animação provisória (usuário aprovou).
- [ ] Testar a animação com os modelos definitivos.
- [ ] Implementar vida, dano limitado a zero, morte, recompensa única e retorno dos Pets ao acompanhamento.
- [ ] Implementar inimigos comuns imóveis, dois tipos por região e respawn após cinco segundos.
- [x] Criar dois inimigos provisórios fracos com 50 HP, recompensa de 20 moedas e respawn; testar pelo MCP dano, intervalo, troca de alvo e recompensa única.
- [x] Testar manualmente o protótipo de combate na cena `CombatTest` (usuário relatou resultado satisfatório).
- [ ] Adicionar números de dano, impacto, barra de vida, feedback de derrota e recompensa de moedas.
- [x] Mostrar notificação provisória de moedas ao derrotar um inimigo, somente após a carteira receber a recompensa; verificar valor e unicidade pelo MCP.
- [x] Validar manualmente a notificação de moedas durante o combate (usuário aprovou).
- [x] Adicionar brilho provisório no inimigo e números flutuantes coloridos por raridade, mostrando o dano efetivamente aplicado; verificar golpes sucessivos e golpe final pelo MCP.
- [x] Validar manualmente o brilho e os números durante o combate (usuário aprovou).
- [ ] Testar troca e perda de alvo, temporizadores independentes, golpes simultâneos e recompensa única.

## 6. Bosses e sobrevivência

- [x] Criar `BossTest` com Guardião provisório de 350 HP, recompensa de 200 moedas, respawn de 30 s e arena circular de 30 m, ampliada após feedback.
- [x] Implementar protótipo de ataque sinalizado (1,5 s, raio 2,5 m), vida de três pontos e retorno à entrada após morte.
- [x] Verificar pelo MCP esquiva, acerto, fim de combate ao sair, reinício após morte, recompensa única, respawn e seleção através do trigger da arena.
- [ ] Validar manualmente a luta, a legibilidade da área de ataque e o ritmo do protótipo em `BossTest`.
- [x] Atualizar os GDDs com a progressão aprovada: Água básico, Fogo mais rápido, Lava com golpe duplo preditivo e Gelo com sequência especial.
- [x] Criar cenas provisórias independentes para Fogo, Lava e Gelo com dados dos Bosses e equipes de teste regionais; verificar padrões e cancelamento pelo MCP.
- [ ] Avaliar manualmente legibilidade, rotas de fuga e balanceamento dos quatro padrões; calibrar valores provisórios por playtest.
- [ ] Criar controlador reutilizável de Boss e arena com detecção de entrada e saída.
- [ ] Sinalizar área circular fixa sob o jogador por 1,5 s; aplicar dano de um coração no impacto e aguardar três segundos antes da próxima preparação.
- [ ] Configurar raio inicial de 2,5 m e demais valores pelo Inspector.
- [ ] Reiniciar vida do Boss quando o jogador morrer ou sair da arena; respawn do Boss em 30 s após derrota.
- [ ] Conceder recompensa uma vez por derrota e exibir barra de vida maior na HUD.
- [ ] Testar esquiva, dano, morte, retorno à entrada, abandono da arena e derrota simultânea.

## 7. Mundo e progressão

- [x] Montar Fogo, Lava e Gelo com chão, Gacha, seis inimigos e Boss por região, conectados à Água na mesma cena; usar materiais provisórios e números editáveis.
- [x] Implementar e verificar as três barreiras e seu salvamento, retomada em Fogo e registro da primeira vitória final; implementar retomada nas demais regiões.
- [ ] Validar manualmente os caminhos, colisões, Gachas, combate, Bosses, recompensas, barreiras, reaparecimento, vitória final e reinício nas três novas regiões.
- [x] Montar `RegiaoAguaPrototipo` com entrada, Gacha, quatro inimigos fracos, dois fortes, arena de 30 m, Guardião das Marés e saída provisória para Fogo, agrupados na Hierarchy.
- [x] Implementar barreira provisória da Água por 400 moedas, com confirmação e validação de saldo; verificar pelo MCP o ciclo de recompensas e compra.
- [ ] Validar manualmente navegação, legibilidade dos elementos, combate e compra da barreira na região provisória.
- [ ] Persistir o estado da barreira e conectar o caminho à região Fogo quando essas etapas forem implementadas.
- [ ] Construir uma cena principal contínua com Água, Fogo, Lava e Gelo, áreas abertas, pontos de entrada e arenas.
- [ ] Em cada região, colocar Gacha, quatro inimigos fracos, dois fortes e um Boss com valores configuráveis.
- [ ] Criar três barreiras com interação, confirmação, saldo suficiente e abertura permanente.
- [ ] Permitir retorno às regiões desbloqueadas e restaurar barreiras a partir do salvamento.
- [ ] Criar identidade visual e navegação legível para cada região, usando objetos provisórios até a arte final.
- [ ] Definir e validar a relação entre derrotar Bosses, tutorial e compra de barreiras.

## 8. HUD, menus e tutorial

- [x] Remover a identificação fixa da região da HUD para liberar espaço à barra do Boss, conforme pedido do usuário.
- [x] Criar pausa provisória na região Água com Esc, tempo congelado, bloqueio dos controles, retomada e salvamento antes de sair; verificar pelo MCP.
- [x] Usuário aprovou o menu de pausa em teste manual.
- [x] Criar menu inicial provisório com Jogar e Sair, e integrar o retorno pela pausa com salvamento; verificar navegação e preservação do progresso pelo MCP.
- [x] Usuário confirmou funcionamento do menu inicial.
- [ ] Validar manualmente os botões do menu inicial, saída e retorno em diferentes situações; aplicar a identidade visual definitiva futuramente.
- [x] Criar HUD provisória na região Água com moedas, região, vida, equipe, DPS e botão de inventário; verificar integração em Play Mode pelo MCP.
- [x] Receber aprovação visual da HUD provisória pelo usuário.
- [ ] Validar manualmente o botão da HUD provisória; substituir por Canvas responsivo e ícones finais.
- [x] Implementar primeiras quatro etapas do tutorial na região Água: máquina, Roll, inventário e equipamento, com progresso salvo e compatibilidade com dados anteriores.
- [x] Usuário aprovou a primeira parte do tutorial em partida de teste separada.
- [x] Implementar objetivos provisórios de primeiro inimigo e segundo Pet, reconhecendo ações antecipadas e salvando o progresso.
- [x] Validar manualmente as etapas de primeiro inimigo e segundo Pet (usuário confirmou).
- [x] Implementar os objetivos do Guardião e da barreira, com marcadores provisórios, conclusão salva e reconhecimento de ações antecipadas; verificar Boss, compra e gravação/leitura pelos testes do Unity.
- [x] Usuário aprovou o tutorial até Boss e barreira.
- [ ] Validar manualmente a retomada do tutorial concluído após fechar o jogo.
- [ ] Criar menu inicial com Jogar e Sair e inicialização de uma nova partida.
- [ ] Criar HUD de moedas, região, três corações, objetivo, equipe/DPS e acesso ao inventário.
- [ ] Criar sistema de modal único: bloquear movimento, câmera e seleção; inventário e Gacha mantêm mundo ativo.
- [ ] Criar menu de pausa em Esc com retomada, retorno ao menu e saída; pausar o tempo.
- [ ] Implementar oito objetivos sequenciais: máquina, primeiro Roll, inventário, equipar, inimigo, segundo Pet, Boss e barreira.
- [ ] Adicionar marcadores e tutorial contextual opcional de fusão; resolver ações concluídas antes da etapa correspondente.
- [ ] Exibir conclusão na primeira vitória contra o Boss final e permitir continuar jogando.

## 9. Salvamento

- [x] Implementar a primeira versão do slot local na cena `RegiaoAguaPrototipo`: moedas, Pets, três slots e abertura da barreira para Fogo, com validação, gravação automática e backup.
- [x] Executar `SaveSmokeTests` no Unity: gravação, restauração, backup, dados inválidos e configuração da cena.
- [x] Validar manualmente moedas e barreira após reiniciar o jogo (usuário confirmou).
- [ ] Validar manualmente após reiniciar: Pets, fusão e equipamentos; verificar a experiência de recuperação de backup.
- [ ] Criar um slot local JSON versionado em `Application.persistentDataPath`.
- [ ] Persistir moedas, IDs e quantidades de Pets por nível, três slots, regiões, tutorial e vitória final.
- [ ] Restaurar estado com validação de IDs, quantidades e equipamentos; iniciar com vida cheia na última região desbloqueada.
- [ ] Salvar após Gacha, equipamento, fusão, barreira, etapas do tutorial e vitória final; também periodicamente e ao sair normalmente.
- [ ] Evitar salvar operações incompletas; usar arquivo temporário, substituição segura e backup quando possível.
- [ ] Testar reinício, recuperação de backup, dados inválidos e ausência de duplicação ou perda.

## 10. Arte, áudio, balanceamento e entrega

- [x] Importar os 20 modelos de pets fornecidos, mantendo os FBX originais e texturas em 4K; criar prefabs e associar ao catálogo.
- [x] Verificar instanciação dos 20 pets, texturas e combate com três dragões; executar medição exploratória no Editor sem alterar o inventário salvo.
- [x] Afastar a câmera principal do protótipo para manter os pets equipados visíveis; validar enquadramento manualmente no jogo.
- [ ] Validar manualmente os 20 pets na cena `TesteModelosPets`, incluindo orientação, escala, acompanhamento, ataque e materiais; medir desempenho na build em gameplay real.

  Medição exploratória: Unity 6000.3.11f1, Editor em segundo plano, RTX 4060 Ti; três dragões equipados atacando um alvo isolado, 10 s de amostragem após aquecimento: 4.041 quadros, média 2,48 ms (~404 FPS), percentil 95 de 3,50 ms. O Editor informou ~1.174 MiB de memória total alocada e ~427 MiB somados para as 20 texturas 4K carregadas. Esses números incluem o Editor e não substituem a medição da build em gameplay real ou em outros PCs.
- [ ] Criar ou integrar modelos, ícones, prefabs e animações simples de idle, acompanhamento e ataque dos Pets.
- [ ] Criar visuais e animações de inimigos e Bosses, efeitos de combate, Gacha e fusão, interfaces e áudio.
- [ ] Validar acessibilidade visual e legibilidade de alvo, ataques, objetivos e interfaces.
- [ ] Ajustar economia, dano, HP e recompensas por playtest; verificar progressão com Pets comuns e tempos dos Bosses.
- [ ] Testar o ciclo completo: primeira partida → Roll → equipar → combate → Boss → quatro regiões → Boss final → continuar jogando.
- [ ] Fazer testes de regressão, compilação, Console, desempenho e build Windows; registrar problemas e limitações.

## Decisões de design pendentes

- **Probabilidades:** o usuário confirmou 68,9% / 25% / 5% / 1% / 0,1%, conforme os GDDs v0.4/v0.5. O GDD geral anterior soma 101,1% e deve ser corrigido ao revisar os documentos.
- **Boss e barreiras:** o resumo geral sugere derrotar Bosses para avançar; o GDD de mapa permite comprar barreiras sem derrotá-los. O tutorial pede o Boss antes da primeira barreira. Confirmar a regra global e como o tutorial reage a compras antecipadas.
- **Objetivos antecipados:** o tutorial é sequencial e permite exploração livre; definir como reconhecer eventos ocorridos antes da etapa correspondente para evitar bloqueios.

## Fora do escopo definido para o MVP

Multiplayer, contas, nuvem, múltiplos slots, pity, auto-roll, Roll ×10, moedas premium, compras com dinheiro real, troca/venda de Pets, fusão automática ou em massa, filtros avançados, missões secundárias, conquistas, localização e cinemáticas não integram esta checklist de entrega.
