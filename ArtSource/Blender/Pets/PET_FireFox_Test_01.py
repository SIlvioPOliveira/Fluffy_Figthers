"""Non-canonical Fire Fox prototype. Run in Blender 4.4 with --python.

Change SPEC and choose a fresh id/output path for variations. The script refuses
to overwrite outputs or an existing collection and builds a separate scene.
"""
import bpy
import math
from mathutils import Vector
from pathlib import Path

SPEC = {
    "id": "PET_FireFox_Test_01",
    "orange": (0.95, 0.235, 0.035, 1),
    "cream": (1.0, 0.79, 0.49, 1),
    "dark": (0.055, 0.026, 0.022, 1),
    "white": (1.0, 0.97, 0.86, 1),
    "body_width": 1.7,
    "tail_scale": 1.0,
}


def material(name, color, roughness=0.7):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get('Principled BSDF')
    bsdf.inputs['Base Color'].default_value = color
    bsdf.inputs['Roughness'].default_value = roughness
    return mat


def build_character(spec, scene):
    ident = spec['id']
    if not ident.replace('_', '').isalnum() or bpy.data.collections.get(ident):
        raise ValueError('Use a unique alphanumeric ID with underscores')
    coll = bpy.data.collections.new(ident)
    scene.collection.children.link(coll)
    root = bpy.data.objects.new(ident + '_ROOT', None)
    coll.objects.link(root)
    mats = {key: material(ident + '_' + key, spec[key], 0.3 if key == 'dark' else 0.7)
            for key in ('orange', 'cream', 'dark', 'white')}

    def finish(obj, name, mat):
        obj.name = name
        for c in list(obj.users_collection):
            c.objects.unlink(obj)
        coll.objects.link(obj)
        obj.parent = root
        obj.data.materials.append(mats[mat])
        return obj

    def rounded_box(name, loc, dimensions, radius, mat):
        bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
        obj = bpy.context.object
        obj.dimensions = dimensions
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        bevel = obj.modifiers.new('Soft low-poly corners', 'BEVEL')
        bevel.width = radius
        bevel.segments = 2
        bpy.ops.object.modifier_apply(modifier=bevel.name)
        for poly in obj.data.polygons:
            poly.use_smooth = True
        obj.modifiers.new('Weighted normals', 'WEIGHTED_NORMAL')
        return finish(obj, name, mat)

    def oval(name, loc, scale, mat, segments=16, rings=8):
        bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=rings, location=loc)
        obj = bpy.context.object
        obj.scale = scale
        for poly in obj.data.polygons:
            poly.use_smooth = True
        return finish(obj, name, mat)

    def prism(name, verts, faces, mat, bevel=0.035):
        mesh = bpy.data.meshes.new(name + '_Mesh')
        mesh.from_pydata(verts, [], faces)
        mesh.update()
        obj = bpy.data.objects.new(name, mesh)
        coll.objects.link(obj)
        obj.parent = root
        mesh.materials.append(mats[mat])
        if bevel:
            mod = obj.modifiers.new('Soft edges', 'BEVEL')
            mod.width = bevel
            mod.segments = 2
            obj.modifiers.new('Weighted normals', 'WEIGHTED_NORMAL')
        return obj

    # Primary body, then the four feet and the two ear silhouettes.
    rounded_box('Body', (0, 0, 0.99), (spec['body_width'], 1.40, 1.42), 0.17, 'orange')
    for side, x in [('L', 0.57), ('R', -0.57)]:
        for end, y in [('Front', -0.46), ('Back', 0.46)]:
            rounded_box('Paw_' + end + '_' + side, (x, y, 0.16), (0.35, 0.40, 0.32), 0.09, 'orange')
        # A thick tapered ear; inner ear is a small front-facing prism.
        pts = [(x-0.30, 1.49), (x+0.30, 1.49), (x+(-0.06 if x < 0 else 0.06), 2.40)]
        verts = [(a, y, z) for y in (-0.20, 0.14) for a, z in pts]
        prism('Ear_' + side, verts, [(0,2,1),(3,4,5),(0,1,4,3),(1,2,5,4),(2,0,3,5)], 'orange', 0.045)
        inner = [(x-0.18,-0.225,1.62), (x+0.18,-0.225,1.62), (x+(-0.045 if x < 0 else 0.045),-0.225,2.20),
                 (x-0.18,-0.20,1.62), (x+0.18,-0.20,1.62), (x+(-0.045 if x < 0 else 0.045),-0.20,2.20)]
        prism('EarInner_' + side, inner, [(0,2,1),(3,4,5),(0,1,4,3),(1,2,5,4),(2,0,3,5)], 'cream', 0.015)

    # Broad flame-like tail, ring-built so it stays a single low-poly mesh.
    rings = [(0.40,0.61,0.78,0.19), (0.67,0.90,1.00,0.38),
             (0.94,1.10,1.32,0.42), (1.10,1.18,1.69,0.34),
             (1.12,1.17,1.98,0.22), (0.97,1.10,2.25,0.025)]
    vertices = []
    for x,y,z,r in rings:
        for i in range(8):
            a = 2*math.pi*i/8
            vertices.append((x*spec['tail_scale'] + r*math.cos(a), y+r*0.72*math.sin(a), z))
    faces = [tuple(reversed(range(8)))]
    for j in range(len(rings)-1):
        for i in range(8):
            faces.append((j*8+i,j*8+(i+1)%8,(j+1)*8+(i+1)%8,(j+1)*8+i))
    faces.append(tuple((len(rings)-1)*8+i for i in range(8)))
    tail = prism('Tail_Flame', vertices, faces, 'orange', 0.035)
    tail.data.materials.append(mats['cream'])
    for p in tail.data.polygons:
        if p.center.z > 1.90:
            p.material_index = 1

    # Face on the -Y side; L/R are the character's sides.
    oval('Muzzle', (0,-0.707,0.73), (0.55,0.10,0.30), 'cream')
    for side,x in [('L',0.34),('R',-0.34)]:
        oval('Eye_'+side, (x,-0.716,1.14), (0.18,0.095,0.25), 'dark')
        oval('Highlight_'+side, (x-0.045,-0.805,1.23), (0.042,0.025,0.052), 'white', 12, 6)
    oval('Nose', (0,-0.817,0.84), (0.095,0.048,0.065), 'dark', 12, 6)
    curve = bpy.data.curves.new('Smile_Curve', 'CURVE')
    curve.dimensions = '3D'
    curve.bevel_depth = 0.018
    curve.bevel_resolution = 2
    spline = curve.splines.new('POLY')
    coords = [(-0.18,-0.813,0.69),(-0.12,-0.824,0.645),(0,-0.833,0.625),(0.12,-0.824,0.645),(0.18,-0.813,0.69)]
    spline.points.add(len(coords)-1)
    for p,co in zip(spline.points, coords):
        p.co = (*co,1)
    smile = bpy.data.objects.new('Smile', curve)
    coll.objects.link(smile)
    smile.parent = root
    curve.materials.append(mats['dark'])
    return coll, root


def setup_review(scene):
    studio = bpy.data.collections.new('Review_Studio')
    scene.collection.children.link(studio)
    world = bpy.data.worlds.new('Review_World')
    world.use_nodes = True
    world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.21,0.16,0.19,1)
    world.node_tree.nodes['Background'].inputs['Strength'].default_value = 0.35
    scene.world = world
    def aim(obj, target):
        obj.rotation_euler = (Vector(target)-obj.location).to_track_quat('-Z','Y').to_euler()
    for name, loc, power, size in [('Key',(-3,-4,6),650,4.0),('Fill',(4,-2,4),430,3.5),('Rim',(1,4,5),600,3.0)]:
        data = bpy.data.lights.new(name, 'AREA')
        data.energy, data.shape, data.size = power, 'DISK', size
        obj = bpy.data.objects.new(name, data)
        studio.objects.link(obj)
        obj.location = loc
        aim(obj,(0,0,1))
    camdata = bpy.data.cameras.new('Review_Camera')
    camera = bpy.data.objects.new('Review_Camera', camdata)
    studio.objects.link(camera)
    scene.camera = camera
    camdata.type = 'ORTHO'
    camdata.ortho_scale = 3.8
    scene.render.engine = 'CYCLES'
    scene.cycles.samples = 24
    scene.cycles.use_denoising = True
    scene.render.resolution_x = 768
    scene.render.resolution_y = 768
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = 'PNG'
    scene.render.film_transparent = False
    return camera, aim


def main():
    script_path = Path(__file__).resolve()
    base = script_path.parent.parent
    ident = SPEC['id']
    blend_path = script_path.with_name(ident + '.blend')
    review_dir = base / 'Reviews'
    views = {'front': (0,-7,1.4), 'side': (7,0,1.4), 'three_quarter': (5,-7,4.0)}
    outputs = [review_dir / (ident+'_'+view+'.png') for view in views]
    if blend_path.exists() or any(p.exists() for p in outputs):
        raise FileExistsError('Refusing to overwrite an existing blend or review image')
    if not review_dir.is_dir():
        raise FileNotFoundError(review_dir)
    scene = bpy.data.scenes.new(ident + '_Scene')
    bpy.context.window.scene = scene
    scene.unit_settings.system = 'METRIC'
    coll, root = build_character(SPEC, scene)
    camera, aim = setup_review(scene)
    depsgraph = bpy.context.evaluated_depsgraph_get()
    tris = 0
    for obj in coll.objects:
        if obj.type in {'MESH','CURVE'}:
            evaluated = obj.evaluated_get(depsgraph)
            mesh = evaluated.to_mesh()
            mesh.calc_loop_triangles()
            tris += len(mesh.loop_triangles)
            evaluated.to_mesh_clear()
    print('PET_COUNTS', 'triangles=',tris,'objects=',len(coll.objects),'materials=',len({m.name for o in coll.objects if o.type in {'MESH','CURVE'} for m in o.data.materials if m}))
    for (view, pos), output in zip(views.items(), outputs):
        camera.location = pos
        aim(camera, (0.22,0.20,1.20))
        scene.render.filepath = str(output)
        bpy.ops.render.render(write_still=True)
        print('RENDER_SAVED', output)
    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))
    print('BLEND_SAVED', blend_path)


if __name__ == '__main__':
    main()
