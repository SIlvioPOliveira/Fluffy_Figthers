"""Execute on the original PET_Water_Peixinho.blend to create a review copy."""
import bpy
from mathutils import Vector

out = r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Blender\Pets\PET_Water_Peixinho\Refinamento_01'
assert bpy.data.filepath.endswith('PET_Water_Peixinho.blend')
coll = bpy.data.collections['PET_Water_Peixinho']
for side in ('L', 'R'):
    eye = bpy.data.objects['Eye_' + side]
    eye.scale.y *= 0.78
    eye.location.y -= 0.05
    bpy.data.objects['EyeHighlight_' + side].location.y -= 0.033
    cheek = bpy.data.objects['Cheek_' + side]
    cheek.scale.y *= 0.65
    cheek.location.y -= 0.012
    bpy.data.objects['Brow_' + side].scale.y *= 0.70
    for name in ('SideFin_Rim_' + side, 'SideFin_Yellow_' + side):
        obj = bpy.data.objects[name]
        obj.data = obj.data.copy()
        for v in obj.data.vertices:
            v.co.z *= 0.62 + 0.38 * (v.co.y + 1.0) / 2.0
for name in ('Tail_Blue_Rim', 'Tail_Yellow_L', 'Tail_Yellow_R'):
    obj = bpy.data.objects[name]
    obj.data = obj.data.copy()
    for v in obj.data.vertices:
        v.co.z *= 0.38 + 0.62 * (v.co.y + 1.0) / 2.0
bpy.data.objects['Mouth_Smile'].location.y -= 0.045
bpy.data.objects['Tongue'].location.y -= 0.045
scene = bpy.context.scene
camera = scene.camera
scene.cycles.samples = 24
scene.render.resolution_x = scene.render.resolution_y = 768
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = 'PNG'
bpy.ops.wm.save_as_mainfile(filepath=out + r'\PET_Water_Peixinho_refinamento_01.blend')
views = [('front',(0,-7,1.18)), ('side',(7,0,1.18)), ('back',(0,7,1.18)), ('top',(0,0,8)), ('front_three_quarter',(5,-7,3.7)), ('back_three_quarter',(5,7,3.7))]
for name, pos in views:
    camera.location = pos
    camera.rotation_euler = (Vector((0,0,1.18))-camera.location).to_track_quat('-Z','Y').to_euler()
    scene.render.filepath = out + r'\PET_Water_Peixinho_' + name + '.png'
    bpy.ops.render.render(write_still=True)
depsgraph = bpy.context.evaluated_depsgraph_get()
tris = 0
for obj in coll.objects:
    if obj.type == 'MESH':
        ev = obj.evaluated_get(depsgraph)
        mesh = ev.to_mesh()
        mesh.calc_loop_triangles()
        tris += len(mesh.loop_triangles)
        ev.to_mesh_clear()
print('STATS', len(coll.objects), 'objects', sum(o.type == 'MESH' for o in coll.objects), 'meshes', tris, 'triangles')
print('MATERIALS', sorted({m.name for o in coll.objects if o.type == 'MESH' for m in o.data.materials if m}))
