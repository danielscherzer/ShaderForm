[![Build status](https://ci.appveyor.com/api/projects/status/1sd0eouxsm4vt8e5?svg=true)](https://ci.appveyor.com/project/danielscherzer/shaderform)

# Shader Form
See the [change log](CHANGELOG.md) for changes, features and road map.
![ShaderForm](ShaderForm.png)

## Look at [Examples](https://github.com/danielscherzer/ACG-shader)

## Create videos
+ You can create image sequences with Window -> Save Images with arbitrary resolution and a defined framerate. You can feed those into `ffmpeg -framerate 25 -i %%05d.png -i audio.mp3 -c:v libx264 -pix_fmt yuv420p -c:a copy "output.mp4"` to create a video

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

## License
[Apache 2.0](LICENSE)