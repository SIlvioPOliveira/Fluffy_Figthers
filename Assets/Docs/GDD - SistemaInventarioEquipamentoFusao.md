# FLUFFY FIGHTERS
## Game Design Document — v0.6
### Sistema de Inventário, Equipamentos e Fusão

**Engine:** Unity 3D  
**Status:** Especificação do MVP  
**Dependências:** Gacha v0.4, balanceamento v0.5 e combate v0.3

---

## 1. Objetivo

O inventário é responsável por armazenar, organizar e gerenciar todos os Pets adquiridos pelo jogador.

O sistema deve permitir visualizar a coleção, equipar até três Pets, consultar seus atributos e fundir cópias repetidas para aumentar seu dano.

A interface deve ser visualmente atrativa, fácil de compreender e simples de implementar na Unity.

## 2. Estrutura de dados

Cada tipo de Pet possui dados estáticos definidos em um ScriptableObject chamado `PetData`:

- `petId`: identificador único e permanente.
- `petName`: nome.
- `rarity`: Comum, Raro, Épico, Lendário ou Secreto.
- `region`: Água, Fogo, Lava ou Gelo.
- `baseDamage`: dano base.
- `icon`: imagem utilizada na interface.
- `prefab`: modelo 3D utilizado quando equipado.

O inventário utiliza o identificador do Pet e o nível de fusão para organizar as cópias.

Uma entrada do inventário contém:

- Identificador do tipo de Pet.
- Nível de fusão.
- Quantidade total possuída.
- Quantidade equipada, derivada da lista de equipamentos.

As cópias com o mesmo identificador e nível de fusão são agrupadas.

**Exemplo:** cinco Peixinhos Lv. 1 e dois Peixinhos Lv. 2 aparecem como duas entradas diferentes.

## 3. Interface

A interface principal contém três seções:

### 3.1. Equipe equipada

Na parte superior, são apresentados três espaços correspondentes aos Pets equipados.

Cada espaço exibe o ícone, nome abreviado e DPS do Pet.

O dano total da equipe é apresentado acima dos espaços.

### 3.2. Grade de Pets

A parte central utiliza uma grade com cards organizados automaticamente.

Cada card apresenta:

- Ícone do Pet.
- Cor correspondente à raridade.
- Dano final.
- Nível de fusão.
- Quantidade total de cópias.
- Indicador de quantidade equipada.

A grade pode ser rolada quando houver muitos Pets.

Os Pets aparecem de frente nos cards. Ao selecionar um Pet, sua apresentação gira em 3D; ao desfazer a seleção, o card volta a exibir a vista frontal. A prévia usa os modelos do próprio jogo.

O inventário aceita os 20 tipos de Pets definidos no GDD v0.5, incluindo múltiplas cópias e diferentes níveis de fusão.

### 3.3. Painel de detalhes

Ao selecionar um card, é apresentado um painel contendo:

- Nome do Pet.
- Região de origem.
- Raridade.
- Nível de fusão.
- Dano base e dano final.
- Quantidade total.
- Quantidade disponível para fusão.
- Botão Equipar.
- Botão Desequipar.
- Botão Fundir, quando permitido.

Os botões são exibidos ou desativados de acordo com as condições atuais.

## 4. Ordenação

Os Pets são ordenados automaticamente por dano final, do maior para o menor.

Em caso de empate:

1. Maior raridade.
2. Ordem alfabética do nome.
3. Nível de fusão.

Não será necessário implementar filtros adicionais no MVP.

A interface deve atualizar a ordenação após obter um novo Pet ou realizar uma fusão.

## 5. Equipamentos

O jogador possui três espaços de equipamento.

Cada espaço guarda uma referência para um tipo de Pet e seu nível de fusão.

### Regras

- Cada espaço representa uma cópia equipada.
- É permitido equipar Pets repetidos.
- Não é possível equipar mais cópias do que o jogador possui.
- Equipar não consome nem remove cópias do inventário.
- Não é possível equipar um quarto Pet.
- Desequipar libera um espaço.
- O jogador pode ficar com zero, um, dois ou três Pets equipados.
- Os espaços de equipamento devem ser persistidos no salvamento.

Ao equipar um Pet, seu modelo 3D aparece no cenário e passa a acompanhar o personagem.

Ao desequipar, o modelo correspondente é removido.

Os demais Pets reorganizam sua formação automaticamente.

### Equipamento de cópias repetidas

Se o jogador possui três Peixinhos Lv. 1, poderá equipar os três simultaneamente.

Nesse caso, a interface exibirá quantidade total 3, quantidade equipada 3 e quantidade disponível para fusão 0.

## 6. Cálculo de dano

Cada Pet realiza um golpe a cada segundo.

O dano é determinado por:

`DanoFinal = Arredondar(BaseDamage × MultiplicadorDeFusão)`

Multiplicadores:

| Nível | Multiplicador |
|---|---:|
| 1 | 1,00× |
| 2 | 1,50× |
| 3 | 2,25× |

O arredondamento deve ser consistente, utilizando arredondamento de valores terminados em 0,5 para cima.

O DPS teórico da equipe é a soma dos danos finais dos Pets equipados.

A atualização dos equipamentos ou dos dados dos Pets deve recalcular o DPS exibido.

## 7. Sistema de fusão

A fusão combina três cópias idênticas e do mesmo nível para produzir uma cópia do nível seguinte.

### Regras

- São necessárias três cópias disponíveis.
- As cópias precisam possuir o mesmo `petId`.
- As três cópias precisam possuir o mesmo nível de fusão.
- Pets equipados não podem ser consumidos.
- A fusão não exige moedas.
- Não é possível fundir Pets de nível 3.
- O Pet resultante conserva nome, região, raridade, ícone e modelo do tipo original.
- Somente o nível de fusão e o dano final mudam.
- Não será implementada fusão automática no MVP.

### Exemplo

Inventário inicial:

8 Peixinhos Lv. 1, nenhum equipado.

Primeira fusão:

5 Peixinhos Lv. 1 + 1 Peixinho Lv. 2.

Segunda fusão:

2 Peixinhos Lv. 1 + 2 Peixinhos Lv. 2.

A fusão pode ser realizada novamente apenas se existirem pelo menos três cópias disponíveis de um mesmo tipo e nível.

### Processo técnico

1. O jogador seleciona um Pet.
2. Pressiona Fundir.
3. O sistema verifica a disponibilidade de três cópias.
4. Exibe uma confirmação com o resultado previsto.
5. O jogador confirma.
6. Três cópias são consumidas.
7. Uma cópia do nível seguinte é adicionada.
8. O inventário e o DPS são atualizados.
9. A animação de fusão é apresentada.
10. O progresso é salvo.

A alteração das quantidades deve ser concluída como uma única operação lógica, evitando perda ou duplicação de Pets.

## 8. Feedback visual

### Equipar

- O card apresenta um indicador de equipamento.
- O modelo 3D aparece próximo ao jogador.
- O contador de Pets equipados é atualizado.

### Desequipar

- O indicador é atualizado.
- O modelo 3D correspondente desaparece.
- A formação dos outros Pets é reorganizada.

### Fusão

- Três ícones convergem para o centro.
- Um flash aparece.
- O Pet evoluído é revelado.
- O novo dano é exibido.

A animação será compartilhada entre todos os Pets.

As cores podem variar conforme a raridade.

## 9. Integração com Gacha

Após cada Roll, o `GachaSystem` entrega um `PetData` ao `PlayerInventory`.

O inventário adiciona uma cópia de nível 1 do Pet recebido.

Se o jogador já possui cópias do mesmo Pet e nível, sua quantidade é incrementada.

Se for um Pet ainda não possuído, uma nova entrada é criada.

O jogador não precisa abrir o inventário para que a recompensa seja registrada.

## 10. Salvamento

O SaveSystem deve armazenar:

- Identificadores dos Pets.
- Níveis de fusão.
- Quantidades possuídas.
- Conteúdo dos três espaços de equipamento.

Ao carregar o jogo:

1. Os dados são restaurados.
2. Os identificadores são associados aos respectivos `PetData`.
3. O inventário é reconstruído.
4. Os Pets equipados são instanciados.
5. O DPS total é recalculado.
6. A interface é atualizada.

Referências diretas a GameObjects não devem ser utilizadas como dados persistentes.

## 11. Arquitetura na Unity

### PetData

ScriptableObject com os dados fixos de cada tipo de Pet.

### PlayerInventory

Gerencia quantidades, níveis de fusão, validação das operações e eventos de alteração.

### InventoryUI

Exibe a grade de Pets, detalhes e botões de interação.

### PetCardUI

Componente visual reutilizável para representar uma entrada do inventário.

### PetEquipmentManager

Controla os três espaços equipáveis e os modelos 3D dos Pets ativos.

### PetFusionSystem

Pode ser implementado como parte do PlayerInventory ou como um componente separado que solicita as operações ao inventário.

### SaveSystem

Serializa os dados necessários para restaurar inventário e equipamentos.

## 12. Critérios de aceitação

O sistema será considerado funcional quando:

1. Os Pets obtidos no Gacha aparecerem no inventário.
2. Cópias idênticas do mesmo nível forem agrupadas.
3. Pets de níveis diferentes permanecerem em entradas separadas.
4. A ordenação pelo maior DPS funcionar.
5. O jogador conseguir equipar até três Pets.
6. For possível equipar cópias repetidas.
7. Os modelos 3D aparecerem e desaparecerem corretamente.
8. A interface mostrar o DPS total da equipe.
9. Uma fusão consumir três cópias válidas.
10. Pets equipados não puderem ser consumidos.
11. O dano do Pet evoluído estiver correto.
12. As operações não provocarem quantidades negativas ou duplicações.
13. O inventário permanecer correto após fechar e reabrir o jogo.
14. Os cinco níveis de raridade visuais e os 20 tipos de Pets forem suportados.

## 13. Fora do escopo do MVP

- Arrastar e soltar.
- Venda de Pets.
- Exclusão de Pets.
- Fusão automática.
- Fusão em massa.
- Filtros avançados.
- Busca por texto.
- Bloqueio manual de Pets.
- Expansão paga de slots.
- Troca de Pets entre jogadores.

**Fim do GDD v0.6.**
