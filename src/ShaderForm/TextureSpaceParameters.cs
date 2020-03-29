using System;
using System.Globalization;
using Zenseless.Geometry;

namespace ShaderForm
{
	class TextureSpaceParameters
	{
		private float factor = 1.0f;
		private bool relativeFactor = true;
		private int mouseX = 0;
		private int mouseY = 0;
		private int width = 1;
		private int height = 1;

		public int MouseButton { get; internal set; } = 0;
		public int MouseX
		{
			get
			{
				int x = MathHelper.Clamp(mouseX, 0, width - 1);
				x = (int)Math.Round(relativeFactor ? factor * x : (factor * x) / width);
				return MathHelper.Clamp(x, 0, Width - 1);
			}
		}

		public int MouseY
		{
			get
			{
				int y = height - 1 - MathHelper.Clamp(mouseY, 0, height - 1);
				y = (int)Math.Round(relativeFactor ? factor * y : (factor * y) / width);
				return MathHelper.Clamp(y, 0, Height - 1);
			}
		}

		public int Height => (int)Math.Round(relativeFactor ? height * factor : factor);
		public int Width => (int)Math.Round(relativeFactor ? width * factor : factor);

		public void ParseFactor(string sizeString)
		{
			float.TryParse(sizeString.Substring(1), NumberStyles.Float, CultureInfo.InvariantCulture, out factor);
			relativeFactor = 'x' == sizeString[0];
		}

		public void SetControlSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public void SetMousePosition(int x, int y)
		{
			mouseX = x;
			mouseY = y;
		}
	}
}
