# Fluffy Fighters — GDD v0.4
## Sistema de Gacha

### Objetivo

O sistema de Gacha permite que o jogador gaste moedas obtidas em combate para receber Pets aleatórios, fortalecendo sua equipe e aumentando sua coleção.

### Funcionamento

1. O jogador se aproxima da máquina de Gacha da região.
2. Pressiona E para abrir a interface.
3. A interface apresenta os cinco Pets disponíveis, probabilidades, preço do Roll e saldo atual.
4. O jogador pressiona Roll ×1.
5. O sistema verifica se o jogador possui moedas suficientes.
6. O custo é descontado e um Pet é selecionado pelo algoritmo de probabilidades.
7. Uma animação de roleta horizontal apresenta o resultado.
8. O Pet é adicionado ao inventário independentemente de o jogador assistir à animação inteira.
9. O jogador pode realizar outro Roll ou fechar a interface.

### Raridades e probabilidades propostas

| Raridade | Probabilidade |
|---|---:|
| Comum | 68,9% |
| Raro | 25% |
| Épico | 5% |
| Lendário | 1% |
| Secreto | 0,1% |

Total: 100%.

As probabilidades deverão ser ajustáveis pelo Inspector da Unity. Não existirá sistema de pity no MVP.

### Catálogo

Cada região possui cinco Pets, um para cada raridade. O catálogo total previsto é de 20 Pets distribuídos entre Água, Fogo, Lava e Gelo.

Os nomes, modelos, atributos de dano e probabilidades devem ser definidos em dados configuráveis, evitando valores fixos no código.

### Interface

A interface do Gacha deverá apresentar:

- Nome da região.
- Saldo de moedas do jogador.
- Cinco Pets disponíveis com suas respectivas raridades e probabilidades.
- Preço de um Roll.
- Botão Roll ×1.
- Botão de fechar.
- Tela de resultado com nome, imagem e raridade do Pet sorteado.
- Botão para continuar.

### Animação

O sorteio utiliza uma roleta horizontal com ícones de Pets que se deslocam e desaceleram até parar na recompensa previamente selecionada.

Na apresentação provisória, os cartões mostram os modelos dos Pets de frente. Após a parada, o Pet sorteado é mostrado girando na tela de resultado, reutilizando a prévia 3D do inventário. O layout definitivo será definido na etapa de interfaces.

A versão cartoon integrada apresenta as cinco possibilidades e suas probabilidades, a roleta com cartões coloridos e o Pet sorteado girando. A identidade visual acompanha o inventário. A validação manual da nova apresentação ainda está pendente.

A animação terá duração aproximada de 2,2 segundos e utilizará cores e efeitos sonoros diferentes de acordo com a raridade obtida.

O protótipo atualmente usa 4,5 segundos configuráveis, conforme o ritmo de suspense anteriormente validado pelo usuário. Áudio por raridade permanece pendente.

O sorteio matemático ocorrerá antes da animação.

A conclusão da animação não altera o resultado sorteado.

### Regras obrigatórias

- Cada Roll entrega exatamente um Pet.
- Pets duplicados são permitidos.
- Não é possível comprar um Roll sem moedas suficientes.
- O jogador começa com 100 moedas para garantir a primeira tentativa.
- O botão Roll fica temporariamente desativado enquanto a animação estiver acontecendo.
- O jogador não pode executar dois Rolls simultaneamente.
- A recompensa deve ser registrada de forma segura, evitando duplicações ou perdas.
- O inventário aceita Pets repetidos para uso no sistema de fusão.
- O sistema deve funcionar independentemente da região.
- O custo do Roll é configurável para cada região.
- As probabilidades são independentes em cada tentativa.
- O inventário e as moedas devem ser persistidos no salvamento.

### Custos iniciais

| Região | Custo por Roll |
|---|---:|
| Água | 100 moedas |
| Fogo | 300 moedas |
| Lava | 900 moedas |
| Gelo | 2.500 moedas |

### Estrutura técnica na Unity

**PetData (ScriptableObject):** armazena identificador, nome, raridade, modelo 3D, ícone e dano base.

**GachaPool (ScriptableObject):** armazena a coleção de Pets da região, os respectivos pesos e o custo por Roll.

**GachaSystem:** verifica o custo, realiza o sorteio e envia a recompensa ao inventário.

**GachaMachine:** controla a interação do jogador com a máquina presente na cena.

**GachaUI:** apresenta as informações da máquina, executa a animação e mostra o resultado.

**PlayerInventory:** recebe o Pet sorteado e registra sua quantidade.

**CurrencyManager:** controla o saldo e o desconto de moedas.

Todos os valores de balanceamento deverão ser configuráveis pelo Inspector da Unity.

### Simplificações do MVP

Não serão implementados:

- Roll ×10.
- Auto-roll.
- Sistema de pity.
- Compra de probabilidades maiores.
- Sorteios com dinheiro real.
- Moedas premium.
- Troca de Pets entre jogadores.

O foco será entregar um único Roll visualmente satisfatório e completamente integrado à economia e ao inventário.

### Critérios de aceitação

O sistema estará completo quando o jogador conseguir abrir a máquina, consultar probabilidades, realizar um Roll, visualizar a animação, receber o Pet correto, encontrar o Pet no inventário e repetir a operação sem erros de saldo ou duplicação de recompensas.
