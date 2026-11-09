import bpy, math
from mathutils import Vector
OUT = r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Blender\Pets\Water_Peixinho_Novo'
scene = bpy.data.scenes.new('Water_Peixinho_Studio')
bpy.context.window.scene = scene
scene.unit_settings.system = 'METRIC'
coll = bpy.data.collections.new('Water_Peixinho')
scene.collection.children.link(coll)
root = bpy.data.objects.new('Water_Peixinho_ROOT', None); coll.objects.link(root)
def mat(name, color, rough=.38):
    m=bpy.data.materials.new(name); m.diffuse_color=(*color,1); m.use_nodes=True
    s=next(n for n in m.node_tree.nodes if n.type=='BSDF_PRINCIPLED'); s.inputs['Base Color'].default_value=(*color,1); s.inputs['Roughness'].default_value=rough
    return m
sky=mat('Face azul claro',(.19,.64,.97)); blue=mat('Azul intenso',(.008,.25,.83)); white=mat('Branco suave',(.91,.95,1)); yellow=mat('Amarelo', (1,.67,.025)); eye=mat('Olhos marinho',(.004,.015,.065),.16); pink=mat('Rosa', (1,.34,.48)); mouth=mat('Boca',(.27,.012,.045)); shine=mat('Brilho',(1,1,1),.18); iris=mat('Reflexo azul',(.008,.12,.34),.2)
def mesh(name,vs,fs,materials,indices=None):
    me=bpy.data.meshes.new(name); me.from_pydata(vs,[],fs); me.update(); ob=bpy.data.objects.new(name,me); coll.objects.link(ob); ob.parent=root
    for m in materials: me.materials.append(m)
    for i,p in enumerate(me.polygons):
        p.use_smooth=True
        if indices: p.material_index=indices[i]
    return ob
def ellipsoid(name, loc, scale, material, seg=20, rings=12, rot=None):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=seg,ring_count=rings,location=loc)
    ob=bpy.context.object
    for c in list(ob.users_collection): c.objects.unlink(ob)
    coll.objects.link(ob); ob.name=name; ob.scale=scale; ob.parent=root; ob.data.materials.append(material)
    if rot: ob.rotation_euler=rot
    for p in ob.data.polygons:p.use_smooth=True
    return ob
# -Y faces forward; body rings wrap continuously and taper toward tail.
ts=sorted(set([-1,1,-.17,.02,.43,.59]+[i/20 for i in range(-19,20)])); N=40; vs=[]; fs=[]; ids=[]
for t in ts:
    r=math.sqrt(max(.00001,1-t*t)); taper=1-.16*t
    for i in range(N):
        a=2*math.pi*i/N
        vs.append((.78*r*taper*math.cos(a),1.0*t,1.02+.77*r*taper*math.sin(a)))
for j in range(len(ts)-1):
    t=(ts[j]+ts[j+1])/2
    for i in range(N):
        fs.append((j*N+i,j*N+(i+1)%N,(j+1)*N+(i+1)%N,(j+1)*N+i))
        underside=sum(vs[k][2] for k in fs[-1])/4 < .66
        ids.append(2 if underside or -.17<t<.02 or .43<t<.59 else (0 if t<-.17 else 1))
body=mesh('Corpo continuo com faixas',vs,fs,[sky,blue,white],ids)
# Rounded, thick fan as nested contour rings; custom silhouettes, not discs.
def fin(name, outline, center, axis, depth):
    # 2D coordinates mapped into plane; inner rings are visibly convex.
    n=len(outline); v=[]; f=[]; mi=[]
    for side in (-1,1):
        for frac,offset in [(1,0),(.78,depth*.68),(.20,depth)]:
            for u,w in outline:
                p=Vector(center)+Vector(axis[0])*(u*frac)+Vector(axis[1])*(w*frac)+Vector(axis[2])*(side*offset)
                v.append(tuple(p))
    for side in range(2):
        base=side*3*n
        for ring in range(2):
            for i in range(n):
                a=base+ring*n+i;b=base+ring*n+(i+1)%n;c=base+(ring+1)*n+(i+1)%n;d=base+(ring+1)*n+i
                f.append((a,b,c,d) if side else (d,c,b,a));mi.append(0 if ring==0 else 1)
        f.append(tuple(base+2*n+i for i in (range(n) if side else reversed(range(n)))));mi.append(1)
    for i in range(n):f.append((i,(i+1)%n,3*n+(i+1)%n,3*n+i));mi.append(0)
    return mesh(name,v,f,[blue,yellow],mi)
fan=[(-.48,-.35),(-.53,-.12),(-.50,.15),(-.39,.37),(-.20,.50),(.03,.54),(.25,.48),(.40,.32),(.49,.12),(.50,-.10),(.40,-.30),(.22,-.39),(0,-.38),(-.24,-.36)]
fin('Dorsal leque',fan,(0,.05,1.73),((0,1,0),(0,0,1),(1,0,0)),.105)
tail=[(-.44,-.12),(-.32,-.31),(-.10,-.49),(.13,-.53),(.31,-.46),(.43,-.27),(.47,0),(.43,.27),(.31,.46),(.13,.53),(-.10,.49),(-.32,.31)]
fin('Cauda leque',tail,(0,1.22,1.02),((0,1,0),(0,0,1),(1,0,0)),.12)
for s,label in [(-1,'D'),(1,'E')]:
    smallfan=[(u*.62,w*.62) for u,w in fan]
    fin('Peitoral '+label,smallfan,(s*.75,-.04,1.0),((s*.42,.907,0),(0,0,1),(.907,-s*.42,0)),.09)
    ellipsoid('Nadadeira inferior '+label,(s*.39,-.03,.30),(.16,.26,.105),blue,18,10,rot=(0,s*.16,s*.32))
    ellipsoid('Olho '+label,(s*.34,-.892,1.20),(.175,.075,.255),eye,24,16,rot=(0,0,s*.19))
    ellipsoid('Reflexo '+label,(s*.38,-.965,1.31),(.055,.022,.068),shine,16,8)
    ellipsoid('Reflexo inferior '+label,(s*.34,-.958,1.07),(.09,.012,.045),iris,16,8)
    ellipsoid('Bochecha '+label,(s*.53,-.76,.91),(.105,.026,.054),pink,16,8,rot=(0,0,s*.38))
    ellipsoid('Sobrancelha '+label,(s*.34,-.833,1.52),(.095,.021,.025),blue,16,8,rot=(0,0,s*.13))
# Smile shaped as a shallow, closed rounded aperture, inset into the front surface.
outline=[(-.19,.97),(-.13,.94),(0,.93),(.13,.94),(.19,.97),(.17,.88),(.13,.81),(.07,.77),(0,.75),(-.07,.77),(-.13,.81),(-.17,.88)]
v=[(x,y,z) for y in (-.975,-.945) for x,z in outline]; n=len(outline)
f=[tuple(reversed(range(n))),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
smile=mesh('Sorriso',v,f,[mouth]); mod=smile.modifiers.new('Borda suave','BEVEL'); mod.width=.012;mod.segments=2
ellipsoid('Lingua',(0,-.99,.795),(.105,.025,.046),pink,20,10)
# Isolated studio: pre-existing scene is untouched.
studio=bpy.data.collections.new('Water_Peixinho_Iluminacao');scene.collection.children.link(studio)
world=bpy.data.worlds.new('Fundo cinza claro');world.use_nodes=True;world.node_tree.nodes.get('Background').inputs['Color'].default_value=(.72,.72,.72,1);world.node_tree.nodes.get('Background').inputs['Strength'].default_value=.8;scene.world=world
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
for name,loc,power,size in [('Principal',(-3,-4,5),450,4),('Preenchimento',(4,-3,3),220,3),('Contorno',(1,4,5),400,3)]:
    d=bpy.data.lights.new(name,'AREA');d.energy=power;d.shape='DISK';d.size=size;o=bpy.data.objects.new(name,d);studio.objects.link(o);o.location=loc;aim(o,(0,0,1))
camd=bpy.data.cameras.new('Camera revisao');cam=bpy.data.objects.new('Camera revisao',camd);studio.objects.link(cam);scene.camera=cam;camd.type='ORTHO';camd.ortho_scale=3.2
scene.render.engine='BLENDER_EEVEE_NEXT';scene.render.resolution_x=768;scene.render.resolution_y=768;scene.render.resolution_percentage=100;scene.render.image_settings.file_format='PNG'
scene.render.film_transparent=False
views=[('frente',(0,-7,1.1)),('lateral',(7,0,1.1)),('tras',(0,7,1.1)),('topo',(0,0,8)),('tres_quartos_frente',(5,-7,3.3)),('tres_quartos_tras',(5,7,3.3))]
deps=bpy.context.evaluated_depsgraph_get(); tris=0
for o in coll.objects:
    if o.type=='MESH':
        e=o.evaluated_get(deps); m=e.to_mesh();m.calc_loop_triangles();tris+=len(m.loop_triangles);e.to_mesh_clear()
print('TRIANGLES',tris)
for name,pos in views:
    cam.location=pos;cam.data.ortho_scale=3.7 if name=='topo' else 3.2
    aim(cam,(0,.3,1.1) if name=='topo' else (0,0,1.1));scene.render.filepath=OUT+'\\'+name+'.png';bpy.ops.render.render(write_still=True);print('RENDER',name)
cam.data.ortho_scale=3.2
cam.location=views[4][1];aim(cam,(0,0,1.1));bpy.ops.wm.save_as_mainfile(filepath=OUT+'\\Water_Peixinho.blend')
print('SAVED',OUT+'\\Water_Peixinho.blend')
