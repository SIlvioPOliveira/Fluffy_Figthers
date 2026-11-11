import bpy,math,bmesh
from mathutils import Vector
scene=bpy.data.scenes['Water_Golfin_Studio'];bpy.context.window.scene=scene
body=bpy.data.objects['Corpo e focinho integrados'];old=body.data
N=48;M=38;vs=[];fs=[]
for j in range(M+1):
    t=-1+2*j/M;r=math.sqrt(max(0,1-t*t));fac=1-.18*t
    for i in range(N):
        a=2*math.pi*i/N;x=.78*r*fac*math.cos(a);z=1.10+.76*r*fac*math.sin(a)
        bulge=.20*math.exp(-((x/.49)**2+((z-1.07)/.24)**2))*max(0,-t)**3
        vs.append((x,1.05*t-bulge,z))
for j in range(M):
    for i in range(N):fs.append((j*N+i,j*N+(i+1)%N,(j+1)*N+(i+1)%N,(j+1)*N+i))
me=bpy.data.meshes.new('Corpo focinho suavizado');me.from_pydata(vs,[],fs);me.update();me.materials.append(bpy.data.materials['Corpo azul com identidade pintada']);uv=me.uv_layers.new(name='UV_corpo')
for j in range(M):
    for i in range(N):
        p=me.polygons[j*N+i];p.use_smooth=True
        for li,co in zip(p.loop_indices,[(i/N,j/M),((i+1)/N,j/M),((i+1)/N,(j+1)/M),(i/N,(j+1)/M)]):uv.data[li].uv=co
bm=bmesh.new();bm.from_mesh(me);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free();me.update();body.data=me
bpy.context.view_layer.update()
def surface(x,z):
    ok,loc,n,idx=body.ray_cast(Vector((x,-3,z)),Vector((0,1,0)))
    return loc.y if ok else -.9
# Curved oval eye patches follow the face rather than being cut by it.
for s,name in [(-1,'direita'),(1,'esquerda')]:
    obj=bpy.data.objects['Olho '+name];n=32;verts=[];faces=[];cx=s*.34;cz=1.34
    for r in (1,.70,.35,.015):
        for i in range(n):
            a=2*math.pi*i/n;x=cx+.155*r*math.cos(a);z=cz+.22*r*math.sin(a)
            verts.append((x,surface(x,z)-.014-.008*(1-r*r),z))
    for ring in range(3):
        for i in range(n):faces.append((ring*n+i,ring*n+(i+1)%n,(ring+1)*n+(i+1)%n,(ring+1)*n+i))
    faces.append(tuple(range(3*n,4*n)))
    m=bpy.data.meshes.new('Olho adaptado '+name);m.from_pydata(verts,[],faces);m.materials.append(bpy.data.materials['Olhos azul marinho']);m.update()
    for p in m.polygons:p.use_smooth=True
    obj.data=m;obj.location=(0,0,0);obj.scale=(1,1,1);obj.rotation_euler=(0,0,0)
    ob=bpy.data.objects['Reflexo '+name];x=s*.34-s*.035;z=1.445;ob.location=(x,surface(x,z)-.04,z)
    ob=bpy.data.objects['Bochecha '+name];x=s*.51;z=1.01;ob.location=(x,surface(x,z)-.025,z)
    ob=bpy.data.objects['Sobrancelha '+name];x=s*.34;z=1.62;ob.location=(x,surface(x,z)-.025,z)
smile=bpy.data.objects['Sorriso interior']
for v in smile.data.vertices:v.co.y=surface(v.co.x,v.co.z)-.022
tongue=bpy.data.objects['Lingua'];tongue.location.y=surface(0,.805)-.045
# Lighten the blue paint while retaining painted white, swirl and forehead mark.
im=bpy.data.images['Water_Golfin_cor_e_simbolos'];pixels=list(im.pixels[:])
for i in range(0,len(pixels),4):
    if pixels[i]<.05 and .35<pixels[i+1]<.45 and pixels[i+2]>.85:
        pixels[i]=.08;pixels[i+1]=.51;pixels[i+2]=.95
im.pixels.foreach_set(pixels);im.pack()
blue=bpy.data.materials['Azul Golfin'];p=next(n for n in blue.node_tree.nodes if n.type=='BSDF_PRINCIPLED');p.inputs['Base Color'].default_value=(.04,.43,.92,1)
out=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Golfin_2026';cam=scene.camera
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
views=[('frente',(0,-7,1.1)),('lado_esquerdo',(7,0,1.1)),('lado_direito',(-7,0,1.1)),('tras',(0,7,1.1)),('topo',(0,0,8)),('tres_quartos_frente',(5,-7,3.5)),('tres_quartos_tras',(5,7,3.5))]
for name,pos in views:
    cam.location=pos;aim(cam,(0,.05,1.1));scene.render.filepath=out+'\\'+name+'_refinado.png';bpy.ops.render.render(write_still=True);print('RENDER',name)
cam.location=views[5][1];aim(cam,(0,.05,1.1));bpy.ops.wm.save_as_mainfile(filepath=out+'\\Water_Golfin_refinado.blend');print('SAVED REFINADO')
