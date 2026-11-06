# FLUFFY FIGHTERS
## GDD v0.5 — Progressão dos Pets e Balanceamento

**Status:** Valores iniciais definidos para playtest  
**Objetivo:** Estabelecer a progressão de poder dos Pets e a economia das quatro regiões.

### 1. Progressão entre regiões

Os Pets de regiões posteriores possuem dano base superior aos Pets equivalentes de regiões anteriores.

A raridade determina a força relativa dos Pets dentro de uma mesma região, mas não garante que um Pet de uma região anterior seja superior aos de regiões posteriores.

Um Pet Comum de Fogo, por exemplo, possui 20 pontos de dano, superando o Pet Épico de Água, que possui 12.

Pets de regiões anteriores continuam utilizáveis e podem receber melhorias por fusão.

### 2. Catálogo e dano base

| Região | Raridade | Nome | Dano base |
|---|---|---|---:|
| Água | Comum | Peixinho | 5 |
| Água | Raro | Tartaruga | 8 |
| Água | Épico | Polvinho | 12 |
| Água | Lendário | Golfinho | 18 |
| Água | Secreto | Dragão Marinho | 30 |
| Fogo | Comum | Gatinho de Fogo | 20 |
| Fogo | Raro | Raposa Flamejante | 30 |
| Fogo | Épico | Fênix Bebê | 45 |
| Fogo | Lendário | Leão Solar | 65 |
| Fogo | Secreto | Dragão de Fogo | 100 |
| Lava | Comum | Pedrinha Vulcânica | 70 |
| Lava | Raro | Caranguejo Magma | 105 |
| Lava | Épico | Salamandra | 160 |
| Lava | Lendário | Urso Vulcânico | 230 |
| Lava | Secreto | Dragão de Magma | 350 |
| Gelo | Comum | Pinguinzinho | 240 |
| Gelo | Raro | Raposa Polar | 360 |
| Gelo | Épico | Lobo de Gelo | 540 |
| Gelo | Lendário | Urso Glacial | 800 |
| Gelo | Secreto | Dragão de Cristal | 1.200 |

Os nomes são provisórios. O dano base, a raridade, os modelos e os ícones deverão ser configuráveis no Inspector da Unity por meio de ScriptableObjects.

### 3. Probabilidades do Gacha

A mesma distribuição será utilizada nas quatro regiões:

| Raridade | Chance |
|---|---:|
| Comum | 68,9% |
| Raro | 25% |
| Épico | 5% |
| Lendário | 1% |
| Secreto | 0,1% |

A distribuição totaliza 100%.

Cada região possui um Pet de cada raridade. O sorteio é independente e não possui sistema de pity.

### 4. Dano e fusão

Cada Pet realiza um golpe a cada segundo.

**DanoFinal = Arredondar(DanoBase × MultiplicadorDeFusão)**

Os multiplicadores são:

- Nível 1: 1,00×
- Nível 2: 1,50×
- Nível 3: 2,25×

O arredondamento deve seguir uma única regra consistente em todo o jogo, preferencialmente arredondamento de valores terminados em 0,5 para cima.

O dano total teórico da equipe é a soma dos danos finais dos três Pets equipados, considerando um golpe por segundo de cada um.

### 5. Vida e recompensa dos inimigos

| Região | Fraco HP / moedas | Forte HP / moedas | Boss HP / moedas |
|---|---|---|---|
| Água | 50 / 20 | 120 / 50 | 350 / 200 |
| Fogo | 250 / 80 | 550 / 150 | 1.000 / 600 |
| Lava | 900 / 250 | 1.800 / 450 | 3.600 / 1.600 |
| Gelo | 2.600 / 750 | 5.500 / 1.300 | 11.000 / 4.500 |

Os valores são provisórios e deverão ser ajustados após os testes de combate.

### 6. Custos do Gacha e desbloqueio

| Região | Roll | Próxima região |
|---|---:|---:|
| Água | 100 | 400 |
| Fogo | 300 | 1.200 |
| Lava | 900 | 4.000 |
| Gelo | 2.500 | — |

Todas as moedas são obtidas derrotando inimigos ou Bosses.

### 7. Objetivos de balanceamento

- O primeiro Pet deve permitir derrotar inimigos fracos sem dificuldade excessiva.
- Pets Comuns devem ser suficientes para avançar no jogo.
- Pets Raros e Épicos devem proporcionar melhorias perceptíveis.
- Lendários e Secretos devem acelerar bastante a progressão, sem serem obrigatórios.
- Uma equipe de três Pets Comuns da região deve conseguir derrotar seu Boss em aproximadamente 15 a 25 segundos de dano contínuo ideal.
- O jogador deve conseguir comprar novos Rolls sem longos períodos de repetição.
- O jogador deve continuar tendo motivos para utilizar o Gacha das regiões recém-desbloqueadas.

### 8. Regras de implementação

Os seguintes valores devem ser configuráveis pelo Inspector da Unity:

- Dano base de cada Pet.
- Raridade de cada Pet.
- Probabilidade de sorteio.
- Custo de cada Gacha.
- Vida dos inimigos.
- Recompensa dos inimigos.
- Vida dos Bosses.
- Recompensa dos Bosses.
- Custo das barreiras.
- Multiplicadores de fusão.

As regras matemáticas devem permanecer separadas dos efeitos visuais, animações e interfaces.

### 9. Critérios de aceitação

O balanceamento estará pronto para validação quando:

1. Todos os 20 Pets estiverem registrados nos dados do jogo.
2. Cada máquina utilizar somente os Pets de sua região.
3. As probabilidades somarem 100%.
4. Os ataques aplicarem o dano individual correto.
5. O dano das fusões for calculado corretamente.
6. A economia permitir a progressão utilizando Pets Comuns.
7. O desbloqueio das regiões ocorrer sem exigir Pets raros.
8. Todos os atributos puderem ser alterados sem modificar código.

**Fim do GDD v0.5.**