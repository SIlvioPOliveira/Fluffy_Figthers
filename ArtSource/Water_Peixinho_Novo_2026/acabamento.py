import bpy
from mathutils import Vector
scene=bpy.data.scenes['Water_Peixinho_Novo'];bpy.context.window.scene=scene
# Turn the side fins toward the front while retaining contact with the body.
for s,label in [(-1,'direita'),(1,'esquerda')]:
    ob=bpy.data.objects['Peitoral '+label]
    for v in ob.data.vertices:
        x,y,z=v.co;u=-s*.30*(x-s*.83)+.954*(y+.09);d=.954*(x-s*.83)+s*.30*(y+.09)
        v.co=(s*.79+s*.6*u+.8*d,-.13+.8*u-s*.6*d,1.02+(z-1.02)*1.1)
# Softer neutral studio prevents clipped highlights while preserving vibrant colors.
scene.view_settings.view_transform='Standard';scene.view_settings.exposure=-.8
cam=scene.camera
def aim(o,p):o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
out=r'C:\Users\sotav\Documents\ChatGPT\Fluffy Figthers\ArtSource\Water_Peixinho_Novo_2026'
views=[('frente',(0,-7,1.05)),('lado',(7,0,1.05)),('costas',(0,7,1.05)),('tres_quartos_frente',(5,-7,3.2)),('tres_quartos_costas',(5,7,3.2))]
for name,pos in views:
    cam.location=pos;aim(cam,(0,.1,1.08));scene.render.filepath=out+'\\'+name+'_final.png';bpy.ops.render.render(write_still=True);print('VIEW',name)
cam.location=views[3][1];aim(cam,(0,.1,1.08));bpy.ops.wm.save_as_mainfile(filepath=out+'\\Water_Peixinho_Final.blend')
print('FINAL SAVED')
