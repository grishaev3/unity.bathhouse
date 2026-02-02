import random
import pymxs
rt = pymxs.runtime

for j in range(5):
    for i in range(15):
        cyl = rt.Cylinder(
            radius=5 + random.uniform(-1.5, 1.5),
            height=40 + random.uniform(-4, 4),
            
            pos=rt.Point3(i*12, 0, j*6),
            
            slice=1,
            sliceFrom=0,
            sliceTo=random.randint(210, 220),
            
            heightsegs=2
        )
        
        myPosition = cyl.transform.position
        myTransform = cyl.transform
        rotation_y = random.uniform(0.0, 0.2)
        myTransform.rotation = rt.angleAxis(90, rt.point3(1, rotation_y, 0))
        myTransform.position = myPosition
        cyl.transform  = myTransform
        
        noise_x =  3 + random.uniform(-1, 1)
        noise_y =  3 + random.uniform(-1, 1)
        noise_mod = rt.Noisemodifier(strength=rt.Point3(noise_x, noise_y, 0), scale=15, fractal=1,iterations=1.0)
        rt.addModifier(cyl, noise_mod)