using Zenseless.HLGL;
using Zenseless.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using Zenseless.Patterns;

namespace ShaderForm.Visual
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="Disposable" />
	public class PingPongFbo : Disposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PingPongFbo"/> class.
		/// </summary>
		public PingPongFbo(uint renderTargetCount)
		{
			var drawBufferList = new List<DrawBuffersEnum>();
			for(int i = 0; i < renderTargetCount; ++i)
			{
				drawBufferList.Add(DrawBuffersEnum.ColorAttachment0 + i);
			}
			drawBuffers = drawBufferList.ToArray();
			CreateFBOs(1, 1);
			activeFBO = fboA;
		}

		/// <summary>
		/// Gets the texture of the active FBO.
		/// </summary>
		/// <value>
		/// The active.
		/// </value>
		public ITexture2D ActiveFirst { get { return activeFBO.Texture; } }
		public IReadOnlyList<ITexture2D> GetActiveRenderTargets() => activeFBO.Textures;

		/// <summary>
		/// Gets the last.
		/// </summary>
		/// <value>
		/// The last.
		/// </value>
		public IReadOnlyList<ITexture2D> Last {  get { return LastFBO.Textures; } }

		/// <summary>
		/// Renders this instance.
		/// </summary>
		public void Render()
		{
			activeFBO.Activate();
			GL.DrawBuffers(drawBuffers.Length, drawBuffers);
			//Drawing
			GL.DrawArrays(PrimitiveType.Quads, 0, 4);
			activeFBO.Deactivate();
		}

		/// <summary>
		/// Swaps the render buffer.
		/// </summary>
		public void SwapRenderBuffer()
		{
			activeFBO = LastFBO;
		}

		/// <summary>
		/// Updates the size of the surface.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public void UpdateSurfaceSize(int width, int height)
		{
			if (0 == width || 0 == height) return;
			if (width != activeFBO.Texture.Width || height != activeFBO.Texture.Height)
			{
				var isActive = activeFBO == fboA;
				fboA.Dispose();
				fboB.Dispose();
				CreateFBOs(width, height);
				activeFBO = isActive ? fboA : fboB;
			}
		}

		/// <summary>
		/// Will be called from the default Dispose method.
		/// Implementers should dispose all their resources her.
		/// </summary>
		protected override void DisposeResources()
		{
			activeFBO = null;
			fboA.Dispose();
			fboB.Dispose();
		}

		private DrawBuffersEnum[] drawBuffers;
		private FBO fboA, fboB, activeFBO;
		private FBO LastFBO { get { return (activeFBO == fboA) ? fboB : fboA; } }

		private void CreateFBOs(int width, int height)
		{
			fboA = new FBO(CreateTexture(width, height));
			fboB = new FBO(CreateTexture(width, height));
			for (int i = 1; i < drawBuffers.Length; ++i)
			{
				fboA.Attach(CreateTexture(width, height));
				fboB.Attach(CreateTexture(width, height));
			}
		}

		private Texture2dGL CreateTexture(int width, int height)
		{
			//return Texture.Create(width, height);
			return Texture2dGL.Create(width, height, 4, true);
		}
	}
}
