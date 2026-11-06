# FLUFFY FIGHTERS
## Game Design Document — Versão 0.7
### Design de Mapa, Regiões, Inimigos e Bosses

**Engine:** Unity 3D  
**Gênero:** Gacha / Pet Collection Simulator  
**Estilo:** Low Poly / Cartoon / Kawaii  
**Modo:** Single-player  
**Status:** Especificação para implementação do MVP

---

## 1. Objetivo

O mundo de Fluffy Fighters será composto por quatro regiões temáticas conectadas em um único mapa contínuo.

Cada região apresenta inimigos, uma máquina de Gacha, uma arena de Boss e um caminho para a região seguinte.

A progressão será controlada por barreiras que podem ser removidas utilizando moedas adquiridas durante os combates.

O design deve priorizar simplicidade de navegação, clareza visual e reutilização de sistemas na Unity.

## 2. Estrutura geral

O mapa será organizado linearmente:

**Água → Fogo → Lava → Gelo**

O jogador começa na região da Água.

As demais regiões permanecem inacessíveis até a compra das respectivas barreiras.

Após um desbloqueio, o jogador poderá transitar livremente entre as regiões liberadas.

O MVP utilizará uma única cena principal da Unity com as quatro regiões.

### Estrutura de cada região

- Zona de entrada segura.
- Máquina de Gacha.
- Área de inimigos comuns.
- Arena de Boss.
- Caminho de progressão.
- Barreira de acesso à região seguinte, exceto em Gelo.
- Objetos decorativos temáticos.

Os espaços de combate serão abertos, evitando navegação complexa dos Pets.

## 3. Dimensões sugeridas

As seguintes dimensões são valores iniciais, sujeitos a testes na Unity:

| Elemento | Dimensão |
|---|---|
| Área principal de cada região | Aproximadamente 55 × 55 metros |
| Arena de Boss | 18 metros de diâmetro |
| Caminho entre regiões | 8 metros de largura |
| Zona segura | Aproximadamente 12 × 12 metros |
| Espaçamento entre inimigos | 4 a 6 metros |
| Alcance máximo de seleção | 20 metros |

As áreas poderão ser ajustadas para acomodar modelos, câmera, decoração e movimentação.

O jogador deverá conseguir identificar visualmente onde estão o Gacha, os inimigos, o Boss e a saída.

## 4. Região da Água

A primeira região apresenta o tutorial e os sistemas fundamentais do jogo.

### Direção de arte

Ambiente aquático estilizado, com tons de azul, turquesa e verde-claro.

Elementos decorativos:
- Corais.
- Pedras arredondadas.
- Pequenas poças.
- Plantas aquáticas.
- Ilhas e plataformas de terreno simples.

### Conteúdo

- 1 máquina de Gacha.
- 4 inimigos fracos.
- 2 inimigos fortes.
- 1 Boss.
- 1 barreira para Fogo.
- Ponto inicial do jogador.

### Balanceamento

| Elemento | Vida | Recompensa |
|---|---:|---:|
| Inimigo fraco | 50 HP | 20 moedas |
| Inimigo forte | 120 HP | 50 moedas |
| Guardião das Marés | 350 HP | 200 moedas |

Gacha: 100 moedas por Roll.

Desbloqueio de Fogo: 400 moedas.

## 5. Região do Fogo

A segunda região introduz inimigos mais resistentes e Pets mais poderosos.

### Direção de arte

Ambiente quente e colorido, com tons de laranja, vermelho e amarelo.

Elementos decorativos:
- Árvores estilizadas com folhas alaranjadas.
- Pequenas chamas decorativas.
- Pedras quentes.
- Cristais luminosos.

### Conteúdo

- 1 máquina de Gacha.
- 4 inimigos fracos.
- 2 inimigos fortes.
- 1 Boss.
- 1 barreira para Lava.

### Balanceamento

| Elemento | Vida | Recompensa |
|---|---:|---:|
| Inimigo fraco | 250 HP | 80 moedas |
| Inimigo forte | 550 HP | 150 moedas |
| Guardião das Chamas | 1.000 HP | 600 moedas |

Gacha: 300 moedas por Roll.

Desbloqueio de Lava: 1.200 moedas.

## 6. Região da Lava

A terceira região possui temática vulcânica e apresenta inimigos com quantidades maiores de vida.

### Direção de arte

Ambiente rochoso, com tons de cinza-escuro, laranja e vermelho.

Elementos decorativos:
- Rochas vulcânicas.
- Rios de lava decorativos.
- Pequenas crateras.
- Cristais de magma.

Para o MVP, a lava será principalmente decorativa e não causará dano ao jogador.

### Conteúdo

- 1 máquina de Gacha.
- 4 inimigos fracos.
- 2 inimigos fortes.
- 1 Boss.
- 1 barreira para Gelo.

### Balanceamento

| Elemento | Vida | Recompensa |
|---|---:|---:|
| Inimigo fraco | 900 HP | 250 moedas |
| Inimigo forte | 1.800 HP | 450 moedas |
| Colosso Vulcânico | 3.600 HP | 1.600 moedas |

Gacha: 900 moedas por Roll.

Desbloqueio de Gelo: 4.000 moedas.

## 7. Região do Gelo

A quarta e última região representa o desafio final do protótipo.

### Direção de arte

Ambiente nevado, com tons de branco, azul-claro e lilás.

Elementos decorativos:
- Árvores congeladas.
- Rochas cobertas de neve.
- Cristais de gelo.
- Pequenas montanhas ao fundo.

### Conteúdo

- 1 máquina de Gacha.
- 4 inimigos fracos.
- 2 inimigos fortes.
- 1 Boss final.

### Balanceamento

| Elemento | Vida | Recompensa |
|---|---:|---:|
| Inimigo fraco | 2.600 HP | 750 moedas |
| Inimigo forte | 5.500 HP | 1.300 moedas |
| Guardião Glacial | 11.000 HP | 4.500 moedas |

Gacha: 2.500 moedas por Roll.

Ao derrotar o Guardião Glacial pela primeira vez, o jogador recebe uma mensagem de conclusão do protótipo.

O jogo continua normalmente depois da vitória.

## 8. Distribuição dos inimigos

Cada região terá seis inimigos comuns ativos:

- Quatro inimigos fracos.
- Dois inimigos fortes.

Os inimigos permanecerão em posições predefinidas.

Não serão implementadas patrulhas nem perseguição nos inimigos comuns.

### Regras de respawn

Quando um inimigo morre:

1. Sua vida chega a zero.
2. Sua animação de derrota é executada.
3. O jogador recebe as moedas.
4. O inimigo desaparece.
5. Após cinco segundos, retorna à sua posição original com a vida completa.

Os inimigos devem utilizar Prefabs reutilizáveis, com atributos ajustáveis pelo Inspector.

## 9. Arenas de Boss

Cada região possui uma arena circular de aproximadamente 18 metros de diâmetro.

O Boss fica próximo ao centro.

A arena deve possuir espaço suficiente para que o jogador consiga desviar dos ataques.

### Funcionamento

O combate começa quando o jogador seleciona o Boss e seus Pets iniciam o ataque.

Os Pets utilizam o mesmo sistema de combate por clique dos inimigos comuns.

O Boss não se movimenta pelo cenário durante o MVP.

### Ataque do Boss

O Boss utiliza um ataque circular de área:

1. Escolhe a posição atual do jogador.
2. Exibe um círculo vermelho no chão.
3. Aguarda 1,5 segundo.
4. Executa o ataque na área marcada.
5. Se o jogador estiver na área no momento do impacto, perde um ponto de vida.
6. Aguarda três segundos após o impacto e prepara outro ataque.

O raio inicial da área de ataque será de 2,5 metros.

Todos esses parâmetros devem ser configuráveis pelo Inspector.

### Vida do jogador

O personagem possui três pontos de vida.

Ao perder toda a vida, reaparece na entrada da região atual.

Não perde moedas nem Pets.

### Reinício de combate

O Boss recupera a vida máxima quando:

- O jogador morre.
- O jogador abandona a arena durante o combate.

A arena terá um Collider do tipo Trigger para monitorar a entrada e saída do personagem.

### Respawn

Após ser derrotado, o Boss desaparece e reaparece após 30 segundos.

A recompensa é concedida apenas uma vez por derrota.

## 10. Barreiras de progressão

O mapa possui três barreiras pagas.

| Barreira | Custo |
|---|---:|
| Água → Fogo | 400 moedas |
| Fogo → Lava | 1.200 moedas |
| Lava → Gelo | 4.000 moedas |

### Funcionamento

1. O jogador se aproxima da barreira.
2. Pressiona E.
3. A interface apresenta o nome da próxima região e o preço.
4. O jogador escolhe Confirmar ou Cancelar.
5. O sistema verifica se existem moedas suficientes.
6. O saldo é descontado.
7. A passagem é desbloqueada.
8. O bloqueio físico é desativado.
9. O desbloqueio é salvo.

Se o jogador não possuir moedas suficientes, a compra não acontece.

Barreiras desbloqueadas permanecem abertas permanentemente.

Não será obrigatório derrotar o Boss para comprar a passagem.

## 11. Tutorial e navegação

O jogador deve encontrar facilmente os objetivos iniciais.

A região da Água apresentará indicadores visuais para:

1. Máquina de Gacha.
2. Inventário.
3. Primeiro inimigo.
4. Boss.
5. Barreira de progressão.

Os indicadores deverão destacar o próximo objetivo do tutorial.

Após o tutorial, o jogador poderá explorar livremente as regiões desbloqueadas.

## 12. Estrutura técnica da Unity

### WorldManager
Mantém referências às regiões e gerencia eventos gerais do mundo.

### RegionData
ScriptableObject com nome, identidade visual, custo do Gacha e referências aos dados de balanceamento da região.

### RegionController
Controla os elementos locais da região, incluindo pontos de entrada e referência à arena do Boss.

### EnemySpawner
Gerencia a criação e o respawn dos inimigos comuns em posições predefinidas.

### EnemyHealth
Controla vida, recebimento de dano e morte, reutilizando o sistema definido no GDD v0.3.

### BossController
Controla combate, ataque de área, morte e reinício do Boss.

### BossArena
Detecta a presença do jogador e os limites da arena.

### ZoneUnlock
Verifica o custo da barreira, solicita o desconto de moedas e controla seu estado aberto ou fechado.

### InteractionSystem
Permite que o jogador interaja com Gachas e barreiras utilizando a tecla E.

### SaveSystem
Armazena os identificadores das regiões desbloqueadas e restaura o estado das barreiras ao carregar o jogo.

## 13. Regras de otimização

Para reduzir o tempo de desenvolvimento e melhorar o desempenho:

- Utilizar uma única cena principal.
- Reaproveitar Prefabs de inimigos.
- Compartilhar o mesmo BossController entre os quatro Bosses.
- Utilizar materiais e objetos Low Poly.
- Evitar terrenos complexos e grandes quantidades de partículas.
- Priorizar cenários com caminhos abertos.
- Não implementar física para rios ou lava.
- Não utilizar inimigos com IA de patrulha.
- Evitar carregamento complexo entre regiões.
- Permitir editar números de balanceamento pelo Inspector.

## 14. Critérios de aceitação

O mapa estará funcional quando:

1. O jogador conseguir navegar pela região da Água.
2. Encontrar e utilizar a máquina de Gacha.
3. Selecionar e derrotar inimigos comuns.
4. Encontrar e enfrentar o Boss.
5. Receber recompensas ao derrotar inimigos.
6. Interagir com a barreira de progressão.
7. Comprar o desbloqueio usando moedas.
8. Atravessar a barreira desbloqueada.
9. Encontrar os inimigos e o Gacha da nova região.
10. Retornar livremente às regiões anteriores.
11. Os quatro Bosses funcionarem corretamente.
12. As barreiras continuarem desbloqueadas após reiniciar o jogo.
13. A vitória contra o Boss final exibir a conclusão do protótipo.

---

**Fim do GDD v0.7.**