# AGENTS.md — Fluffy Fighters

> Instruções para agentes de IA que trabalham neste repositório. Versão 0.1. Escopo atual: **produção artística de pets, inimigos e bosses no Blender**. Este arquivo não autoriza mudanças automáticas em gameplay, Gacha, UI ou cenas Unity.

## 1. Contexto

Fluffy Fighters é um jogo 3D desenvolvido em Unity/C#, com sistema de obtenção e coleção de pets por Gacha. A pesquisa acadêmica associada ao projeto descreve direção artística low poly com silhuetas exageradas, cores vibrantes e estética cute/kawaii. O guia artístico atual especifica a evolução dessa base para **Cute Stylized Low Poly Fantasy / Cartoon Low Poly Híbrido**.

## 2. Leitura obrigatória (ordem)

1. `Docs/Art/VISUAL_IDENTITY.md` — identidade, princípios e critérios visuais.
2. `Docs/Art/CHARACTER_MODELING_RULES.md` — padrões de geometria, face, Blender e validação.
3. `Docs/Art/PET_CATALOG.md` — fichas e estados de aprovação; **não tratar exemplos como conteúdo canônico**.

Se qualquer arquivo estiver ausente, informe e não suponha seu conteúdo. Se houver conflito: solicitação explícita do usuário > decisões marcadas como APROVADAS nos docs > recomendações marcadas como PROPOSTAS > exemplos.

## 3. Limites de escopo

- Trabalhar apenas em **personagens, pets, inimigos comuns, inimigos especiais e bosses**, a menos que o usuário amplie o escopo.
- Não criar ou editar interface, HUD, menus, economia, taxas Gacha, combate, lógica de inventário ou mapa sem pedido específico.
- Não inventar espécies, nomes, raridades, stats ou catálogo oficial. Para personagem novo, pedir ficha ou declarar uma proposta de teste explicitamente.
- Não reestruturar pastas existentes, sobrescrever `.blend`, `.fbx`, cenas, prefabs ou scripts sem verificar o que existe e obter autorização quando houver risco.
- Não instalar addons, executar downloads ou ligar serviços pagos sem aprovação do usuário.

## 4. Fluxo operacional de modelagem

1. Verifique o estado do repositório e identifique caminho real da pasta de arte (não presuma que a estrutura sugerida já existe).
2. Leia os três documentos citados e a ficha individual aprovada para o personagem.
3. Verifique a disponibilidade do **MCP do Blender**; se indisponível, relate o problema. Não alegue ter criado arquivo no Blender sem ter executado e verificado.
4. Crie um plano curto: arquétipo, silhouette/blockout, peças, paleta, face e validação.
5. Use `bpy` e funções reutilizáveis sempre que viável. Crie primeiro formas primárias, depois secundárias, detalhes faciais e materiais.
6. Salve fonte `.blend` com nome estável. Não destrua cenas/objetos preexistentes; use coleção exclusiva.
7. Gere vistas **frontal, lateral e 3/4** (ou capture viewport equivalente), verificando olhos, silhueta, base e interseções.
8. Compare com o checklist de `CHARACTER_MODELING_RULES.md`, identifique desvios e solicite revisão artística.
9. Somente após aprovação, preparar exportação para Unity e prefab, se solicitado. Integração Unity não está implícita na tarefa de modelagem.

## 5. Uso eficiente de ferramentas e contexto

- Prefira **uma operação planejada de construção** e **uma rodada de inspeção** a muitos ajustes experimentais isolados.
- Utilize scripts parametrizados para variações de corpos, olhos, orelhas, materiais e detalhes.
- Evite gerar grandes logs, inspeções redundantes e múltiplas renders sem propósito.
- Nunca afirme que um script executou ou que um asset foi salvo sem verificar saída real.
- Evite processamento caro não solicitado (simulação, subdivisão alta, renderização final complexa).

## 6. Convenções iniciais

- Categorias de arquivo: `PET_`, `ENM_`, `ELITE_`, `BOSS_`.
- Cada criatura deve ter um identificador único e coleção/objeto raiz com o mesmo identificador.
- Use nomes de objetos descritivos (`Body`, `Eye_L`, `Eye_R`, `Ear_L`, `Ear_R`, etc.).
- Blender: Z para cima; X para lados; frente do modelo direcionada a **-Y** no arquivo de trabalho. Exportação deve validar conversão de eixos na Unity.
- Origem do personagem: centro da base de apoio, salvo ficha justificando voo/flutuação.
- Preservar arquivos-fonte `.blend`; exportação em FBX somente após validação.

## 7. Formato esperado da entrega do agente

Ao terminar, informe: (a) personagem e ficha usada; (b) arquivos criados/alterados com caminhos reais; (c) validações realizadas e suas evidências; (d) divergências ou limitações; (e) próximos passos necessários. Separe sempre **executado**, **proposto** e **pendente**.

## 8. Segurança e aprovação

Não executar comandos destrutivos, baixar binários desconhecidos, publicar arquivos, consumir APIs pagas ou alterar assets existentes sem consentimento. Se o MCP não responder, interrompa ações dependentes dele e explique o diagnóstico.
