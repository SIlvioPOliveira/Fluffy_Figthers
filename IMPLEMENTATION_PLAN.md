# Fluffy Fighters — plano de implementação

Acompanhe o progresso detalhado em [Assets/Docs/ROADMAP.md](Assets/Docs/ROADMAP.md).

## Referências e estado inicial

Analisados `Assets/Docs/AGENTS.md`, `EstruturaPastas.md` e os GDDs Geral v0.3, Gacha v0.4, Progressão v0.5, Inventário v0.6, Mapa v0.7 e Tutorial v0.8. As referências a `Docs/GDD.md` e `Docs/ROADMAP.md` no AGENTS não correspondem a arquivos existentes. O projeto Unity conectado está em `C:/Users/sotav/Documents/ChatGPT/Fluffy Figthers`, Unity 6000.3.11f1.

Fase 1: cena `Assets/Scenes/Phase1Test.unity` criada; objetos e referências verificados pelo MCP. O Play Mode iniciou sem erros no Console e o personagem ficou apoiado no chão. O usuário informou que testou e realizou o commit.

Fase 2: catálogo inicial de 20 Pets e quatro regiões, validação e carteira de moedas implementados. Dados são editáveis em `Assets/Data`; IDs de Pets devem permanecer estáveis após a criação de salvamentos. Ícones e prefabs estão sem referências até a produção dos assets visuais. A carteira ainda não está ligada a uma cena nem possui persistência; essas integrações pertencem às fases seguintes.

Fase 3 iniciada: lógica em memória de inventário, três slots, dano e fusão implementada e verificada pelo MCP. Interface, integração ao jogo e persistência permanecem pendentes.

Representação provisória de Pets adicionada na cena separada `Assets/Scenes/PetEquipmentTest.unity`: teclas 1–3 alternam os slots. Verificação pelo MCP incluiu criação e formação por atualização forçada; o acompanhamento em movimento requer teste manual. A interface final e integração ao jogo continuam pendentes.

## Dependências e decisões a confirmar

- O GDD geral apresenta probabilidades de 70%, 25%, 5%, 1% e 0,1%, que somam 101,1%. Os GDDs específicos v0.4 e v0.5 definem 68,9%, 25%, 5%, 1% e 0,1%, totalizando 100%; esta é a distribuição proposta para implementação posterior, sujeita a confirmação.
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
