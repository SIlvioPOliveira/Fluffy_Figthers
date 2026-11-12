# FLUFFY FIGHTERS

## Game Design Document — Versão 0.3

**Engine:** Unity 3D\
**Gênero:** Gacha / Pet Collection Simulator\
**Modo de jogo:** Single-player\
**Estilo visual:** Low Poly / Cartoon / Kawaii\
**Referência:** Anime Fighters Simulator (Roblox)\
**Status:** Mecânicas de combate definidas para o MVP

---

# 1. Visão geral

Fluffy Fighters é um jogo 3D de coleção de criaturas fofinhas, chamadas Pets.

O jogador obtém Pets por meio de um sistema Gacha, equipa uma equipe de até três criaturas e utiliza essa equipe para derrotar inimigos, ganhar moedas e desbloquear novas regiões.

O personagem não realiza ataques diretamente. Sua principal função é explorar o mapa, selecionar alvos e desviar de ataques durante batalhas contra Bosses.

O jogo possui quatro regiões conectadas: Água, Fogo, Lava/Gelo.

### Core Loop

Selecionar inimigo → Pets atacam → Ganhar moedas → Fazer Gacha → Equipar ou fundir Pets → Aumentar o dano → Derrotar Bosses → Desbloquear novas regiões.

---

# 2. Sistema de seleção de alvos

## 2.1. Seleção por clique

O jogador utiliza o botão esquerdo do mouse para selecionar inimigos.

Quando um inimigo válido é selecionado:

1. O sistema identifica o inimigo clicado.
2. O inimigo recebe uma indicação visual de seleção.
3. A referência do inimigo é registrada como alvo ativo.
4. Todos os Pets equipados recebem o mesmo alvo.
5. Os Pets começam a se mover em direção ao inimigo.
6. Ao alcançar a distância de ataque, iniciam os golpes automáticos.

A seleção deve funcionar tanto para inimigos comuns quanto para Bosses.

## 2.2. Regras da seleção

- Apenas inimigos vivos podem ser selecionados.
- Somente um inimigo pode ser o alvo ativo por vez.
- Clicar em outro inimigo troca imediatamente o alvo da equipe.
- Clicar novamente no mesmo inimigo mantém a seleção.
- Clicar no chão não cancela a seleção atual.
- Pressionar Q cancela manualmente o alvo.
- Se o inimigo morrer ou desaparecer, a seleção é cancelada.
- Se o jogador se afastar demais do alvo, a seleção é cancelada.
- Clicar em botões da interface não deve selecionar inimigos acidentalmente.

## 2.3. Alcance de seleção

A distância máxima para selecionar e manter um inimigo como alvo será de **20 metros**, medida entre o jogador e o inimigo.

Se o jogador tentar selecionar um inimigo além dessa distância, o jogo exibirá brevemente a mensagem:

"Aproxime-se para atacar!"

Esse limite evita que os Pets atravessem grandes distâncias do mapa sem o jogador acompanhá-los.

## 2.4. Indicador visual de alvo

O inimigo selecionado apresentará:

- Um círculo ou marcador sob seus pés.
- Um pequeno ícone de alvo acima da cabeça.
- Barra de vida visível.

Quando outro inimigo for selecionado, os indicadores do alvo anterior desaparecerão.

Para o MVP, um círculo no chão e uma barra de vida são suficientes.

---

# 3. Inteligência artificial dos Pets

Cada Pet equipado possui uma inteligência artificial simplificada, com três estados principais.

## Estado FOLLOW

É o estado padrão do Pet.

O Pet acompanha o jogador pelo cenário, mantendo uma posição próxima a ele.

**Comportamentos:**

- Segue o personagem continuamente.
- Mantém uma distância visual confortável.
- Não causa dano.
- Não procura inimigos automaticamente.
- Aguarda receber um alvo válido.

## Estado CHASE

O Pet entra nesse estado quando o jogador seleciona um inimigo válido.

**Comportamentos:**

- Move-se até o inimigo selecionado.
- Procura uma posição disponível ao redor do alvo.
- Não causa dano enquanto estiver fora do alcance de ataque.
- Transita para ATTACK ao chegar à distância adequada.

Se o alvo se tornar inválido, o Pet retorna para FOLLOW.

## Estado ATTACK

O Pet inicia esse estado quando alcança seu alvo.

**Comportamentos:**

- Interrompe a movimentação de aproximação.
- Fica orientado visualmente para o inimigo.
- Reproduz uma animação de golpe.
- Aplica dano no momento de impacto.
- Aguarda seu intervalo de ataque.
- Repete os golpes enquanto o alvo estiver vivo e ao alcance.

Se o Pet ficar distante do inimigo, retorna ao estado CHASE.

Se perder o alvo, retorna ao estado FOLLOW.

---

# 4. Movimentação dos Pets

## 4.1. Seguindo o jogador

Os Pets equipados devem acompanhar o personagem de maneira suave, sem ficarem sobrepostos.

Para uma equipe com três Pets, serão utilizadas três posições relativas ao jogador:

- Pet 1: atrás e à esquerda.
- Pet 2: atrás e ao centro.
- Pet 3: atrás e à direita.

Essas posições se movimentam junto ao personagem.

O deslocamento dos Pets deve ser interpolado, evitando movimentos bruscos.

Uma pequena animação de pulo ou flutuação pode tornar a movimentação mais agradável sem exigir animações complexas.

## 4.2. Movimentação até inimigos

Quando um alvo é selecionado, cada Pet se desloca para uma posição de ataque ao redor dele.

As posições de ataque são distribuídas em ângulos diferentes, formando aproximadamente um semicírculo ao redor do inimigo.

Isso permite que os três Pets permaneçam visíveis durante o combate.

## 4.3. Distâncias

Valores iniciais propostos:

| Parâmetro                                   | Valor                    |
| ------------------------------------------- | ------------------------ |
| Velocidade do Pet                           | 6 metros/segundo         |
| Distância de ataque                         | 1,5 metro                |
| Distância máxima do alvo ao jogador         | 20 metros                |
| Distância de acompanhamento                 | Aproximadamente 2 metros |
| Distância para reposicionamento emergencial | 15 metros                |

As distâncias são parâmetros configuráveis na Unity.

## 4.4. Reposicionamento emergencial

Se um Pet ficar muito distante do jogador, sem um alvo válido, poderá ser reposicionado automaticamente na formação de acompanhamento.

Isso evita que Pets fiquem presos em objetos ou permaneçam em regiões anteriores.

## 4.5. Simplificações técnicas

Para o MVP:

- Pets não terão colisões físicas entre si.
- Pets não poderão empurrar o jogador.
- Pets não terão barra de vida.
- Pets não poderão morrer.
- Os cenários de combate deverão permanecer relativamente abertos.
- Não será necessário implementar desvio complexo de obstáculos.

---

# 5. Sistema de dano individual

## 5.1. Regra principal

**Cada Pet realiza um golpe a cada 1 segundo.**

Cada criatura possui seu próprio temporizador de ataque.

O sistema não deve aplicar dano continuamente a cada frame.

O dano é causado apenas quando um golpe é executado e validado.

## 5.2. Início do ataque

Quando o Pet alcança a distância de ataque:

1. Entra no estado ATTACK.
2. Inicia a animação de ataque.
3. Executa o primeiro golpe no momento de impacto da animação.
4. Aplica o dano ao inimigo.
5. Aguarda até completar um segundo desde o último golpe.
6. Inicia o próximo ataque.

Os intervalos são independentes para cada Pet.

## 5.3. Cálculo do dano

Cada Pet possui um atributo base chamado BaseDamage.

O dano final considera o nível de fusão.

**Fórmula:**

DanoFinal = Arredondar(BaseDamage × MultiplicadorDeFusão)

Exemplo:

Um Gatinho possui 10 de dano base.

- Nível 1: 10 de dano por golpe.
- Nível 2: 15 de dano por golpe.
- Nível 3: 23 de dano por golpe (arredondamento de 22,5).

Como cada Pet executa um golpe por segundo, seu valor numérico de DPS é equivalente ao dano causado por golpe em condições ideais.

## 5.4. Dano total da equipe

O dano dos Pets equipados é calculado individualmente.

Exemplo:

- Pet A: 8 de dano.
- Pet B: 15 de dano.
- Pet C: 25 de dano.

A equipe terá um DPS teórico de 48.

Entretanto, cada golpe acontece de forma independente, conforme o temporizador de cada Pet.

O inimigo recebe os danos na sequência em que os ataques acontecem.

## 5.5. Regras adicionais

- Os ataques não possuem chance de erro.
- Não existe dano crítico no MVP.
- Não existem resistências elementais.
- Não existem vantagens por tipo elemental.
- Todos os golpes causam dano físico simples.
- O dano nunca pode reduzir a vida do inimigo abaixo de zero.
- Ataques não podem continuar causando dano depois que o inimigo morrer.

## 5.6. Interrupção de ataques

Se o jogador selecionar outro inimigo, os Pets interrompem o alvo anterior e começam a perseguir o novo.

O tempo restante do intervalo entre ataques é mantido na troca de alvo, evitando golpes adicionais gratuitos ao clicar repetidamente em inimigos diferentes.

Quando um Pet retorna ao estado FOLLOW, seu intervalo pode ser reiniciado.

---

# 6. Animações dos Pets

Para reduzir o tempo de produção, todos os Pets poderão utilizar animações simples e semelhantes.

## 6.1. Animação Idle

Quando estão parados:

- Leve movimentação vertical.
- Pequena oscilação do corpo.
- Movimento suave e contínuo.

## 6.2. Animação Follow

Durante o acompanhamento:

- Pequenos pulos.
- Movimento de balanço.
- Durante o acompanhamento, o Pet olha na direção em que ele próprio está se deslocando; quando está parado, mantém sua última orientação. Ao selecionar um inimigo, continua orientando-se para o alvo.

## 6.3. Animação Attack

Cada golpe utiliza uma sequência visual curta:

1. O Pet se prepara para atacar.
2. Seu corpo recua levemente.
3. Avança rapidamente em direção ao inimigo.
4. O impacto é registrado.
5. O Pet retorna à posição original.

A duração total sugerida da animação é de aproximadamente 0,4 segundo.

O intervalo entre os impactos permanece em 1 segundo.

Os Pets não precisam possuir braços, armas ou animações de combate complexas.

O ataque pode ser representado por uma pequena investida ou pulo.

---

# 7. Feedback visual do combate

O combate precisa comunicar claramente ao jogador que seus Pets estão causando dano.

## 7.1. Números de dano

Sempre que um golpe acertar:

- Um número aparece próximo ao inimigo.
- O valor corresponde ao dano aplicado.
- O número sobe levemente.
- Desaparece gradualmente após aproximadamente 0,7 segundo.

Os números poderão herdar a cor da raridade do Pet atacante, facilitando a identificação dos diferentes golpes.

## 7.2. Efeito de impacto

Cada ataque gera um pequeno efeito visual no inimigo.

Para o MVP, um efeito genérico de partículas ou brilho será compartilhado entre todos os Pets.

Não será necessário criar efeitos exclusivos para cada criatura.

## 7.3. Feedback no inimigo

Ao receber um golpe, o inimigo pode apresentar:

- Um breve flash visual.
- Pequena animação de impacto.
- Redução na barra de vida.
- Número flutuante indicando o dano recebido.

O inimigo não será empurrado pelo golpe.

## 7.4. Barra de vida

Todos os inimigos possuem uma barra de vida acima da cabeça.

A barra apresenta:

- Vida atual.
- Vida máxima.
- Preenchimento proporcional à quantidade de HP.

A barra dos inimigos comuns não exibe o nome; os Bosses mantêm sua identificação. A redução visual da barra pode acontecer de forma suavizada.

Bosses terão uma barra maior na parte superior da interface.

## 7.5. Feedback de derrota

Quando um inimigo é derrotado:

1. Sua vida chega a zero.
2. A animação de derrota é executada.
3. O inimigo desaparece.
4. A recompensa é adicionada ao saldo do jogador.
5. Uma notificação de moedas aparece na tela.
6. Os Pets retornam ao estado FOLLOW.

A recompensa será concedida apenas uma vez por derrota, mesmo que vários golpes atinjam o inimigo quase simultaneamente.

---

# 8. Comportamento dos inimigos

## 8.1. Inimigos comuns

Os inimigos comuns permanecem parados e não atacam.

Cada região possui dois tipos:

- Fraco: menos vida e menor recompensa.
- Forte: mais vida e maior recompensa.

Todos utilizam o mesmo sistema de vida e recebimento de dano.

Após morrer, desaparecem e reaparecem depois de 5 segundos.

## 8.2. Bosses

Os Bosses podem receber golpes normalmente dos Pets.

Durante o combate, os Bosses executam ataques de área com dificuldade progressiva:

- **Água:** círculo na posição atual do jogador, com 3 segundos entre o impacto e a próxima preparação.
- **Fogo:** mesmo ataque, com intervalo de 2 segundos.
- **Lava:** intervalo de 2 segundos; a cada três ataques, o terceiro cria dois círculos simultâneos, um na posição atual e outro à frente do movimento observado ao iniciar a preparação.
- **Gelo:** utiliza os padrões de Lava e, periodicamente, uma sequência especial de áreas sinalizadas uma após a outra.

Cada área fica fixa e visível por 1,5 segundo antes do impacto; o raio inicial é de 2,5 metros. Estar em uma área no impacto causa um ponto de dano. Círculos simultâneos do mesmo golpe causam no máximo um ponto. A previsão não acompanha o jogador depois de ser marcada. Valores da sequência especial e demais parâmetros são ajustáveis e sujeitos a playtest; o GDD de mapa detalha a proposta inicial.

O jogador possui três pontos de vida.

Caso morra, reaparece na entrada da região sem perder moedas ou Pets.

O Boss restaura a vida ao reiniciar o combate ou quando o jogador abandona a arena.

A recompensa é concedida quando o Boss é derrotado, e ele reaparece após 30 segundos.

---

# 9. Sistema de Gacha

Cada região possui uma máquina de Gacha com Pets temáticos.

O jogador interage com a máquina utilizando E.

O sistema verifica o saldo, desconta moedas, sorteia um Pet conforme probabilidades predefinidas e adiciona o resultado ao inventário.

### Probabilidades

| Raridade  | Chance |
| --------- | -----: |
| Comum     |    70% |
| Raro      |    25% |
| Épico     |     5% |
| Lendario  |     1% |
| Secreto   |   0.1% |

Cada Roll entrega exatamente uma cópia de Pet.

Pets repetidos são permitidos.

O jogador inicia com 100 moedas para realizar o primeiro Roll.

Não haverá sistema de pity no MVP.

---

# 10. Inventário e equipamentos

O inventário exibe todos os Pets recebidos pelo jogador.

Cada entrada apresenta nome, ícone, raridade, dano, nível de fusão, quantidade e indicação de equipamento.

Os Pets são ordenados automaticamente pelo maior dano por golpe. Como o intervalo de ataque é fixo em um segundo, essa organização também corresponde à ordenação por DPS.

O jogador pode equipar até três Pets simultaneamente.

Cada cópia equipada ocupa uma vaga, sendo permitido equipar Pets repetidos.

---

# 11. Sistema de fusão

Três cópias idênticas, do mesmo nível de fusão, podem ser transformadas em uma cópia de nível superior.

| Nível | Multiplicador |
| ----- | ------------: |
| 1     |         1,00× |
| 2     |         1,50× |
| 3     |         2,25× |

O nível máximo é 3.

Pets equipados não podem ser consumidos na fusão.

A fusão não exige moedas nem materiais adicionais.

---

# 12. Regiões e progressão

O mapa é contínuo e dividido em quatro regiões:

Água → Fogo → Lava → Gelo.

Cada região possui dois tipos de inimigos comuns, um Boss, uma máquina de Gacha e um ambiente temático.

As três primeiras regiões possuem barreiras que podem ser removidas permanentemente mediante pagamento em moedas.

### Balanceamento inicial

| Região | Custo do Roll | HP Boss | Custo para desbloquear próxima região |
| ------ | ------------: | ------: | ------------------------------------: |
| Água   |           100 |     350 |                                   400 |
| Fogo   |           300 |   1.000 |                                 1.200 |
| Lava   |           900 |   3.600 |                                 4.000 |
| Gelo   |         2.500 |  11.000 |                                     — |

### Inimigos e recompensas

| Região | Inimigo fraco         | Inimigo forte           | Boss                     |
| ------ | --------------------- | ----------------------- | ------------------------ |
| Água   | 50 HP / 20 moedas     | 120 HP / 50 moedas      | 350 HP / 200 moedas      |
| Fogo   | 250 HP / 80 moedas    | 550 HP / 150 moedas     | 1.000 HP / 600 moedas    |
| Lava   | 900 HP / 250 moedas   | 1.800 HP / 450 moedas   | 3.600 HP / 1.600 moedas  |
| Gelo   | 2.600 HP / 750 moedas | 5.500 HP / 1.300 moedas | 11.000 HP / 4.500 moedas |

Os valores representam uma proposta inicial e devem ser ajustados considerando o DPS real da equipe e o tempo esperado para derrotar cada inimigo.\
\
Os valores devem ser alterados facilmente via inspector na Unity&#x20;

---

# 13. Controles e interface

Para permitir seleção de inimigos com o cursor, o controle da câmera será realizado mantendo pressionado o botão direito do mouse.

| Controle              | Função              |
| --------------------- | ------------------- |
| WASD                  | Movimentar jogador  |
| Botão direito + mouse | Rotacionar câmera   |
| Clique esquerdo       | Selecionar inimigo  |
| Q                     | Cancelar alvo       |
| E                     | Interagir           |
| I                     | Abrir inventário    |
| Esc                   | Fechar menu / pausa |

A HUD exibe moedas, equipe equipada, objetivo atual e vida do jogador.

O tutorial inicial orienta o jogador a realizar o primeiro Roll, equipar o Pet, selecionar um inimigo, ganhar moedas, enfrentar o Boss e desbloquear a próxima região.

---

# 14. Salvamento

O jogo salva automaticamente:

- Saldo de moedas.
- Pets obtidos.
- Quantidades e níveis de fusão.
- Pets equipados.
- Regiões desbloqueadas.
- Progresso do tutorial.

A vida atual dos inimigos, os temporizadores dos ataques e os alvos selecionados não precisam ser salvos.

---

# 15. Estrutura de implementação na Unity

### Componentes do combate

**CombatTargetManager**

- Detecta o clique em inimigos.
- Valida distância e disponibilidade do alvo.
- Mantém a referência do inimigo selecionado.
- Notifica a equipe quando o alvo muda.

**PetAI**

- Controla os estados FOLLOW, CHASE e ATTACK.
- Move o Pet entre posições.
- Controla o intervalo individual entre golpes.
- Executa animações e solicita aplicação de dano.

**PetData**

- Armazena nome, modelo, ícone, raridade e dano base do Pet.

**PlayerInventory**

- Gerencia posse, equipamentos e níveis de fusão.

**EnemyHealth**

- Armazena vida atual e máxima.
- Recebe dano.
- Controla morte e respawn.
- Dispara eventos de atualização de vida e derrota.

**CombatFeedback**

- Exibe números de dano e efeitos de impacto.

**BossController**

- Controla a arena, os ataques de área e o reinício do Boss.

### Recomendação técnica

Utilizar um único controlador de seleção para os três Pets, mas manter a movimentação e os temporizadores de ataque independentes em cada instância de PetAI.

A detecção de inimigos pode utilizar um Raycast da câmera.

O dano deve ser aplicado no evento de impacto da animação ou em um momento fixo configurado do ataque.

As informações compartilhadas dos Pets devem permanecer em ScriptableObjects.

---

# 16. Critérios de aceitação do combate

O sistema de combate será considerado funcional quando:

1. Clicar em um inimigo válido selecionar o alvo corretamente.
2. Todos os Pets equipados perseguirem o mesmo inimigo.
3. Cada Pet ocupar uma posição diferente ao redor do alvo.
4. Cada Pet aplicar um golpe a cada segundo.
5. O dano de cada golpe corresponder aos atributos do Pet.
6. Os golpes mostrarem animação e números de dano.
7. A barra de vida do inimigo diminuir corretamente.
8. Trocar de alvo interromper o ataque ao alvo anterior.
9. O sistema impedir dano em inimigos mortos.
10. As recompensas serem entregues apenas uma vez.
11. Os Pets retornarem ao jogador após a morte do alvo.
12. Os Pets interromperem a perseguição se o jogador ultrapassar o alcance máximo.
13. O mesmo sistema funcionar em inimigos comuns e Bosses.

---

# 17. Limitações de escopo

Não serão implementados no MVP:

- Combos.
- Ataques especiais.
- Dano crítico.
- Resistências elementais.
- Evoluções visuais complexas.
- Vida para Pets.
- Combate manual do personagem.
- Inteligência artificial avançada.
- Multiplayer.
- Sistema de pity.
- Microtransações.

A prioridade é entregar um loop completo de seleção, combate, recompensas, Gacha, inventário, fusão e progressão.

**Fim do GDD v0.3.**
