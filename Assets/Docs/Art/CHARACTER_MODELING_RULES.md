# FLUFFY FIGHTERS — CHARACTER MODELING RULES

**Versão:** 0.3 (proposta técnica inicial)  
**Aplicação:** Blender + Python `bpy`, modelagem manual ou via agente MCP.  
**Categorias:** `PET`, `ENM`, `ELITE`, `BOSS`.  
**Dependência:** ler `VISUAL_IDENTITY.md` e a ficha do personagem em `PET_CATALOG.md` antes de construir.

## 1. Algoritmo de trabalho para IA

1. Carregar ficha aprovada; se incompleta, marcar campo como `TBD` e não inventar uma decisão canônica.
2. Escolher família anatômica (`compacto`, `esferico`, `especial`) e forma primária.
3. Fazer **blockout** com volumes principais; testar leitura da silhueta.
4. Adicionar formas secundárias: cabeça, membros, cauda, orelhas, chifres, asas conforme ficha.
5. Construir face estilo B; ajustar ao volume da superfície frontal.
6. Aplicar materiais simples e identificar partes emissivas, se houver.
7. Produzir visualizações frontal, lateral e 3/4; checar distorção, sobreposição, flutuação ou penetração indesejada.
8. Salvar `.blend` em coleção/arquivo próprios. Relatar evidências e pontos pendentes.
9. Não exportar ou integrar à Unity sem solicitação/aprovação.

## 2. Espaço de trabalho

- Sistema métrico; **1 unidade Blender = 1 metro** como convenção de trabalho proposta.
- Eixos no Blender: `Z` cima, `X` esquerda/direita, **frente para `-Y`**.
- Centro de referência: (`0`, `0`, `0`) na base de apoio; caso flutuante, registrar altura de flutuação.
- Manter escala de objetos consistente e evitar espelhamento com escala negativa não aplicada no asset final.
- Para modelos simétricos usar eixo `X` como plano de simetria; assimetria somente quando intencional.
- Não assumir que o eixo de frente do Blender seja automaticamente igual ao eixo de frente da Unity; testar FBX/importação.

## 3. Modelo compacto — proporções de partida, NÃO rígidas

Para blockout de quadrúpede compacto com corpo de largura `1.0`:

| Medida | Valor inicial | Interpretação |
|---|---:|---|
| Largura do corpo | 1.00 | referência |
| Profundidade do corpo | 0.85 | aproximadamente |
| Altura do corpo | 0.80 | exclui orelhas/membros |
| Altura de patas | 0.10–0.20 × altura do corpo | curtas |
| Altura das orelhas | 0.15–0.30 × altura do corpo | quando presentes |
| Altura de cada olho | 0.15–0.22 × altura visível da face | legível |
| Distância livre entre olhos | ~1 largura de olho | ajustar por espécie |

Essas medidas são **defaults operacionais para testes**, não restrições universais. O corpo pode ser esfera, cubo suavizado ou anatomia especial conforme ficha. Não esticar proporções até perder identidade da espécie.

## 4. Peças recomendadas e nomes

Um personagem deve ser uma coleção individual (`PET_Cat_01`, etc.) e conter uma raiz lógica, por exemplo `PET_Cat_01_ROOT`. Peças mínimas dependem da anatomia.

```text
PET_Example_01 (Collection)
└── PET_Example_01_ROOT (Empty)
    ├── Body
    ├── Head                 # opcional: pode ser integrado
    ├── Eye_L
    ├── Eye_R
    ├── Highlight_L
    ├── Highlight_R
    ├── Mouth                # opcional
    ├── Brow_L               # opcional
    ├── Brow_R               # opcional
    ├── Ear_L                # conforme espécie
    ├── Ear_R
    ├── Paw_Front_L          # conforme espécie
    ├── Paw_Front_R
    ├── Paw_Back_L
    ├── Paw_Back_R
    └── Tail                 # conforme espécie
```

Nomear peças adicionais semanticamente (`Horn_L`, `Wing_R`, `Crystal_01`, `FlameTip`), sem nomes genéricos repetidos (`Cube.093`, `Sphere.014`) no entregável final.

## 5. Primitivas sugeridas

| Parte | Método inicial preferido |
|---|---|
| Corpo/rosto compacto | cubo + bevel controlado |
| Corpo redondo/slime | UV sphere de baixa segmentação |
| Olhos | esfera levemente achatada em direção à face |
| Pupilas/íris | opcionais, geometrias planas ou pouco salientes |
| Reflexos | pontos/ovais claros pequenos |
| Patas | cubos arredondados ou cilindros curtos |
| Orelhas | malha prismática ou cubo deformado |
| Cauda | cilindros segmentados ou curva com espessura |
| Chifres | cones baixos / malhas com poucos segmentos |
| Cristais | prismas com faces definidas |
| Asas | malhas simples com espessura |

### Bevel e suavização

- Primeiro determinar forma/silhueta; só depois ajustar bevel.
- Não usar bevel com segmentos altos apenas por aparência de suavidade.
- Defaults experimentais para cubos arredondados: bevel de aproximadamente **5–12% da menor dimensão**, **2–3 segmentos**. Ajustar conforme câmera, escala e natureza do objeto. **Ainda não aprovados como regra definitiva.**
- Sombras normais devem ser verificadas visualmente. Evitar artefatos causados por normal invertida e superfícies coincidentes.
- Usar smooth shading seletivamente; preservar faces planas quando fizerem parte da silhueta low poly.

## 6. Olhos estilo B — requisitos

- O olho deve ler como **oval arredondado**, escuro, relativamente grande e expressivo.
- Preferir duas geometrias principais (`Eye_L`, `Eye_R`) com pequenos reflexos (`Highlight_L`, `Highlight_R`).
- Reflexos sem cobrir parcela significativa da superfície do olho; um pequeno brilho principal costuma bastar.
- Posicionar sobre a superfície frontal; verificar face de frente e em três quartos.
- Usar a convenção esquerda/direita **do personagem**, não da câmera.
- Não aplicar textura de íris fotográfica.
- Olhos precisam evitar interseção óbvia com corpo, orelhas, chifres e boca.
- Sobrancelhas opcionais para expressões de inimigo, elite, boss e pets com personalidade específica.

### Construção facial por emoção

- `happy`: boca curva curta ou sorriso, olhos abertos.
- `sleepy`: pálpebra ou formato de olho mais fechado.
- `surprised`: olhos abertos, boca em pequeno círculo opcional.
- `determined`: sobrancelhas inclinadas e postura firme.
- `sad`: sobrancelhas suaves ou boca descendente.
- `mischievous`: assimetria leve aprovada na ficha.
- `hostile`: intensidade cartoon, evitando terror realista.

## 7. Diferença entre categorias

### Pets
- Priorizar formas compactas e convidativas, focadas em coleção e expressão.
- Detalhes principais: 2–4 elementos marcantes como ponto de partida, não uma obrigação.
- Novos pets não devem ser apenas recolorações de modelos existentes sem que a ficha preveja variantes.

### Inimigos comuns
- Mesmo universo geométrico; expressão e postura mais competitivas.
- De 1 a 3 marcas visuais distintivas como recomendação inicial.
- Evitar confusão com pets aliados: mudar silhueta, face ou leitura da postura.

### Elites
- Aumentar tamanho relativo e detalhes estruturais, sem excesso decorativo.
- Diferenciar de inimigo comum por acessório geométrico, anatomia ou postura.

### Bosses
- Definir uma silhueta exclusiva a partir de forma primária grande.
- Formas secundárias reforçam espécie e elemento; terciárias decoram sem esconder a silhueta.
- Planejar partes móveis e antecipações de ataque se existir animação posterior.
- Não reutilizar um inimigo comum apenas com `scale` maior como solução final.

## 8. Materiais e texturas

- Preferir materiais simples, poucas variações por criatura.
- Base predominantemente fosca/acetinada; olhos com brilho seletivo e reflexos simples.
- Cores primária/secundária/destaque vêm da ficha individual.
- Texturas fotográficas, ruído detalhado e metalização exagerada são proibidos salvo exceção explicitamente aprovada.
- Emissão apenas para efeitos coerentes, e preferencialmente discreta.
- A paleta global oficial ainda será fixada; evitar inventar códigos oficiais.

## 9. Geometria e performance

A meta de 60 FPS aparece como requisito do TCC, mas **não autoriza presumir um orçamento universal de triângulos**. Faixas abaixo são apenas hipóteses de protótipo e devem ser aferidas na cena real:

| Categoria | Faixa de triângulos de teste (não obrigatória) |
|---|---:|
| Pet simples | 300–1.000 |
| Pet elaborado | 1.000–2.500 |
| Inimigo comum | 500–2.000 |
| Boss | 3.000–10.000 |

Não acrescentar geometria apenas para alcançar um orçamento. Validar densidade, materiais, draw calls, número de criaturas e rig/animação em gameplay.

## 10. Como produzir usando Blender MCP

- Verificar quais ferramentas reais o MCP expõe; não presumir que suporta render, execução Python ou gravação de arquivos.
- Preferir uma função `build_character(spec)` ou equivalente que recebe a ficha e cria peças de modo determinístico.
- Nomes e paths devem ser construídos a partir de IDs sanitizados, sem sobrescrever assets existentes.
- Scripts devem evitar depender de seleção ou viewport ativa quando houver alternativa programática estável (`bpy.data`, `bpy.context` controlado).
- Ao gerar variantes, registrar parâmetros da construção; idealmente salvar script junto ao `.blend`.
- Criar coleção dedicada para nova criatura, sem apagar o restante da cena.
- Antes de salvar, inspecionar existência dos objetos esperados, materiais e geometria visível.

## 11. Entregáveis e validação

### Entregáveis mínimos por personagem

- `.blend` fonte com coleção/nomenclatura organizada.
- Ficha de personagem associada a ID único.
- Três imagens de revisão: frontal, lateral e três quartos, quando o MCP/Blender permitir.
- Resumo curto: materiais, objetos, alterações e pendências.

### Checklist

- [ ] Silhueta é compreensível em material neutro.
- [ ] Olhos estilo B visíveis e corretamente orientados.
- [ ] Expressão confere com ficha.
- [ ] As partes não apresentam colisões visuais não intencionais.
- [ ] O modelo respeita a categoria e o elemento.
- [ ] Origem, eixos e escala são coerentes.
- [ ] Objetos e materiais possuem nomes legíveis.
- [ ] Vistas frontal, lateral e 3/4 foram revisadas.
- [ ] O arquivo `.blend` foi salvo e sua existência confirmada.
- [ ] Exportação/integração à Unity não foi presumida como concluída.

## 12. Convenções de saída sugeridas (ajustar após verificar repositório)

```text
ArtSource/Blender/Pets/PET_Example_01.blend
ArtSource/Blender/Enemies/ENM_Example_01.blend
ArtSource/Blender/Bosses/BOSS_Example_01.blend
ArtSource/Blender/Reviews/PET_Example_01_front.png
ArtSource/Blender/Reviews/PET_Example_01_side.png
ArtSource/Blender/Reviews/PET_Example_01_three_quarter.png
```

## 13. Critérios para pedir revisão

Pedir aprovação se: anatomia se desviar da família; houver decisão não definida de cor ou expressão; surgirem acessórios novos; o modelo não atingir legibilidade; for necessário alterar malhas preexistentes; ou antes de exportação/inserção em cena final.
