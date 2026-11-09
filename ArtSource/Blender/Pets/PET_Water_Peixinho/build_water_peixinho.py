"""Primeira versão de teste de Water_Peixinho; execute com Blender 4.4 e bpy.

Gera uma cena isolada e se recusa a sobrescrever a fonte ou os renders.
Edite SPEC e escolha uma pasta de saída vazia para produzir variações.
Frente do personagem: -Y; cima: +Z. Não exporta para Unity.
"""
import bpy
import math
from mathutils import Vector
from pathlib import Path

SPEC = {
    'id': 'PET_Water_Peixinho',
    'body_width': 0.82,
    'body_length': 1.02,
    'body_height': 0.80,
    'center_height': 1.03,
    'colors': {
        'LightBlue': (0.19, 0.60, 0.94, 1),
        'OceanBlue': (0.008, 0.20, 0.78, 1),
        'White': (0.91, 0.94, 1.0, 1),
        'Yellow': (1.0, 0.63, 0.015, 1),
        'Eye': (0.004, 0.015, 0.047, 1),
        'Mouth': (0.25, 0.012, 0.05, 1),
        'Pink': (1.0, 0.24, 0.37, 1),
    },
}


def build_character(spec, scene):
    ident = spec['id']
    if bpy.data.collections.get(ident):
        raise ValueError('Collection already exists: ' + ident)
    coll = bpy.data.collections.new(ident)
    scene.collection.children.link(coll)
    root = bpy.data.objects.new(ident + '_ROOT', None)
    coll.objects.link(root)
    mats = {}
    for name, color in spec['colors'].items():
        mat = bpy.data.materials.new(ident + '_' + name)
        mat.diffuse_color = color
        mat.use_nodes = True
        shader = mat.node_tree.nodes.get('Principled BSDF')
        shader.inputs['Base Color'].default_value = color
        shader.inputs['Roughness'].default_value = 0.26 if name == 'Eye' else 0.43
        mats[name] = mat

    def link(obj, name, color):
        obj.name = name
        for old in list(obj.users_collection):
            old.objects.unlink(obj)
        coll.objects.link(obj)
        obj.parent = root
        obj.data.materials.append(mats[color])
        return obj

    def oval(name, pos, size, color, segments=24, rings=12, rotate=None):
        bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=rings, location=pos)
        obj = bpy.context.object
        obj.scale = size
        if rotate:
            obj.rotation_euler = rotate
        for poly in obj.data.polygons:
            poly.use_smooth = True
        return link(obj, name, color)

    # Ellipsoid built as transverse rings. Ring boundaries match the color bands;
    # the white belly is assigned on the same continuous mesh, without textures.
    ts = sorted(set([-1.0, 1.0, -0.14, 0.04, 0.39, 0.56] + [i/24 for i in range(-23,24)]))
    around = 40
    verts = []
    for t in ts:
        radius = math.sqrt(max(0, 1-t*t))
        for i in range(around):
            a = 2*math.pi*i/around
            verts.append((spec['body_width']*radius*math.cos(a), spec['body_length']*t,
                          spec['center_height']+spec['body_height']*radius*math.sin(a)))
    faces = []
    colors = []
    for j in range(len(ts)-1):
        t = (ts[j]+ts[j+1])/2
        for i in range(around):
            faces.append((j*around+i,j*around+(i+1)%around,(j+1)*around+(i+1)%around,(j+1)*around+i))
            underside = math.sin(2*math.pi*(i+0.5)/around) < -0.58
            colors.append(2 if underside or -0.14 <= t <= 0.04 or 0.39 <= t <= 0.56 else (0 if t < -0.14 else 1))
    mesh = bpy.data.meshes.new('Body_Mesh')
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    body = bpy.data.objects.new('Body_Striped', mesh)
    coll.objects.link(body)
    body.parent = root
    for key in ('LightBlue','OceanBlue','White'):
        mesh.materials.append(mats[key])
    for poly, idx in zip(mesh.polygons, colors):
        poly.material_index = idx
        poly.use_smooth = True

    # Raised, rounded fin layers. Yellow is inset inside the blue edge.
    oval('Dorsal_Blue_Rim',(0,0.06,1.76),(0.13,0.65,0.52),'OceanBlue')
    for side, x in [('L',0.105),('R',-0.105)]:
        oval('Dorsal_Yellow_'+side,(x,0.06,1.79),(0.065,0.52,0.40),'Yellow')
    oval('Tail_Base',(0,0.92,1.02),(0.26,0.32,0.27),'LightBlue')
    oval('Tail_Blue_Rim',(0,1.39,1.04),(0.14,0.40,0.53),'OceanBlue')
    for side, x in [('L',0.115),('R',-0.115)]:
        oval('Tail_Yellow_'+side,(x,1.39,1.04),(0.075,0.31,0.43),'Yellow')
    for side, sign in [('L',1),('R',-1)]:
        oval('SideFin_Rim_'+side,(sign*0.77,0.05,0.98),(0.17,0.32,0.31),'OceanBlue')
        oval('SideFin_Yellow_'+side,(sign*0.905,0.035,1.00),(0.095,0.265,0.25),'Yellow')
        oval('LowerFin_'+side,(sign*0.39,-0.06,0.31),(0.18,0.29,0.105),'OceanBlue',rotate=(0,sign*0.20,sign*0.25))
        # Both eyes follow the curved front surface of the body.
        oval('Eye_'+side,(sign*0.35,-0.868,1.20),(0.195,0.10,0.27),'Eye',rotate=(0,0,sign*0.15))
        oval('EyeHighlight_'+side,(sign*0.395,-0.955,1.315),(0.060,0.035,0.072),'White',16,8)
        oval('Cheek_'+side,(sign*0.55,-0.765,0.91),(0.125,0.035,0.065),'Pink',16,8,rotate=(0,0,sign*0.34))
        oval('Brow_'+side,(sign*0.35,-0.825,1.52),(0.115,0.035,0.035),'OceanBlue',16,8)

    # Curved open smile: closed 3D mesh with a dark interior and pink tongue.
    outline = [(-0.19,0.94),(-0.14,0.915),(0,0.90),(0.14,0.915),(0.19,0.94),
               (0.16,0.82),(0.10,0.76),(0,0.74),(-0.10,0.76),(-0.16,0.82)]
    verts = [(x,y,z) for y in (-1.013,-0.99) for x,z in outline]
    n = len(outline)
    faces = [tuple(range(n-1,-1,-1)),tuple(range(n,2*n))]
    faces += [(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
    mouth_mesh = bpy.data.meshes.new('Mouth_Mesh')
    mouth_mesh.from_pydata(verts,[],faces)
    mouth_mesh.update()
    mouth = bpy.data.objects.new('Mouth_Smile',mouth_mesh)
    coll.objects.link(mouth)
    mouth.parent = root
    mouth.data.materials.append(mats['Mouth'])
    bevel = mouth.modifiers.new('Rounded lip', 'BEVEL')
    bevel.width = 0.012
    bevel.segments = 2
    oval('Tongue',(0,-1.027,0.785),(0.105,0.026,0.045),'Pink',20,10)
    return coll


def add_studio(scene):
    studio = bpy.data.collections.new('Water_Peixinho_Review_Studio')
    scene.collection.children.link(studio)
    world = bpy.data.worlds.new('Water_Peixinho_Review_World')
    world.use_nodes = True
    world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.035,0.045,0.105,1)
    world.node_tree.nodes['Background'].inputs['Strength'].default_value = 0.38
    scene.world = world
    def aim(obj, target):
        obj.rotation_euler = (Vector(target)-obj.location).to_track_quat('-Z','Y').to_euler()
    for name,loc,power,size in [('Key',(-3,-4,5),450,4),('Fill',(4,-2,3),250,3),('Rim',(1,4,5),450,3)]:
        light = bpy.data.lights.new(name,'AREA')
        light.energy, light.shape, light.size = power, 'DISK', size
        obj = bpy.data.objects.new(name,light)
        studio.objects.link(obj)
        obj.location = loc
        aim(obj,(0,0,1))
    camera_data = bpy.data.cameras.new('Review_Camera')
    camera = bpy.data.objects.new('Review_Camera', camera_data)
    studio.objects.link(camera)
    scene.camera = camera
    camera_data.type = 'ORTHO'
    camera_data.ortho_scale = 3.55
    scene.render.engine = 'CYCLES'
    scene.cycles.samples = 32
    scene.cycles.use_denoising = True
    scene.render.resolution_x = scene.render.resolution_y = 768
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = 'PNG'
    return camera, aim


def main():
    out = Path(__file__).resolve().parent
    blend = out / (SPEC['id'] + '.blend')
    views = [('front',(0,-7,1.18)),('side',(7,0,1.18)),('back',(0,7,1.18)),
             ('top',(0,0,8)),('front_three_quarter',(5,-7,3.7)),('back_three_quarter',(5,7,3.7))]
    images = [out / (SPEC['id']+'_'+name+'.png') for name,_ in views]
    if blend.exists() or any(p.exists() for p in images):
        raise FileExistsError('Output exists; choose a fresh folder before running')
    scene = bpy.data.scenes.new(SPEC['id']+'_Scene')
    bpy.context.window.scene = scene
    scene.unit_settings.system = 'METRIC'
    coll = build_character(SPEC,scene)
    camera, aim = add_studio(scene)
    depsgraph = bpy.context.evaluated_depsgraph_get()
    triangles = 0
    for obj in coll.objects:
        if obj.type == 'MESH':
            e = obj.evaluated_get(depsgraph)
            mesh = e.to_mesh()
            mesh.calc_loop_triangles()
            triangles += len(mesh.loop_triangles)
            e.to_mesh_clear()
    used_mats = sorted({m.name for o in coll.objects if o.type == 'MESH' for m in o.data.materials if m})
    print('PET_STATS objects=',len(coll.objects),'meshes=',sum(o.type=='MESH' for o in coll.objects),'triangles=',triangles,'materials=',used_mats)
    for (name,pos),path in zip(views,images):
        camera.location = pos
        aim(camera,(0,0,1.18))
        scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print('RENDER',name,str(path))
    bpy.ops.wm.save_as_mainfile(filepath=str(blend))
    print('BLEND',str(blend))


if __name__ == '__main__':
    main()
