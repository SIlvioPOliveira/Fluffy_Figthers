# FLUFFY FIGHTERS
## Game Design Document — v0.8
### Tutorial, HUD, Menus e Salvamento

**Engine:** Unity 3D  
**Plataforma inicial:** PC / Windows  
**Modo:** Single-player  
**Status:** Especificação para implementação do MVP

---

## 1. Objetivo

Definir o fluxo inicial de aprendizagem do jogador, a interface principal, os menus e o sistema de persistência de progresso.

O jogo deverá apresentar suas funcionalidades de maneira gradual e intuitiva, sem depender de longas explicações textuais.

## 2. Inicialização do jogo

Ao iniciar, o jogo apresenta um menu com as opções:

- Jogar.
- Sair.

Ao selecionar Jogar, o sistema verifica se existe progresso salvo.

Se existir, carrega o estado do jogador.

Caso contrário, inicia uma nova partida com:

- 100 moedas.
- Inventário vazio.
- Nenhum Pet equipado.
- Apenas a região Água desbloqueada.
- Tutorial na primeira etapa.
- Personagem na posição inicial da região Água.

O jogador não receberá Pets automaticamente.

## 3. Sistema de tutorial

O tutorial será composto por objetivos sequenciais, exibidos na HUD.

### Etapa 1 — Primeiro Gacha

**Objetivo:** Vá até a máquina de Gacha.

Um marcador indica sua posição.

A etapa termina quando o jogador abre a interface da máquina.

### Etapa 2 — Primeiro Roll

**Objetivo:** Realize seu primeiro Roll.

O jogador começa com as 100 moedas necessárias.

A etapa termina quando uma recompensa é adicionada ao inventário.

### Etapa 3 — Inventário

**Objetivo:** Abra o inventário.

A interface indica a tecla I.

A etapa termina quando o jogador abre o inventário.

### Etapa 4 — Equipamento

**Objetivo:** Equipe seu primeiro Pet.

O jogador seleciona o Pet recebido e pressiona Equipar.

A etapa termina quando pelo menos um Pet estiver equipado.

### Etapa 5 — Primeiro combate

**Objetivo:** Derrote um inimigo.

O jogador deve clicar em um inimigo e permitir que seu Pet o ataque.

A etapa termina quando o jogador recebe a recompensa por derrotar o inimigo.

### Etapa 6 — Novo Roll

**Objetivo:** Consiga moedas e obtenha outro Pet.

O jogador derrota inimigos para acumular pelo menos 100 moedas e realiza outro Roll.

A etapa termina após a obtenção do segundo Pet.

### Etapa 7 — Boss

**Objetivo:** Derrote o Guardião das Marés.

O marcador do tutorial indica a arena.

O jogador precisa utilizar seus Pets para atacar enquanto desvia dos ataques do Boss.

A etapa termina quando o Boss é derrotado.

### Etapa 8 — Próxima região

**Objetivo:** Desbloqueie a região Fogo.

O jogador acumula 400 moedas e interage com a barreira.

Ao confirmar a compra, a passagem é aberta e o tutorial principal é concluído.

### Regras do tutorial

- Apenas um objetivo principal aparece por vez.
- O jogador pode explorar livremente.
- A conclusão dos objetivos deve ser detectada por eventos dos sistemas.
- O tutorial não deve exigir a obtenção de raridades específicas.
- Não deve exigir fusões para avançar.
- O jogador não perde o progresso do tutorial ao fechar o jogo.
- O tutorial não reinicia após ser concluído.

## 4. Tutorial contextual de fusão

Quando o jogador possuir três cópias não equipadas do mesmo Pet e nível, o inventário poderá destacar o botão Fundir.

A interface exibirá uma pequena mensagem explicando a mecânica.

Esse aviso não fará parte dos objetivos principais e poderá ser ignorado.

## 5. HUD principal

A HUD utilizará elementos responsivos. A versão integrada usa UI Toolkit da Unity para acompanhar o inventário e o Gacha; a distribuição dos elementos abaixo permanece a mesma.

### Canto superior esquerdo

Contador de moedas com ícone e valor atual.

### Parte superior central

Área reservada para informações de combate, incluindo a barra de vida do Boss. A identificação permanente da região foi removida da HUD a pedido do usuário para evitar sobreposição.

### Canto superior direito

Indicador de vida do jogador com três corações.

### Área superior abaixo do cabeçalho

Painel do objetivo atual do tutorial.

### Canto inferior esquerdo

Resumo da equipe equipada:

- Ícones dos três Pets.
- DPS total teórico da equipe.

### Canto inferior direito

Botão de inventário com indicação da tecla I.

### Elementos contextuais

- Indicação de interação ao aproximar-se de máquinas e barreiras.
- Indicador de inimigo selecionado.
- Números flutuantes de dano.
- Recompensas em moedas.
- Barra de vida do Boss.
- Mensagens breves de erro ou confirmação.

## 6. Comportamento das interfaces

O jogo terá apenas uma interface modal principal aberta por vez.

Quando o inventário, o Gacha ou o menu de pausa estiver aberto:

- O cursor do mouse ficará disponível.
- A seleção de inimigos será bloqueada.
- A movimentação do personagem será bloqueada.
- O controle da câmera será bloqueado.

No MVP, o mundo continuará ativo durante o inventário e o Gacha. Portanto, ataques de Boss não serão pausados por esses menus.

O tutorial não deverá instruir o jogador a abrir menus durante um combate obrigatório com Boss.

O menu de pausa utilizará Time.timeScale = 0, suspendendo a jogabilidade enquanto estiver aberto.

## 7. Menu inicial

O menu inicial conterá:

- Logo Fluffy Fighters.
- Botão Jogar.
- Botão Sair.
- Fundo temático utilizando elementos visuais do jogo.

Não haverá sistema de contas, seleção de personagens ou multiplayer.

## 8. Menu de pausa

O menu de pausa será aberto utilizando Esc quando não houver outra interface modal.

Botões:

- Continuar.
- Voltar ao menu inicial.
- Sair do jogo.

O progresso será salvo antes de voltar ao menu inicial ou sair normalmente.

## 9. Conclusão do jogo

Ao derrotar o Guardião Glacial pela primeira vez, será exibida uma tela:

**Parabéns! Você conquistou todas as regiões de Fluffy Fighters!**

O jogador poderá fechar a mensagem e continuar explorando, obtendo Pets, realizando fusões e enfrentando Bosses.

A primeira vitória contra o Boss final deverá ser registrada no salvamento.

A apresentação cartoon utiliza a identidade visual das interfaces e um botão para continuar jogando. Se outro menu estiver aberto quando a vitória acontecer, a mensagem aguarda o fechamento desse menu.

Não haverá créditos cinematográficos ou uma cena final separada no MVP.

## 10. Sistema de salvamento

O jogo terá um único slot de progresso local.

Os dados serão armazenados em um arquivo JSON no diretório persistente da aplicação, usando Application.persistentDataPath.

### Dados persistidos

- Versão do formato de salvamento.
- Quantidade de moedas.
- Pets adquiridos.
- Identificadores dos Pets.
- Quantidade de cópias por nível de fusão.
- Pets equipados em cada slot.
- Regiões desbloqueadas.
- Etapa atual do tutorial.
- Estado de conclusão do tutorial.
- Primeira vitória contra o Boss final.

### Dados não persistidos

- Vida atual de inimigos.
- Vida atual de Bosses.
- Alvo selecionado.
- Temporizadores de ataque.
- Posição temporária dos Pets.
- Estado das animações.
- Estado das interfaces abertas.
- Vida atual do jogador.

Ao carregar o jogo, o personagem reaparecerá com vida completa no ponto inicial da última região desbloqueada.

## 11. Momentos de salvamento

O jogo executará salvamento automático:

- Após receber um Pet do Gacha.
- Após equipar ou desequipar.
- Após realizar uma fusão.
- Após comprar uma barreira.
- Após concluir uma etapa do tutorial.
- Após a primeira vitória contra o Boss final.
- Periodicamente, enquanto estiver jogando.
- Ao retornar ao menu inicial ou sair normalmente.

As moedas recebidas em combate deverão integrar o estado salvo, inclusive por meio do salvamento periódico.

### Integridade

O sistema deverá:

- Evitar quantidades negativas.
- Evitar referências inválidas de Pets.
- Restaurar corretamente os equipamentos.
- Impedir a gravação de dados incompletos durante operações de Gacha ou fusão.
- Utilizar gravação temporária e substituição segura do arquivo, quando possível.
- Manter uma cópia de segurança do salvamento anterior.

## 12. Estrutura técnica

### TutorialManager

Controla a etapa atual, recebe eventos dos sistemas e atualiza objetivos.

### HUDManager

Apresenta moedas, vida, equipe, região e objetivo.

### UIManager

Controla abertura e fechamento de interfaces modais.

### MainMenuController

Controla Jogar e Sair.

### PauseMenuController

Controla a pausa e o retorno ao jogo.

### SaveSystem

Realiza serialização, gravação, leitura e validação do progresso.

### PlayerProgressData

Estrutura serializável com os dados necessários para restaurar uma partida.

### GameManager

Coordena a inicialização e a integração dos sistemas gerais.

## 13. Critérios de aceitação

O capítulo estará implementado quando:

1. O jogador começar com 100 moedas e inventário vazio em uma nova partida.
2. O tutorial orientar o primeiro Roll.
3. O tutorial detectar o primeiro Pet recebido.
4. O jogador conseguir equipá-lo.
5. O tutorial reconhecer a primeira derrota de um inimigo.
6. A progressão continuar até o Boss inicial e a primeira barreira.
7. As instruções aparecerem e desaparecerem corretamente.
8. O inventário e o Gacha bloquearem controles indevidos.
9. A HUD atualizar moedas, vida e DPS.
10. O jogador conseguir pausar e retomar o jogo.
11. O progresso persistir após fechar e reabrir a aplicação.
12. Os equipamentos e níveis de fusão forem restaurados corretamente.
13. As barreiras compradas permanecerem abertas.
14. A primeira vitória contra o Boss final exibir a mensagem de conclusão.
15. O jogador conseguir continuar jogando após concluir o protótipo.

## 14. Restrições do MVP

Não serão implementados:

- Contas online.
- Salvamento em nuvem.
- Múltiplos slots.
- Customização do personagem.
- Configurações gráficas avançadas.
- Localização para múltiplos idiomas.
- Sistema de conquistas.
- Missões secundárias.
- Cinemáticas de história.

**Fim do GDD v0.8.**
