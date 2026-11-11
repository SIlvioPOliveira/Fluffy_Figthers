import bpy, math
from mathutils import Vector
scene=bpy.data.scenes['Water_Peixinho_Novo'];bpy.context.window.scene=scene
coll=bpy.data.collections['Water_Peixinho']
for s,label in [(-1,'direita'),(1,'esquerda')]:
    obj=bpy.data.objects['Peitoral '+label]
    for v in obj.data.vertices:
        x,y,z=v.co;u=s*.30*(x-s*.67)+.954*(y+.03);d=.954*(x-s*.67)-s*.30*(y+.03)
        v.co=(s*.83-s*.30*u+.954*d,-.09+.954*u+s*.30*d,z)
# High-resolution embedded texture with antialiased white belly edge.
im=bpy.data.images['Water_Peixinho_cores'];im.scale(1024,1024);px=[]
for iy in range(1024):
    t=(iy/1023)*2-1;radius=math.sqrt(max(0,1-t*t))
    for ix in range(1024):
        a=2*math.pi*ix/1023;z=1.04+.75*radius*(1-.12*t)*math.sin(a)
        color=(.43,.75,1) if t<-.12 else (.015,.28,.90)
        if -.12<=t<=.09 or .53<=t<=.67:color=(.96,.97,1)
        w=max(0,min(1,(.795-z)/.012))
        px.extend((color[0]*(1-w)+.95*w,color[1]*(1-w)+.96*w,color[2]*(1-w)+w,1))
im.pixels.foreach_set(px);im.pack()
# Subtle painted radial grooves, mapped separately on each continuous fin mesh.
finim=bpy.data.images.new('Sulcos amarelos',width=512,height=512,alpha=True);px=[]
for j in range(512):
    v=(j/511)*2-1
    for i in range(512):
        u=(i/511)*2-1;theta=math.atan2(v,u+.85);dist=math.sqrt((u+.85)**2+v*v)
        line=min(abs(theta),abs(theta-.48),abs(theta+.48))
        amount=max(0,1-line/.012)*(.18 if dist>.3 else 0)
        px.extend((1,.68-.20*amount,.018,1))
finim.pixels.foreach_set(px);finim.pack()
ymat=bpy.data.materials['Amarelo das nadadeiras'];nodes=ymat.node_tree.nodes
tex=nodes.new('ShaderNodeTexImage');tex.image=finim
shader=next(n for n in nodes if n.type=='BSDF_PRINCIPLED');ymat.node_tree.links.new(tex.outputs['Color'],shader.inputs['Base Color'])
specs=[('Dorsal',(0,.10,1.72),(0,1,0),(0,0,1),.63,.51),('Cauda',(0,1.19,1.06),(0,1,0),(0,0,1),.39,.49)]
for s,label in [(-1,'direita'),(1,'esquerda')]:specs.append(('Peitoral '+label,(s*.83,-.09,1.02),(-s*.30,.954,0),(0,0,1),.30,.31))
for name,center,u,v,ru,rv in specs:
    obj=bpy.data.objects[name];layer=obj.data.uv_layers.new(name='UV sulcos');center=Vector(center);u=Vector(u);v=Vector(v)
    for p in obj.data.polygons:
        for li in p.loop_indices:
            point=obj.data.vertices[obj.data.loops[li].vertex_index].co-center
            layer.data[li].uv=(.5+.5*point.dot(u)/ru,.5+.5*point.dot(v)/rv)
scene.view_settings.view_transform='Standard'
cam=scene.camera
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
out=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Peixinho_Novo_2026'
views=[('frente',(0,-7,1.05)),('lado',(7,0,1.05)),('costas',(0,7,1.05)),('tres_quartos_frente',(5,-7,3.2)),('tres_quartos_costas',(5,7,3.2))]
for name,pos in views:
    cam.location=pos;aim(cam,(0,.1,1.08));scene.render.filepath=out+'\\'+name+'_refinado.png';bpy.ops.render.render(write_still=True);print('VIEW',name)
cam.location=views[3][1];aim(cam,(0,.1,1.08));bpy.ops.wm.save_as_mainfile(filepath=out+'\\Water_Peixinho_refinado.blend')
print('REFINED SAVE OK')
