# Shader Form
![ShaderForm](ShaderForm.png)

## Look at [Examples](https://github.com/danielscherzer/ACG-shader)

## Create videos
+ Project [DemoRecorder](/src/DemoRecorder): Hier können Bilderfolgen von demos mit fixer frame rate und beliebiger auflösung erzeugt werden. Für die unter euch die richtig smoothe videos haben wollen, nehmen dann diese bilder und z.b. ffmpeg  -framerate 25 -i %1\%%05d.png -c:v libx264 -pix_fmt yuv420p "output.mp4" + euer sound file... und fertig ist das video