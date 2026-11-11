# Fluffy Fighters — interface: proposta de produção

**Status:** direção cartoon colorida e conceito de inventário aprovados pelo usuário; cores exatas, tipografia e acabamento ainda estão em avaliação.

## Direção sugerida

Interface de fantasia cartoon acolhedora, alinhada à identidade cute dos Pets: formas arredondadas, cores alegres, contraste suficiente e poucos detalhes decorativos. Os modelos dos Pets devem ser protagonistas. A cor ajuda a identificar raridade, mas o nome da raridade também deve aparecer.

O protótipo usa painéis em azul e creme com detalhes dourados, molduras coloridas de raridade, realce ao passar o mouse e uma área própria para o modelo. Patinhas, espada e brilho são desenhos vetoriais do projeto. A fonte experimental é Baloo 2, dos autores do projeto Baloo 2, sob SIL Open Font License 1.1; o arquivo completo da licença acompanha a fonte em `Assets/UI/Fonts/Baloo2/OFL.txt`. O acabamento final segue em avaliação.

O conceito visual em `ArtSource/UI/conceito-inventario-v1.png` é uma referência de composição, não uma tela funcional nem uma especificação final de arte. A imagem contém personagens, filtros, atributos e níveis ilustrativos que não representam novas mecânicas; os dados e as regras reais continuam sendo os dos GDDs.

## Primeira tela: inventário

- Cabeçalho: título, botão de fechar, DPS total e três espaços da equipe. Clicar em um espaço ocupado seleciona o Pet e abre seus detalhes; espaços vazios não alteram a seleção.
- Coleção: grade rolável com retrato frontal do Pet, raridade, dano, nível, quantidade e indicador de cópias equipadas; ordenação conforme o GDD. Uma, duas ou três estrelas no canto do card representam os níveis de fusão 1, 2 e 3.
- Detalhes: prévia 3D giratória do Pet selecionado, nome, região, raridade, dano, nível e quantidades.
- Ações: equipar, desequipar e fundir, com estados disponíveis e indisponíveis claros; confirmação antes da fusão.
- Estado vazio: orientar a obtenção de Pets no Gacha. Clicar novamente no Pet selecionado desfaz a seleção e devolve o retrato frontal.

## Componentes reutilizáveis

Painel modal, botão principal/secundário, botão de fechar, card de Pet, indicação de raridade, contador, mensagens e confirmação. A prévia 3D já existente deve ser reaproveitada no inventário e no Gacha.

## Ordem de produção

1. Avaliar a proposta visual e definir cores, fontes, formas e referências com o usuário.
2. Construir a base responsiva e implementar o inventário em uma cena de teste isolada, preservando a interface atual até validação. O primeiro protótipo usa o UI Toolkit nativo da Unity; a HUD continua seguindo sua especificação de Canvas.
3. Testar seleção, equipamento, fusão, diferentes quantidades de Pets, legibilidade e resoluções; testar fechamento, controles e salvamento.
4. Integrar o inventário aprovado ao jogo e adaptar os mesmos componentes ao Gacha, incluindo roleta e resultado.
5. Adaptar HUD, menus, tutorial e demais mensagens, uma tela por vez; revisar sons e animações depois da base visual.

## Condições de aceitação

- As ações existentes mantêm as regras de inventário, equipamentos, fusão e persistência dos GDDs.
- As telas permanecem legíveis e utilizáveis em diferentes resoluções de PC, sem sobreposição de elementos.
- O inventário e o Gacha mantêm o comportamento modal e o mundo ativo conforme o GDD de HUD e menus.
- O visual, a legibilidade e a sensação de uso são validados pelo usuário dentro da Unity antes da substituição da interface provisória.

## Pendências para definição

Paleta e acabamento final; validação da fonte no jogo; ícones de moedas, corações e outros comandos; sons e animações de interação; referências e preferência visual do usuário.
