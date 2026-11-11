import bpy, math
from mathutils import Vector

OUT=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Golfin_2026'
scene=bpy.data.scenes.new('Water_Golfin_Studio');bpy.context.window.scene=scene
scene.unit_settings.system='METRIC'
coll=bpy.data.collections.new('PET_Water_Golfin');scene.collection.children.link(coll)
studio=bpy.data.collections.new('Water_Golfin_Studio_Luzes');scene.collection.children.link(studio)
root=bpy.data.objects.new('PET_Water_Golfin',None);coll.objects.link(root)
def mat(name,c,rough=.38):
    m=bpy.data.materials.new(name);m.diffuse_color=(*c,1);m.use_nodes=True
    p=next(n for n in m.node_tree.nodes if n.type=='BSDF_PRINCIPLED');p.inputs['Base Color'].default_value=(*c,1);p.inputs['Roughness'].default_value=rough
    return m
blue=mat('Azul Golfin',(.018,.37,.85));navy=mat('Olhos azul marinho',(.003,.014,.045),.2);pink=mat('Rosa bochechas e lingua',(1,.30,.44));white=mat('Branco reflexos',(1,1,1),.22);mouthmat=mat('Interior sorriso',(.29,.01,.045));skin=mat('Corpo azul com identidade pintada',(.02,.38,.85))
def mesh(name,vs,fs,mats):
    me=bpy.data.meshes.new(name);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(name,me);coll.objects.link(o);o.parent=root
    for m in mats:me.materials.append(m)
    for p in me.polygons:p.use_smooth=True
    return o
# A single continuous body with a smoothly blended forward muzzle deformation.
N=48;M=38;vs=[];fs=[]
for j in range(M+1):
    t=-1+2*j/M;r=math.sqrt(max(0,1-t*t));taper=1-.18*t
    for i in range(N):
        a=2*math.pi*i/N;x=.78*r*taper*math.cos(a);z=1.10+.76*r*taper*math.sin(a)
        bulge=.32*math.exp(-((x/.38)**4+((z-1.07)/.16)**4))*max(0,-t)**2
        y=1.05*t-bulge
        vs.append((x,y,z))
for j in range(M):
    for i in range(N):fs.append((j*N+i,j*N+(i+1)%N,(j+1)*N+(i+1)%N,(j+1)*N+i))
body=mesh('Corpo e focinho integrados',vs,fs,[skin]);uv=body.data.uv_layers.new(name='UV_corpo')
for j in range(M):
    for i in range(N):
        p=body.data.polygons[j*N+i]
        for li,co in zip(p.loop_indices,[(i/N,j/M),((i+1)/N,j/M),((i+1)/N,(j+1)/M),(i/N,(j+1)/M)]):uv.data[li].uv=co
# Packed body colour texture: white underside, two side spirals and forehead diamond.
spiral=[]
for k in range(49):
    t=k/48;a=t*math.pi*2.05;r=.20*(1-t)+.018
    spiral.append((.20+r*math.cos(a),1.32+r*math.sin(a)))
def segdist(x,y,ax,ay,bx,by):
    dx=bx-ax;dy=by-ay;den=dx*dx+dy*dy
    h=max(0,min(1,((x-ax)*dx+(y-ay)*dy)/den)) if den else 0
    return math.sqrt((x-ax-h*dx)**2+(y-ay-h*dy)**2)
im=bpy.data.images.new('Water_Golfin_cor_e_simbolos',width=768,height=768,alpha=True);pixels=[]
for j in range(768):
    t=j/767*2-1;rad=math.sqrt(max(0,1-t*t));fac=1-.18*t;y=1.05*t
    for i in range(768):
        a=2*math.pi*i/767;x=.78*rad*fac*math.cos(a);z=1.10+.76*rad*fac*math.sin(a)
        c=(.035,.40,.91)
        boundary=.94+.025*math.cos(t*math.pi)
        w=max(0,min(1,(boundary-z)/.014));c=tuple(c[k]*(1-w)+(0.96,0.97,1)[k]*w for k in range(3))
        if abs(x)>.40 and abs(y-.20)<.27 and abs(z-1.32)<.27:
            dist=min(segdist(y,z,*spiral[k],*spiral[k+1]) for k in range(48))
            if dist<.048:c=(.57,.86,1)
        # Tiny faceted water mark painted on the upper front of the forehead.
        if t<-.48 and abs(x)<.135 and 1.48<z<1.75:
            q=abs(x)/.135+abs(z-1.615)/.135
            if q<1:
                c=(.10,.89,.99) if x<0 else (.28,.97,1)
                if q>.88:c=(.70,1,1)
        pixels.extend((*c,1))
im.pixels.foreach_set(pixels);im.pack();nodes=skin.node_tree.nodes;tex=nodes.new('ShaderNodeTexImage');tex.image=im
shader=next(n for n in nodes if n.type=='BSDF_PRINCIPLED');skin.node_tree.links.new(tex.outputs['Color'],shader.inputs['Base Color'])
def oval(name,loc,scale,m,seg=20,rings=12,rot=(0,0,0)):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=seg,ring_count=rings,location=loc);o=bpy.context.object
    for c in list(o.users_collection):c.objects.unlink(o)
    coll.objects.link(o);o.name=name;o.parent=root;o.scale=scale;o.rotation_euler=rot;o.data.materials.append(m)
    for p in o.data.polygons:p.use_smooth=True
    return o
# Smooth closed volumetric fin from a sampled Bezier outline and nested shells.
def cubic(p0,p1,p2,p3,t):
    s=1-t;return tuple(s**3*p0[i]+3*s*s*t*p1[i]+3*s*t*t*p2[i]+t**3*p3[i] for i in range(2))
def outline(segs,steps=7):
    return [cubic(*seg,i/steps) for seg in segs for i in range(steps)]
def fin(name,points,center,U,V,normal,depth):
    n=len(points);center=Vector(center);U=Vector(U);V=Vector(V);normal=Vector(normal);verts=[];faces=[]
    for sign in (-1,1):
        for f,h in [(1,0),(.72,.72),(.25,1)]:
            for x,z in points:verts.append(tuple(center+U*(f*x)+V*(f*z)+normal*(sign*h*depth)))
    for side in range(2):
        b=side*3*n
        for r in range(2):
            for i in range(n):
                q=(b+r*n+i,b+r*n+(i+1)%n,b+(r+1)*n+(i+1)%n,b+(r+1)*n+i);faces.append(q if side else tuple(reversed(q)))
        faces.append(tuple(b+2*n+i for i in (range(n) if side else reversed(range(n)))))
    # Both edge loops are intentionally welded after creation.
    for i in range(n):faces.append((i,(i+1)%n,3*n+(i+1)%n,3*n+i))
    obj=mesh(name,verts,faces,[blue])
    import bmesh
    bm=bmesh.new();bm.from_mesh(obj.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(obj.data);bm.free();obj.data.update()
    return obj
dorsal=outline([((-.50,-.18),(-.40,.16),(-.10,.53),(.20,.56)),((.20,.56),(.45,.60),(.70,.48),(.63,.34)),((.63,.34),(.43,.20),(.32,.08),(.30,-.18)),((.30,-.18),(.08,-.23),(-.22,-.23),(-.50,-.18))])
fin('Nadadeira dorsal',dorsal,(0,.20,1.72),(0,1,0),(0,0,1),(1,0,0),.115)
flipper=outline([((-.21,.15),(-.35,.07),(-.33,-.06),(-.20,-.17)),((-.20,-.17),(-.03,-.32),(.29,-.40),(.36,-.31)),((.36,-.31),(.40,-.21),(.29,.07),(.14,.16)),((.14,.16),(.03,.22),(-.12,.22),(-.21,.15))])
for s,name in [(-1,'direita'),(1,'esquerda')]:
    fin('Nadadeira peitoral '+name,flipper,(s*.66,.01,.88),(s*.58,.82,0),(0,0,1),(.82,-s*.58,0),.10)
# Horizontal flukes lie in the XY plane, not the vertical fin plane.
tail=outline([((0,-.28),(-.21,-.25),(-.45,-.10),(-.62,.10)),((-.62,.10),(-.74,.28),(-.67,.42),(-.51,.38)),((-.51,.38),(-.28,.32),(-.16,.20),(0,.15)),((0,.15),(.16,.20),(.28,.32),(.51,.38)),((.51,.38),(.67,.42),(.74,.28),(.62,.10)),((.62,.10),(.45,-.10),(.21,-.25),(0,-.28))],5)
fin('Cauda horizontal',tail,(0,1.14,1.03),(1,0,0),(0,1,0),(0,0,1),.105)
# Ray cast onto the actual deformed body for attachment of facial elements.
bpy.context.view_layer.update()
def surface(x,z):
    ok,loc,n,idx=body.ray_cast(Vector((x,-3,z)),Vector((0,1,0)))
    return loc.y if ok else -.9
for s,name in [(-1,'direita'),(1,'esquerda')]:
    x=s*.34;z=1.34;y=surface(x,z)
    oval('Olho '+name,(x,y-.025,z),(.155,.055,.22),navy,20,14,(0,0,s*.35))
    oval('Reflexo '+name,(x-s*.035,y-.078,1.445),(.053,.026,.060),white,14,8)
    x=s*.51;z=1.01;y=surface(x,z)
    oval('Bochecha '+name,(x,y-.02,z),(.105,.035,.062),pink,16,10,(0,0,s*.45))
    x=s*.34;z=1.62;y=surface(x,z)
    oval('Sobrancelha '+name,(x,y-.023,z),(.085,.03,.025),blue,16,8,(0,0,s*.35))
outline=[(-.22,1.005),(-.16,.982),(-.08,.97),(0,.965),(.08,.97),(.16,.982),(.22,1.005),(.21,.93),(.18,.86),(.13,.80),(.07,.77),(0,.76),(-.07,.77),(-.13,.80),(-.18,.86),(-.21,.93)]
v=[(x,surface(x,z)-.014,z) for x,z in outline]+[(0,surface(0,.87)-.012,.87)]
faces=[(i,(i+1)%len(outline),len(outline)) for i in range(len(outline))]
mesh('Sorriso interior',v,faces,[mouthmat]);oval('Lingua',(0,surface(0,.805)-.035,.805),(.115,.025,.045),pink,18,10)
# Weld body poles without removing its UV layer.
import bmesh
bm=bmesh.new();bm.from_mesh(body.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(body.data);bm.free();body.data.update()
world=bpy.data.worlds.new('Fundo claro');world.use_nodes=True;bg=next(n for n in world.node_tree.nodes if n.type=='BACKGROUND');bg.inputs['Color'].default_value=(.80,.78,.79,1);bg.inputs['Strength'].default_value=.55;scene.world=world
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
for name,pos,power,size in [('Principal',(-3,-4,5),400,4),('Preenchimento',(4,-3,3),160,4),('Contorno',(1,4,5),300,3)]:
    d=bpy.data.lights.new(name,'AREA');d.energy=power;d.shape='DISK';d.size=size;o=bpy.data.objects.new(name,d);studio.objects.link(o);o.location=pos;aim(o,(0,0,1))
cd=bpy.data.cameras.new('Camera de revisao');cam=bpy.data.objects.new('Camera de revisao',cd);studio.objects.link(cam);scene.camera=cam;cd.type='ORTHO';cd.ortho_scale=3.25
scene.render.engine='CYCLES';scene.cycles.samples=32;scene.cycles.use_denoising=True;scene.render.resolution_x=768;scene.render.resolution_y=768;scene.render.resolution_percentage=100;scene.render.image_settings.file_format='PNG'
scene.view_settings.view_transform='Standard';scene.view_settings.exposure=-.7
views=[('frente',(0,-7,1.1)),('lado_esquerdo',(7,0,1.1)),('lado_direito',(-7,0,1.1)),('tras',(0,7,1.1)),('topo',(0,0,8)),('tres_quartos_frente',(5,-7,3.5)),('tres_quartos_tras',(5,7,3.5))]
dg=bpy.context.evaluated_depsgraph_get();tri=0
for o in coll.objects:
    if o.type=='MESH':
        e=o.evaluated_get(dg);m=e.to_mesh();m.calc_loop_triangles();tri+=len(m.loop_triangles);e.to_mesh_clear()
print('STATS triangles',tri,'meshes',sum(o.type=='MESH' for o in coll.objects),'objects',len(coll.objects))
for name,pos in views:
    cam.location=pos;aim(cam,(0,.05,1.10));scene.render.filepath=OUT+'\\'+name+'.png';bpy.ops.render.render(write_still=True);print('RENDER',name)
cam.location=views[5][1];aim(cam,(0,.05,1.1));bpy.ops.wm.save_as_mainfile(filepath=OUT+'\\Water_Golfin.blend');print('SAVED',OUT+'\\Water_Golfin.blend')
