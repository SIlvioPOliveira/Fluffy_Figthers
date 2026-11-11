import bpy, math
from mathutils import Vector

# New scene only. Forward = -Y, up = +Z; 1 Blender unit = 1 metre.
OUT = r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Peixinho_Novo_2026'
scene=bpy.data.scenes.new('Water_Peixinho_Novo')
bpy.context.window.scene=scene
scene.unit_settings.system='METRIC'
coll=bpy.data.collections.new('Water_Peixinho'); scene.collection.children.link(coll)
studio=bpy.data.collections.new('Estudio_Revisao');scene.collection.children.link(studio)
root=bpy.data.objects.new('Water_Peixinho',None);coll.objects.link(root)
def material(name,color,rough=.38):
    m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True
    p=next(n for n in m.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
    p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=rough
    return m
blue=material('Azul das nadadeiras',(.012,.26,.86)); yellow=material('Amarelo das nadadeiras',(1,.68,.018));
dark=material('Olhos azul marinho',(.004,.013,.048),.19); pink=material('Rosa bochechas e lingua',(1,.29,.41));
white=material('Branco reflexo',(.98,.98,1),.22); mouth=material('Interior da boca',(.30,.014,.045));
bodymat=material('Corpo - textura embutida',(.3,.65,.96))
# Texture represents clean continuous paint, avoiding geometric overlay bands.
W=256;H=256
im=bpy.data.images.new('Water_Peixinho_cores',width=W,height=H,alpha=True)
px=[]
for iy in range(H):
    t=(iy/(H-1))*2-1
    radius=math.sqrt(max(0,1-t*t))
    for ix in range(W):
        a=2*math.pi*ix/(W-1); z=1.04+.75*radius*(1-.12*t)*math.sin(a)
        col=(.59,.82,1) if t<-.12 else (.03,.38,.93)
        if -.12<=t<=.09 or .53<=t<=.67:col=(.96,.97,1)
        if z<.79:col=(.95,.96,1)
        px.extend((*col,1))
im.pixels.foreach_set(px);im.pack()
nodes=bodymat.node_tree.nodes;links=bodymat.node_tree.links
tex=nodes.new('ShaderNodeTexImage');tex.image=im;tex.interpolation='Linear'
bs=next(n for n in nodes if n.type=='BSDF_PRINCIPLED');links.new(tex.outputs['Color'],bs.inputs['Base Color'])
def custom(name,verts,faces,mats,ids=None):
    me=bpy.data.meshes.new(name);me.from_pydata(verts,[],faces);me.update()
    o=bpy.data.objects.new(name,me);coll.objects.link(o);o.parent=root
    for m in mats:me.materials.append(m)
    for i,p in enumerate(me.polygons):p.use_smooth=True;p.material_index=ids[i] if ids else 0
    return o
# Transverse rings make the plump front continuous with a gently tapered back.
N=40; M=30; verts=[];faces=[]
for j in range(M+1):
    t=-1+2*j/M;r=math.sqrt(max(.0000001,1-t*t));fac=1-.12*t
    for i in range(N):
        a=2*math.pi*i/N
        verts.append((.76*r*fac*math.cos(a),.96*t,1.04+.75*r*fac*math.sin(a)))
for j in range(M):
    for i in range(N):faces.append((j*N+i,j*N+(i+1)%N,(j+1)*N+(i+1)%N,(j+1)*N+i))
body=custom('Corpo',verts,faces,[bodymat]);uv=body.data.uv_layers.new(name='UV Cores')
for j in range(M):
    for i in range(N):
        poly=body.data.polygons[j*N+i]
        for loop_idx,coord in zip(poly.loop_indices,[(i/N,j/M),((i+1)/N,j/M),((i+1)/N,(j+1)/M),(i/N,(j+1)/M)]):uv.data[loop_idx].uv=coord
def oval(name,loc,scale,mat,seg=18,rings=12,rot=(0,0,0)):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=seg,ring_count=rings,location=loc)
    ob=bpy.context.object
    for c in list(ob.users_collection):c.objects.unlink(ob)
    coll.objects.link(ob);ob.name=name;ob.scale=scale;ob.rotation_euler=rot;ob.parent=root;ob.data.materials.append(mat)
    for p in ob.data.polygons:p.use_smooth=True
    return ob
# Closed softly convex fans. Blue and yellow share the same continuous mesh.
def fan(name,center,U,V,normal,ru,rv,depth,lobes=3):
    count=32; verts=[];faces=[];indices=[];center=Vector(center);U=Vector(U);V=Vector(V);normal=Vector(normal)
    for side in (-1,1):
        for r,z in [(1,0),(.91,.065),(.73,.11),(.42,.145),(.025,.16)]:
            for i in range(count):
                a=2*math.pi*i/count
                # Low-amplitude scallops keep the edges rounded.
                scallop=1+.035*math.cos(lobes*a)
                p=center+U*(ru*r*math.cos(a)*scallop)+V*(rv*r*math.sin(a)*scallop)+normal*(side*z*depth/.16)
                verts.append(tuple(p))
    for side in range(2):
        base=side*5*count
        for ring in range(4):
            for i in range(count):
                a=base+ring*count+i;b=base+ring*count+(i+1)%count;c=base+(ring+1)*count+(i+1)%count;d=base+(ring+1)*count+i
                faces.append((a,b,c,d) if side else (d,c,b,a));indices.append(0 if ring<2 else 1)
        faces.append(tuple(base+4*count+i for i in (range(count) if side else reversed(range(count)))));indices.append(1)
    for i in range(count):faces.append((i,(i+1)%count,5*count+(i+1)%count,5*count+i));indices.append(0)
    return custom(name,verts,faces,[blue,yellow],indices)
fan('Dorsal',(0,.10,1.72),(0,1,0),(0,0,1),(1,0,0),.63,.51,.12)
fan('Cauda',(0,1.19,1.06),(0,1,0),(0,0,1),(1,0,0),.39,.49,.13)
for s,label in [(-1,'direita'),(1,'esquerda')]:
    fan('Peitoral '+label,(s*.67,-.03,1.02),(s*.30,.954,0),(0,0,1),(.954,-s*.30,0),.30,.31,.10)
    oval('Ventral '+label,(s*.40,-.03,.32),(.16,.26,.11),blue,16,10,(0,s*.15,s*.28))
# Front surface mapping: details conform to body rather than float on its face.
def fy(x,z):
    q=(x/.81)**2+((z-1.04)/.80)**2
    return -.96*math.sqrt(max(.05,1-q))
for s,label in [(-1,'direita'),(1,'esquerda')]:
    x=s*.33; z=1.23; y=fy(x,z)
    oval('Olho '+label,(x,y-.012,z),(.165,.055,.235),dark,20,14,(0,0,s*.40))
    oval('Brilho '+label,(x-s*.035,y-.073,1.335),(.055,.027,.063),white,12,8)
    x=s*.50;z=.92;y=fy(x,z)
    oval('Bochecha '+label,(x,y-.012,z),(.115,.035,.065),pink,16,10,(0,0,s*.55))
    x=s*.34;z=1.54;y=fy(x,z)
    oval('Sobrancelha '+label,(x,y-.015,z),(.102,.038,.033),blue,16,8,(0,0,s*.36))
# Concave-looking mouth surface: inset center and mouth border follow the body.
outline=[(-.185,.995),(-.14,.975),(-.07,.966),(0,.964),(.07,.966),(.14,.975),(.185,.995),(.18,.93),(.16,.86),(.12,.81),(.06,.78),(0,.775),(-.06,.78),(-.12,.81),(-.16,.86),(-.18,.93)]
verts=[(x,fy(x,z)-.017,z) for x,z in outline]+[(0,fy(0,.88)-.010,.88)]
faces=[(i,(i+1)%len(outline),len(outline)) for i in range(len(outline))]
custom('Interior sorriso',verts,faces,[mouth])
oval('Lingua',(0,fy(0,.805)-.035,.805),(.105,.022,.043),pink,16,10)
# Isolated neutral studio.
world=bpy.data.worlds.new('Fundo neutro');world.use_nodes=True
bg=next(n for n in world.node_tree.nodes if n.type=='BACKGROUND');bg.inputs['Color'].default_value=(.77,.75,.76,1);bg.inputs['Strength'].default_value=.8;scene.world=world
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
for name,pos,power,size in [('Principal',(-3,-4,5),500,4),('Preenchimento',(4,-3,3),220,4),('Recorte',(1,4,4),380,3)]:
    d=bpy.data.lights.new(name,'AREA');d.energy=power;d.shape='DISK';d.size=size;o=bpy.data.objects.new(name,d);studio.objects.link(o);o.location=pos;aim(o,(0,0,1))
cd=bpy.data.cameras.new('Camera revisao');cam=bpy.data.objects.new('Camera revisao',cd);studio.objects.link(cam);scene.camera=cam;cd.type='ORTHO';cd.ortho_scale=3.15
scene.render.engine='CYCLES';scene.cycles.samples=32;scene.cycles.use_denoising=True
scene.render.resolution_x=768;scene.render.resolution_y=768;scene.render.resolution_percentage=100;scene.render.image_settings.file_format='PNG'
scene.view_settings.view_transform='AgX'
views=[('frente',(0,-7,1.05)),('lado',(7,0,1.05)),('costas',(0,7,1.05)),('tres_quartos_frente',(5,-7,3.2)),('tres_quartos_costas',(5,7,3.2))]
dg=bpy.context.evaluated_depsgraph_get();tri=0
for o in coll.objects:
    if o.type=='MESH':
        e=o.evaluated_get(dg);m=e.to_mesh();m.calc_loop_triangles();tri+=len(m.loop_triangles);e.to_mesh_clear()
print('MODEL_STATS triangles=',tri,'mesh_objects=',sum(o.type=='MESH' for o in coll.objects),'materials=',len({m.name for o in coll.objects if o.type=='MESH' for m in o.data.materials if m}))
for name,pos in views:
    cam.location=pos;aim(cam,(0,.1,1.08));scene.render.filepath=OUT+'\\'+name+'.png';bpy.ops.render.render(write_still=True);print('VIEW',name)
cam.location=views[3][1];aim(cam,(0,.1,1.08));bpy.ops.wm.save_as_mainfile(filepath=OUT+'\\Water_Peixinho.blend')
print('SAVED',OUT+'\\Water_Peixinho.blend')
