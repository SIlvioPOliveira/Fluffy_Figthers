import bpy,bmesh
from mathutils import Vector
scene=bpy.data.scenes['Water_Peixinho_Novo'];bpy.context.window.scene=scene
coll=bpy.data.collections['Water_Peixinho']
for name in ('Corpo','Dorsal','Cauda','Peitoral direita','Peitoral esquerda'):
    ob=bpy.data.objects[name];bm=bmesh.new();bm.from_mesh(ob.data)
    bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.001 if name=='Corpo' else .00001)
    bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces))
    bm.to_mesh(ob.data);bm.free();ob.data.update()
dg=bpy.context.evaluated_depsgraph_get();tri=0
for ob in coll.objects:
    if ob.type=='MESH':
        e=ob.evaluated_get(dg);m=e.to_mesh();m.calc_loop_triangles();tri+=len(m.loop_triangles);e.to_mesh_clear()
        bm=bmesh.new();bm.from_mesh(ob.data);non=sum(not edge.is_manifold for edge in bm.edges);deg=sum(f.calc_area()<1e-9 for f in bm.faces);bm.free()
        if non or deg:print('OPEN_OR_DEGENERATE',ob.name,non,deg)
print('TRIANGLES',tri,'MESHES',sum(o.type=='MESH' for o in coll.objects),'OBJECTS',len(coll.objects))
out=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Peixinho_Novo_2026'
cam=scene.camera;cam.location=(5,-7,3.2);cam.rotation_euler=(Vector((0,.1,1.08))-cam.location).to_track_quat('-Z','Y').to_euler()
scene.render.filepath=out+'\\tres_quartos_frente_validado.png';bpy.ops.render.render(write_still=True)
bpy.ops.wm.save_as_mainfile(filepath=out+'\\Water_Peixinho_Validado.blend')
print('SAVED_VALIDATED')
