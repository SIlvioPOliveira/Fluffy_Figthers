# Fluffy Fighters — plano de implementação

Acompanhe o progresso detalhado em [Assets/Docs/ROADMAP.md](Assets/Docs/ROADMAP.md).

## Referências e estado inicial

Analisados `Assets/Docs/AGENTS.md`, `EstruturaPastas.md` e os GDDs Geral v0.3, Gacha v0.4, Progressão v0.5, Inventário v0.6, Mapa v0.7 e Tutorial v0.8. As referências a `Docs/GDD.md` e `Docs/ROADMAP.md` no AGENTS não correspondem a arquivos existentes. O projeto Unity conectado está em `C:/Users/sotav/Documents/ChatGPT/Fluffy Figthers`, Unity 6000.3.11f1.

Fase 1: cena `Assets/Scenes/Phase1Test.unity` criada; objetos e referências verificados pelo MCP. O Play Mode iniciou sem erros no Console e o personagem ficou apoiado no chão. O usuário informou que testou e realizou o commit.

Fase 2: catálogo inicial de 20 Pets e quatro regiões, validação e carteira de moedas implementados. Dados são editáveis em `Assets/Data`; IDs de Pets devem permanecer estáveis após a criação de salvamentos. Ícones e prefabs estão sem referências até a produção dos assets visuais. A carteira ainda não está ligada a uma cena nem possui persistência; essas integrações pertencem às fases seguintes.

Fase 3 iniciada: lógica em memória de inventário, três slots, dano e fusão implementada e verificada pelo MCP. Interface, integração ao jogo e persistência permanecem pendentes.

Representação provisória de Pets adicionada na cena separada `Assets/Scenes/PetEquipmentTest.unity`: teclas 1–3 alternam os slots. Verificação pelo MCP incluiu criação e formação por atualização forçada; o acompanhamento em movimento requer teste manual. A interface final e integração ao jogo continuam pendentes.

O usuário testou o acompanhamento e relatou que funciona bem. A cena de teste agora inclui inventário provisório aberto com I, dados de teste, confirmação de fusão, personagem azul, Pets laranja e chão quadriculado. Falta validação manual da interface; a versão final em Canvas e a persistência continuam pendentes.

Fase 4 iniciada: pools e lógica de compra em memória criados; `Assets/Scenes/GachaTest.unity` contém máquina provisória da Água, interface simples, 100 moedas e inventário vazio. O MCP verificou abertura modal, compra, recebimento e saldo insuficiente; o usuário relatou resultado satisfatório no teste da máquina inicial.

A máquina provisória agora apresenta uma roleta de 4,5 segundos com desaceleração e cartões de outros Pets em ambos os lados do prêmio, conforme feedback. O prêmio é entregue antes da animação e permanece no inventário ao fechar a janela; a lógica foi verificada pelo MCP. O ritmo e o enquadramento ainda precisam de teste manual. Ícones, áudio, interface final e persistência continuam pendentes.

Fase 5 iniciada: `Assets/Scenes/CombatTest.unity` acrescenta dois inimigos provisórios fracos ao teste de Gacha, seleção por clique/Q e ataques dos Pets. O MCP verificou troca de alvo, dano, intervalo, recompensa única e respawn com atualização forçada. Seleção por mouse e o ciclo completo ainda precisam de teste manual; animações e feedback visual completo permanecem pendentes.

Animação genérica provisória de ataque adicionada ao modelo dos Pets: recuo, investida, pequeno pulo e retorno em 0,4 s, com dano no impacto e intervalo individual de 1 s. O MCP verificou impacto único, temporizadores e cancelamento ao trocar de alvo; a compatibilidade com modelos definitivos aguarda validação.

O usuário aprovou o ritmo da animação provisória. `CombatTest` também possui brilho breve no inimigo e números de dano flutuantes, com cor por raridade; o valor exibido considera a vida restante no golpe final. Os eventos foram verificados pelo MCP. Os modelos definitivos permanecem pendentes.

O usuário aprovou o brilho e os números de dano. Uma notificação provisória de moedas foi adicionada à derrota dos inimigos de `CombatTest`; o MCP verificou que ela aparece uma vez por recompensa recebida, inclusive para inimigos diferentes. A legibilidade e o ritmo da notificação aguardam teste manual.

## Dependências e decisões a confirmar

- O GDD geral apresenta probabilidades que somam 101,1%. O usuário confirmou a distribuição dos GDDs v0.4 e v0.5: 68,9%, 25%, 5%, 1% e 0,1%, totalizando 100%.
- O resumo do loop sugere derrotar Bosses para desbloquear regiões, mas o GDD de mapa v0.7 diz explicitamente que a compra da barreira não exige vitória. O tutorial pede vencer o primeiro Boss antes da barreira; tratar a sequência do tutorial separadamente da condição de compra e confirmar a intenção.
- O tutorial diz permitir exploração livre, mas é sequencial; ações realizadas antes da etapa correspondente precisam de uma regra de reconhecimento retroativo para evitar bloqueio. Definir isso antes do tutorial.
- Arredondamento de metades para cima é explícito no inventário v0.6; usar uma única regra compartilhada para dano, em vez de depender de arredondamentos padrão divergentes.
- Saldo, concessão de Pet, fusão e compra de barreira exigem operações atômicas e salvamento consistente. Definir eventos e persistência antes da integração; não salvar referências a GameObjects.
- Câmera com botão direito preserva o cursor para seleção; interfaces modais bloqueiam movimento, câmera e seleção, mas só a pausa interrompe o tempo. A Fase 1 não implementa essas interfaces.
- Os valores de combate, distâncias e economia são iniciais e configuráveis; validar tempos e progressão em playtest antes de ajustar. Os quatro biomas são Água, Fogo, Lava e Gelo, apesar da abreviação ambígua na visão geral.

## Fases e critérios de aceitação

1. **Base jogável:** estrutura de pastas prevista, cena de teste independente com chão, luz, cápsula provisória, movimento WASD relativo à câmera e câmera em terceira pessoa controlada com botão direito do mouse. Aceite: cena abre e salva; diagonais não são mais rápidas; personagem permanece sobre o chão; câmera acompanha e gira apenas com botão direito; nenhuma falha de compilação ou erro no Console durante o teste. Validar sensação de controle manualmente.
2. **Dados e economia:** IDs estáveis, catálogo de Pets e dados configuráveis de regiões, raridades e moedas. Aceite: 20 tipos podem ser definidos, saldos não ficam negativos e dados inválidos são identificados.
3. **Inventário, equipamento e fusão:** quantidades agrupadas por ID e nível, três slots e fusão de três cópias disponíveis. Aceite: duplicatas equipáveis, limites respeitados, fusões atômicas e arredondamento consistente.
4. **Gacha e interação:** máquina por região, preço e pesos configuráveis, sorteio e entrega atômicos. Aceite: pesos válidos totalizam 100%, saldo insuficiente impede compra e cada roll entrega exatamente um Pet.
5. **Seleção e combate básico:** alvo por clique, cancelamento Q, limite de 20 m, vida e recompensa única, pets FOLLOW/CHASE/ATTACK com temporizadores independentes. Aceite: troca e invalidação de alvo corretas; dano e recompensa aplicados uma única vez por evento; inimigos comuns reaparecem após cinco segundos.
6. **Boss e jogador:** três pontos de vida, área sinalizada, dano e retorno à entrada, reinício e respawn do Boss. Aceite: telegráfico fixo por 1,5 s, raio configurável, intervalo de 3 s, reset ao morrer ou sair da arena e recompensa única.
7. **Mundo e progressão:** mapa contínuo com quatro regiões, seis inimigos comuns por região, máquinas e três barreiras. Aceite: compras descontam o valor correto, abrem o caminho e permitem retorno; Boss não é pré-requisito de compra conforme v0.7, pendente de confirmação.
8. **Interface e tutorial:** HUD, inventário, Gacha, pausa, objetivos e feedback de combate. Aceite: modais bloqueiam controles apropriados, pausa suspende tempo e objetivos avançam sem travar após ações antecipadas, conforme decisão de design.
9. **Persistência e integração:** JSON versionado com substituição segura e backup, inicialização e salvamentos nos eventos definidos. Aceite: reiniciar restaura saldo, Pets, slots, regiões, tutorial e conclusão; corrupção e operações incompletas não geram perda ou duplicação silenciosa.
10. **Polimento e validação:** arte e áudio provisórios substituídos gradualmente, testes de progressão, UX e desempenho em Windows. Aceite: ciclo completo do primeiro roll à vitória final, progressão com Pets comuns e erros de Console resolvidos.

As fases 1 e 2 são a base atual. As fases seguintes permanecem como planejamento.
