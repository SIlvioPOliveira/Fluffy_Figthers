# FLUFFY FIGHTERS — VISUAL IDENTITY

**Versão:** 0.3 (base de trabalho)  
**Escopo:** identidade artística de pets, inimigos e bosses em 3D.  
**Fora de escopo:** UI, HUD, menus, cartas Gacha, logotipia, ambientes, VFX gerais e identidade sonora.

## 1. Origem e status das diretrizes

### Fundamentado no TCC fornecido

O TCC *Desenvolvimento de um Jogo Digital do Tipo Gacha com Sistema de Recompensas utilizando Unity* estabelece:
- Unity como motor, C# para programação e Blender como ferramenta de modelagem.
- Pets colecionáveis ligados a dados de nome, ícone, prefab 3D e atributos.
- Direção de arte inspirada em *Pet Simulator 99*, com geometria simples, silhuetas exageradas, cores vibrantes e estética cute/kawaii.
- Escolha low poly por motivos de expressividade, viabilidade produtiva e desempenho.

**Observação:** o TCC não define medidas anatômicas, orçamento poligonal, cores hexadecimais, topologia, olhos ou especificações precisas. Essas regras são decisões de direção artística desenvolvidas para a produção do jogo, não transcrições do artigo.

### Decisões tomadas na conversa (APROVADAS)

- **Estética principal:** Cartoon Low Poly Híbrido / *Cute Stylized Low Poly Fantasy*.
- **Olhos:** estilo B — grandes, expressivos, simples, com brilho pequeno/controlado.
- **Escopo deste conjunto de documentos:** modelagem artística de pets, inimigos e bosses; interface terá guia próprio.
- **Prioridade:** documentar regras que IA e artista consigam aplicar de maneira consistente em Blender.

### Em aberto (NÃO APROVADO)

- Forma-base predominante: cubo suavizado, cubo arredondado ou formas orgânicas simples. **Default operacional provisório: cubo arredondado para pet compacto**, até decisão final.
- Valores exatos de proporções, limites de geometria, paletas oficiais e detalhes visuais individuais das espécies. A lista de 20 pets e sua distribuição estão registradas em `PET_CATALOG.md`.
- Escalas relativas definitivas entre pets, jogador, inimigos e bosses.

## 2. Frase-guia

> Criaturas de fantasia em 3D com silhuetas claras, volumes compactos, cantos suavizados, rostos muito expressivos, cores vibrantes e detalhes geométricos reconhecíveis, sem realismo fotográfico.

## 3. Pilares obrigatórios

1. **Legibilidade:** reconhecer a criatura pela forma mesmo sem texturas.
2. **Carisma:** olhos estilo B e expressões comunicam a personalidade com poucos elementos.
3. **Simplicidade produtiva:** primitivas, poucas peças principais e materiais fáceis de editar.
4. **Coesão:** pets e inimigos compartilham materiais e construção, variando linguagem corporal e expressão.
5. **Variedade controlada:** espécies diferentes podem ter anatomias próprias sem abandonar princípios comuns.
6. **Baixo custo de renderização:** preservar performance sem sacrificar silhueta e expressão.

## 4. Linguagem de formas

**Preferir:** cubos com bevel moderado, esferas simplificadas, cilindros e cones com poucas faces, volumes orgânicos leves produzidos por manipulação de primitivas. Detalhes principais devem ser volumes reais quando mudarem a silhueta.

**Evitar:** realismo anatômico, poros/pelos realistas, subdivisão indiscriminada, texturas fotográficas, metalização generalizada, microdetalhes invisíveis na câmera de jogo.

### Famílias anatômicas de pets

- **A — Compacto:** corpo cúbico/volume único, cabeça integrada ou destacada, patas curtas (ex.: pequenos mamíferos).
- **B — Esférico:** corpo arredondado, sem membros obrigatórios (ex.: slime, espírito).
- **C — Especial:** anatomia própria (ex.: dragão, elemental, híbrido), obedecendo à linguagem comum.

Estas famílias são categorias de produção, **não** a lista oficial de personagens.

## 5. Linguagem facial

- Olhos grandes e posicionados para leitura frontal e em 3/4.
- Formato base: oval arredondado ou volume esférico levemente achatado, preferencialmente escuro.
- Reflexo pequeno, simples e consistente (ponto ou oval claro).
- Sobrancelhas, pálpebras e boca são recursos opcionais de expressão; nunca obrigatórios para todas as criaturas.
- Iris detalhada é opcional, somente quando a ficha do personagem justificar.
- O rosto deve ser legível a distância real de gameplay, não apenas em render close-up.

### Vocabulário de expressões

| Expressão | Recursos preferidos |
|---|---|
| Feliz | Olhos abertos, boca/sorriso simples |
| Sonolento | Pálpebras baixas, boca neutra |
| Surpreso | Olhos abertos, boca pequena circular |
| Determinado | Sobrancelhas levemente inclinadas e postura assertiva |
| Triste | Pálpebras/sobrancelhas suaves e boca descendente |
| Travesso | Assimetria discreta, sorriso lateral |
| Hostil (inimigo) | Sobrancelhas marcadas, postura de confronto; ainda cartoon |

## 6. Diferenciação por função

| Tipo | Silhueta | Rosto | Complexidade |
|---|---|---|---|
| Pet | Compacta, convidativa, formas suaves | Amigável ou específica da personalidade | Simples a moderada |
| Inimigo comum | Familiar ao universo, com sinais de ameaça | Determinado/hostil sem horror realista | Simples |
| Inimigo especial | Mais distinto, acessórios estruturais | Mais intenso | Moderada |
| Boss | Silhueta única e dominante | Forte personalidade | Maior, mas controlada |

Bosses **não** devem ser meramente inimigos aumentados por escala. Devem ter forma primária distintiva e detalhes secundários coerentes.

## 7. Direção elemental (para personagens)

Há planejamento anterior de regiões de Água, Fogo, Lava e Gelo. O uso abaixo serve como **vocabulário sugerido**, não como catálogo obrigatório de criaturas:

- **Água:** curvas, gotas, nadadeiras, volumes fluidos; azuis/turquesas.
- **Fogo:** formas ascendentes, pontas suavizadas, laranjas/vermelhos.
- **Lava:** blocos de rocha, fendas emissivas pontuais, contraste escuro/quente.
- **Gelo:** prismas, cristais, ângulos mais definidos, azuis frios.

O elemento deve ser reconhecível pela silhueta/estrutura, e não somente pela troca de cor.

## 8. Cor, acabamento e materiais

- Cores vibrantes com poucas cores dominantes por criatura.
- Priorizar material fosco ou acetinado. Brilho seletivo em olhos, cristais e partes mágicas.
- Emissão apenas onde justificada. Não usar emissão indiscriminada no corpo.
- Para cada criatura, a ficha deve explicitar cor primária, secundária e destaque, com valores RGB/HEX se aprovados.
- **Paleta oficial do jogo ainda não foi aprovada**; não tratar códigos hex sugeridos anteriormente como vinculantes.

## 9. Regras negativas (o que evitar)

- Não transformar todos os pets em clones com troca de cor.
- Não gerar membros/acessórios não previstos na ficha.
- Não usar olhos hiper-realistas ou humanos.
- Não exagerar brilhos/reflexos a ponto de esconder os volumes.
- Não comprometer a expressão facial com acessórios na frente do rosto.
- Não usar texturas complexas para corrigir silhueta mal resolvida.

## 10. Critérios de aprovação artística

Um personagem está visualmente **aprovável** quando:

- Sua silhueta é reconhecível sem materiais.
- A expressão corresponde à ficha.
- Olhos seguem o estilo B.
- Corpo, membros e acessórios parecem pertencer ao mesmo estilo.
- Cores são coerentes com a ficha e o elemento.
- É legível nas vistas frontal, lateral e três quartos.
- É comparável com outras criaturas do mesmo universo sem parecer de outro jogo.

A aprovação final deve ser humana; uma IA pode preparar a checagem, mas não autodeclarar aprovação criativa definitiva.

## 11. Relação com os demais arquivos

- `CHARACTER_MODELING_RULES.md`: implementação técnica no Blender e testes.
- `PET_CATALOG.md`: fichas concretas que determinam detalhes por personagem.
- `../../AGENTS.md`: procedimentos, limites e prioridades para agentes.
