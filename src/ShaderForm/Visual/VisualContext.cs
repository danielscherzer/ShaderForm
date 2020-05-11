namespace ShaderForm.Visual
{
	using OpenTK.Graphics.OpenGL4;
	using ShaderForm.Interfaces;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Numerics;
	using System.Reflection;
	using Zenseless.HLGL;
	using Zenseless.OpenGL;
	using Zenseless.Patterns;

	public class VisualContext : Disposable, ISetUniform
	{
		public VisualContext()
		{
			contentManager = ContentManagerGL.Create(Assembly.GetExecutingAssembly(), false);
			GL.Disable(EnableCap.DepthTest);
			GL.ClearColor(1, 0, 0, 0);

			renderSurfaces = new PingPongFbo(2);

			copyToScreen = new PostProcessing(contentManager.LoadPixelShader("copy.frag"));
			shaderDefault = contentManager.LoadPixelShader("Checker.frag");
		}

		public void SetUniform(string uniformName, float value)
		{
			Debug.Assert(!(shaderCurrent is null));
			shaderCurrent.Uniform(uniformName, value);
		}

		public void SetUniform(string uniformName, float valueX, float valueY)
		{
			Debug.Assert(!(shaderCurrent is null));
			GL.Uniform2(shaderCurrent.GetResourceLocation(ShaderResourceType.Uniform, uniformName), valueX, valueY);
		}

		public void SetUniform(string uniformName, float valueX, float valueY, float valueZ)
		{
			Debug.Assert(!(shaderCurrent is null));
			GL.Uniform3(shaderCurrent.GetResourceLocation(ShaderResourceType.Uniform, uniformName), valueX, valueY, valueZ);
		}

		public bool SetShader(string shaderFileName)
		{
			if (!shaders.TryGetValue(shaderFileName, out shaderCurrent))
			{
				shaderCurrent = shaderDefault;
			}
			Debug.Assert(!(shaderCurrent is null));
			shaderCurrent.Activate();
			return shaderCurrent != shaderDefault;
		}

		public void Update()
		{
			Debug.Assert(!(shaderCurrent is null));

			glTimer.Activate(QueryTarget.TimeElapsed);
			//texture binding
			int id = 0;
			foreach (var tex in textures)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + id);
				tex.Activate();
				shaderCurrent.Uniform("tex" + id.ToString(), id);
				++id;
			}
			//bind last frame as texture
			for (int i = 0; i < renderSurfaces.Last.Count; ++i)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + id);
				renderSurfaces.Last[i].Activate();
				shaderCurrent.Uniform($"texLastFrame{i}", id);
				++id;
			}

			renderSurfaces.Render();

			id = 0;
			foreach (var tex in textures)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + id);
				tex.Deactivate();
				++id;
			}
			//unbind last frame as texture
			for (int i = 0; i < renderSurfaces.Last.Count; ++i)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + id);
				renderSurfaces.Last[i].Deactivate();
				++id;
			}
			GL.ActiveTexture(TextureUnit.Texture0);
			glTimer.Deactivate();
		}

		public void UpdateSurfaceSize(int width, int height)
		{
			renderSurfaces.UpdateSurfaceSize(width, height);
		}

		public bool AddUpdateTexture(string fileName)
		{
			if (File.Exists(fileName))
			{
				try
				{
					using (var bitmap = new Bitmap(fileName))
					{
						var tex = TextureLoaderDrawing.FromBitmap(bitmap);
						int index = textureNames.FindIndex((string name) => name == fileName);
						if (0 <= index)
						{
							//overwrite old
							textures[index].Dispose();
							textures[index] = tex;
						}
						textureNames.Add(fileName);
						textures.Add(tex);
						return true;
					}
				}
				catch { }
			}
			return false;
		}

		public IEnumerable<string> GetTextureNames()
		{
			return textureNames;
		}

		public bool RemoveTexture(string fileName)
		{
			int index = textureNames.FindIndex((string name) => name == fileName);
			if (0 <= index)
			{
				textures[index].Dispose();
				textureNames.RemoveAt(index);
				textures.RemoveAt(index);
				return true;
			}
			return false;
		}

		public string AddUpdateFragmentShader(string fileName)
		{
			var dir = Path.GetDirectoryName(fileName);
			string GetIncludeCode(string includeName)
			{
				var includePath = Path.Combine(dir, includeName);
				var includeCode = File.ReadAllText(includePath);
				using (var includeShader = new ShaderGL(Zenseless.HLGL.ShaderType.FragmentShader))
				{
					if (!includeShader.Compile(includeCode))
					{
						var e = includeShader.CreateException(includeCode);
						throw new ShaderLoadException($"Error compiling include file '{includePath}'");
					}
				}
				return includeCode;
			}

			if (shaders.ContainsKey(fileName))
			{
				if (shaderDefault != shaders[fileName])
				{
					shaders[fileName].Dispose();
					shaders[fileName] = shaderDefault;
				}
			}
			var fragmentShaderCode = File.ReadAllText(fileName);
			var expandedFragmentShaderCode = GLSLhelper.Transformations.ExpandIncludes(fragmentShaderCode, GetIncludeCode);
			var fragmentShader = new ShaderGL(Zenseless.HLGL.ShaderType.FragmentShader);
			if (!fragmentShader.Compile(expandedFragmentShaderCode))
			{
				throw new ShaderLoadException($"Error loading '{fileName}' with log\n{fragmentShader.Log}");
			}
			var vertexShaderCode = contentManager.Load<string>("ScreenQuad.vert");
			var vertexShader = new ShaderGL(Zenseless.HLGL.ShaderType.VertexShader);
			if (!vertexShader.Compile(vertexShaderCode))
			{
				throw new ShaderLoadException($"Error loading default vertex shader! Hardware too old or update graphics drivers required!");
			}
			ShaderProgramGL shaderProgram = new ShaderProgramGL();
			shaderProgram.Attach(vertexShader);
			shaderProgram.Attach(fragmentShader);
			if (!shaderProgram.Link())
			{
				throw new ShaderLoadException($"Error linking shaders for fragment shader '{fileName}'");
			}
			shaders[fileName] = shaderProgram;
			return shaderProgram.Log;
		}

		public void RemoveShader(string shaderFileName)
		{
			if (shaders.TryGetValue(shaderFileName, out IShaderProgram shaderProgram))
			{
				shaderProgram.Dispose();
				shaders.Remove(shaderFileName);
			}
		}

		public void Draw(int width, int height, bool filter)
		{
			GL.Viewport(0, 0, width, height);
			renderSurfaces.ActiveFirst.Filter = filter ? TextureFilterMode.Linear : TextureFilterMode.Nearest;
			copyToScreen.Draw(renderSurfaces.ActiveFirst);
			renderSurfaces.SwapRenderBuffer();
		}

		public Bitmap GetScreenshot()
		{
			return TextureLoaderDrawing.SaveToBitmap(renderSurfaces.ActiveFirst);
		}

		public Vector4[] GetBuffer()
		{
			var tex = renderSurfaces.ActiveFirst;
			var buffer = new Vector4[tex.Width * tex.Height];
			tex.Activate();
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Float, buffer);
			tex.Deactivate();
			return buffer;
		}

		public float UpdateTime { get { return (float)(glTimer.ResultLong * 1e-9); } }
		//public IEnumerable<string> ShaderList { get { return shaders.Keys; } }
		//public IEnumerable<string> TextureList { get { return textureNames; } }

		private readonly List<string> textureNames = new List<string>();
		private readonly List<ITexture> textures = new List<ITexture>();
		private readonly Dictionary<string, IShaderProgram> shaders = new Dictionary<string, IShaderProgram>();
		private readonly PingPongFbo renderSurfaces;
		private readonly PostProcessing copyToScreen;
		private IShaderProgram shaderCurrent;
		private readonly IShaderProgram shaderDefault;
		private readonly QueryObject glTimer = new QueryObject();
		private readonly FileContentManager contentManager;

		protected override void DisposeResources()
		{
			foreach (var shader in shaders.Values) shader.Dispose();
			foreach (var tex in textures) tex.Dispose();
			shaderDefault.Dispose();
			copyToScreen.Dispose();
			renderSurfaces.Dispose();
		}
	}
}
