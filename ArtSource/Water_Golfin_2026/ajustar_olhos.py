import bpy,bmesh
from mathutils import Vector
scene=bpy.data.scenes['Water_Golfin_Studio'];bpy.context.window.scene=scene
for name in ('direita','esquerda'):
    eye=bpy.data.objects['Olho '+name]
    for v in eye.data.vertices:v.co.y-=.10
    eye.data.update()
    bpy.data.objects['Reflexo '+name].location.y-=.13
tongue=bpy.data.objects['Lingua'];tongue.location.y-=.025
out=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Golfin_2026';cam=scene.camera
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
views=[('frente',(0,-7,1.1)),('lado_esquerdo',(7,0,1.1)),('lado_direito',(-7,0,1.1)),('tras',(0,7,1.1)),('topo',(0,0,8)),('tres_quartos_frente',(5,-7,3.5)),('tres_quartos_tras',(5,7,3.5))]
coll=bpy.data.collections['PET_Water_Golfin'];dg=bpy.context.evaluated_depsgraph_get();tri=0
for ob in coll.objects:
    if ob.type=='MESH':
        e=ob.evaluated_get(dg);m=e.to_mesh();m.calc_loop_triangles();tri+=len(m.loop_triangles);e.to_mesh_clear()
        bm=bmesh.new();bm.from_mesh(ob.data);non=sum(not ed.is_manifold for ed in bm.edges);deg=sum(f.calc_area()<1e-9 for f in bm.faces);bm.free()
        if non or deg:print('TOPOLOGY',ob.name,non,deg)
print('STATS triangles',tri,'mesh_objects',sum(o.type=='MESH' for o in coll.objects),'all_objects',len(coll.objects))
print('MATERIALS',sorted({m.name for o in coll.objects if o.type=='MESH' for m in o.data.materials if m}))
for name,pos in views:
    cam.location=pos;aim(cam,(0,.05,1.1));scene.render.filepath=out+'\\'+name+'_final.png';bpy.ops.render.render(write_still=True);print('RENDER',name)
cam.location=views[5][1];aim(cam,(0,.05,1.1));bpy.ops.wm.save_as_mainfile(filepath=out+'\\Water_Golfin_Final.blend');print('SAVED_FINAL')
