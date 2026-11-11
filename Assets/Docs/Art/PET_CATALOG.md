# FLUFFY FIGHTERS — CATÁLOGO DE CRIATURAS

**Versão:** 0.1 — estrutura inicial  
**Escopo:** pets, inimigos, elites e bosses.  
**Estado do catálogo:** o responsável forneceu os 20 nomes e sua distribuição. As fichas e o estado de aprovação visual de cada modelo ainda precisam ser registrados individualmente. **Não inferir detalhes a partir de exemplos visuais.**

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

## 6. Elenco de pets informado pelo responsável

Os IDs abaixo são os identificadores usados pelo jogo e pelo salvamento. Nomes e distribuição seguem a lista informada; a inclusão nesta tabela **não atesta aprovação visual, exportação ou integração de um modelo**. As fichas individuais, outros personagens e o estado dos modelos ainda precisam de registro.

| ID no jogo | Região | Raridade | Nome | Dano base |
|---|---|---|---|---:|
| `water_peixinho` | Água | Comum | Peixinho | 5 |
| `water_tartaruga` | Água | Raro | Tartaruga | 8 |
| `water_polvinho` | Água | Épico | Polvinho | 12 |
| `water_golfinho` | Água | Lendário | Golfinho | 18 |
| `water_dragao_marinho` | Água | Secreto | Dragão Marinho | 30 |
| `fire_gatinho` | Fogo | Comum | Gatinho de Fogo | 20 |
| `fire_raposa` | Fogo | Raro | Lobo de Fogo | 30 |
| `fire_fenix` | Fogo | Épico | Fênix Bebê | 45 |
| `fire_leao` | Fogo | Lendário | Leão Solar | 65 |
| `fire_dragao` | Fogo | Secreto | Dragão de Fogo | 100 |
| `lava_pedrinha` | Lava | Comum | Pedrinha Vulcânica | 70 |
| `lava_caranguejo` | Lava | Raro | Caranguejo Magma | 105 |
| `lava_salamandra` | Lava | Épico | Salamandra | 160 |
| `lava_urso` | Lava | Lendário | Urso Vulcânico | 230 |
| `lava_dragao` | Lava | Secreto | Dragão de Magma | 350 |
| `ice_pinguinzinho` | Gelo | Comum | Pinguinzinho | 240 |
| `ice_raposa` | Gelo | Raro | Raposa Polar | 360 |
| `ice_lobo` | Gelo | Épico | Yet | 540 |
| `ice_urso` | Gelo | Lendário | Urso Glacial | 800 |
| `ice_dragao` | Gelo | Secreto | Dragão de Cristal | 1.200 |

Os IDs `fire_raposa` e `ice_lobo` são históricos e foram preservados para manter a compatibilidade com os salvamentos. Veja também [progressão e balanceamento](../GDD%20-%20ProgBalPets.md).

### Arquivos recebidos para os modelos

Origem informada: `C:/Users/sotav/Downloads/Modelos 3D Pts/ModelosProntos`. Os caminhos abaixo são relativos a essa pasta, que está fora do repositório. A correspondência do Gatinho com `fire_raposa.zip` foi confirmada pelo responsável; as demais são identificações pelos nomes dos arquivos. Cópias dos 20 FBX e texturas foram importadas para `Assets/Art/Pets/ModelosProntos` e associadas aos respectivos pets por prefabs em `Assets/Art/Pets/Prefabs` para testes.

| Pet | Arquivo recebido |
|---|---|
| Peixinho | `AGUA/water_peixinho.zip` |
| Tartaruga | `AGUA/water_tartaruginhal.zip` |
| Polvinho | `AGUA/water_polvinho/cute_purple_octopus_3d_model.fbx` |
| Golfinho | `AGUA/water_golfinho.zip` |
| Dragão Marinho | `AGUA/water_dragaoMarinho.zip` |
| Gatinho de Fogo | `FOGO/fire_raposa.zip` |
| Lobo de Fogo | `FOGO/fire_Lobo.zip` |
| Fênix Bebê | `FOGO/fire_fenix.zip` |
| Leão Solar | `FOGO/fire_leaoSolar.zip` |
| Dragão de Fogo | `FOGO/fire_DragaoFogo.zip` |
| Pedrinha Vulcânica | `LAVA/lava_pedrinha.zip` |
| Caranguejo Magma | `LAVA/lava_carangueijo.zip` |
| Salamandra | `LAVA/lava_salamandra.zip` |
| Urso Vulcânico | `LAVA/lava_urso.zip` |
| Dragão de Magma | `LAVA/lava_dragao.zip` |
| Pinguinzinho | `GELO/ice_pinguin.zip` |
| Raposa Polar | `GELO/ice_fox.zip` |
| Yet | `GELO/ice_yet.zip` |
| Urso Glacial | `GELO/ice_urso.zip` |
| Dragão de Cristal | `GELO/ice_dragon.zip` |

Inspeção técnica inicial: cada FBX contém uma malha com UV e material de textura; não foram encontrados esqueletos ou animações internas. Os arquivos originais e suas texturas foram preservados; as texturas estão configuradas na Unity com resolução máxima de 4096 para teste. Todos os prefabs possuem textura e foram instanciados pelo equipamento. A cena `TesteModelosPets` permite experimentar os 20 modelos sem usar o salvamento do jogador. Escala, orientação e aparência exigem revisão manual.

## 7. Regra de uso pelo agente

Para criar um modelo de produção, exigir ficha `FICHA_APROVADA`. Para executar um **teste de pipeline**, usar somente um exemplo rotulado como `IDEIA` e registrar que não é asset oficial. Nunca transformar a ficha de teste em personagem canônico sem aprovação explícita.
