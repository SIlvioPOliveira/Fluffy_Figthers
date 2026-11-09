# FLUFFY FIGHTERS — CATÁLOGO DE CRIATURAS

**Versão:** 0.1 — estrutura inicial  
**Escopo:** pets, inimigos, elites e bosses.  
**Estado do catálogo:** ainda não foram fornecidas fichas individuais oficiais suficientes para registrar uma lista canônica de personagens. **Não inferir o elenco a partir de exemplos visuais.**

## 1. Estados válidos

- `IDEIA`: conceito não aprovado.
- `FICHA_PENDENTE`: personagem previsto, detalhes insuficientes.
- `FICHA_APROVADA`: design especificado e aprovado para iniciar blockout.
- `BLOCKOUT`: volumetria inicial em produção.
- `REVISAO`: modelo aguardando avaliação.
- `MODELO_APROVADO`: modelagem artística aprovada.
- `EXPORTADO`: exportado e verificado para Unity.

O agente **não** deve promover um personagem para `FICHA_APROVADA` ou `MODELO_APROVADO` sem decisão do responsável pelo projeto.

## 2. Regras de identificação

- `PET_<SpeciesOrName>_<NN>` — pet.
- `ENM_<SpeciesOrName>_<NN>` — inimigo comum.
- `ELITE_<SpeciesOrName>_<NN>` — inimigo especial.
- `BOSS_<SpeciesOrName>_<NN>` — boss.
- IDs são estáveis após aprovação. Não reutilizar ID excluído nem sobrescrever modelo existente.

## 3. Campos obrigatórios para geração por IA

| Campo | Descrição |
|---|---|
| `id` | Identificador único, seguro para nomes de objeto/arquivo |
| `categoria` | PET / ENM / ELITE / BOSS |
| `status` | Um dos estados acima |
| `nome_exibicao` | Nome legível |
| `arquetipo` | compacto / esferico / especial |
| `especie_conceito` | Animal ou criatura de base |
| `elemento` | agua / fogo / lava / gelo / neutro / outro / TBD |
| `silhueta` | Descrição da forma primária |
| `expressao` | happy / sleepy / surprised / determined / sad / mischievous / hostile / custom |
| `cor_primaria` | Nome ou HEX confirmado |
| `cor_secundaria` | Nome ou HEX confirmado |
| `detalhes` | Lista de estruturas anatômicas efetivamente solicitadas |
| `restricoes` | O que não deve ser criado |
| `observacoes` | Referências e decisões específicas |

Campos críticos sem definição devem ser marcados `TBD`. O agente pode propor preenchimentos, mas precisa separá-los das fichas aprovadas.

## 4. Template de ficha — COPIAR E PREENCHER

```yaml
id: PET_Nome_01
categoria: PET
status: FICHA_PENDENTE
nome_exibicao: TBD
arquetipo: compacto
especie_conceito: TBD
elemento: TBD
silhueta: TBD
expressao: happy
olhos: estilo_B
cor_primaria: TBD
cor_secundaria: TBD
cor_destaque: TBD
detalhes: []
restricoes:
  - nao_adicionar_acessorios_nao_solicitados
observacoes: TBD
arquivo_blend: TBD
render_front: TBD
render_side: TBD
render_three_quarter: TBD
aprovado_por: TBD
```

## 5. Exemplo de ficha de TESTE (NÃO CANÔNICA)

A ficha a seguir existe apenas para validar o pipeline Blender MCP. **Não representa um pet já definido para o jogo.**

```yaml
id: PET_FireFox_Test_01
categoria: PET
status: IDEIA
nome_exibicao: Raposa de Fogo (teste)
arquetipo: compacto
especie_conceito: raposa
elemento: fogo
silhueta: corpo compacto arredondado, orelhas grandes, quatro patas curtas
expressao: happy
olhos: estilo_B
cor_primaria: laranja
cor_secundaria: creme
cor_destaque: vermelho_alaranjado
detalhes:
  - orelhas_grandes
  - quatro_patas_curtas
  - cauda_flamejante_estilizada
restricoes:
  - sem_roupas
  - sem_armadura
  - sem_texturas_realistas
observacoes: exemplo para teste procedural, sujeito a aprovacao
arquivo_blend: TBD
render_front: TBD
render_side: TBD
render_three_quarter: TBD
aprovado_por: TBD
```

## 6. Cadastro oficial — aguardando fichas

| ID | Nome | Categoria | Elemento | Estado |
|---|---|---|---|---|
| *A preencher* | *A preencher* | *A preencher* | *A preencher* | `FICHA_PENDENTE` |

> A existência planejada de regiões Água, Fogo, Lava e Gelo **não determina automaticamente** quantos pets, inimigos ou bosses existem ou quais seus nomes.

## 7. Regra de uso pelo agente

Para criar um modelo de produção, exigir ficha `FICHA_APROVADA`. Para executar um **teste de pipeline**, usar somente um exemplo rotulado como `IDEIA` e registrar que não é asset oficial. Nunca transformar a ficha de teste em personagem canônico sem aprovação explícita.
