using OpenTK;
using OpenTK.Input;
using ShaderForm.Demo;
using ShaderForm.DemoModelFactory;
using ShaderForm.Visual;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DemoRecorder
{
	internal class Program
	{
		[STAThread]
		private static void Main()
		{
			var window = new GameWindow(512, 512);
			var parameters = ParseCommandLine(window.Width, window.Height);
			if (parameters is null) return;
			var demo = CreateDemo();
			demo.TimeSource.TimeFinished += () => window.Close();
			try
			{
				DemoLoader.LoadFromFile(demo, parameters.DemoConfigFileName);
			}
			catch (Exception e)
			{
				MessageBox.Show("Error loading demo '" + parameters.DemoConfigFileName + '"' + Environment.NewLine + e.Message);
				return;
			}
			window.KeyDown += (s, e) =>
			{
				if (Key.Escape == e.Key)
				{
					window.Close();
				}
			};
			window.VSync = VSyncMode.Off;
			window.Visible = true; //show the window
			var fileNumber = 0;
			demo.UpdateBuffer(0, 0, 0, parameters.ResolutionX, parameters.ResolutionY);
			demo.Draw(window.Width, window.Height, true);
			window.SwapBuffers();
			while (window.Exists)
			{
				demo.UpdateBuffer(0, 0, 0, parameters.ResolutionX, parameters.ResolutionY);
				demo.Draw(window.Width, window.Height, true);
				using (var bmp = demo.GetScreenshot())
				{
					bmp.Save(Path.Combine(parameters.SaveDirectory, fileNumber.ToString("00000") + ".png"));
				}
				window.SwapBuffers();
				window.ProcessEvents();
				++fileNumber;
				demo.TimeSource.Position += 1.0f / parameters.FrameRate; //step 1/25 of a second
			}
		}

		private static DemoModel CreateDemo()
		{
			var visualContext = new VisualContext();
			var textures = new Textures(visualContext);
			var shaders = new Shaders(() => new ShaderFile(visualContext));
			return new DemoModel(visualContext, shaders, textures, false);
		}

		private static Parameters ParseCommandLine(int defaultResolutionX, int defaultResolutionY)
		{
			var arguments = Environment.GetCommandLineArgs();
			if (3 > arguments.Length)
			{
				MessageBox.Show("DemoRecorder <shaderform demo configfile> <saveDirectory> [<resolutionX> <resolutionY> <frameRate>]"
					+ Environment.NewLine + " Please give the demo configuration file name as application parameter followed by the resolution.");
				return null;
			}
			var parameters = new Parameters
			{
				DemoConfigFileName = arguments.ElementAt(1),
				SaveDirectory = Directory.CreateDirectory(arguments.ElementAt(2)).FullName
			};
			try
			{
				parameters.ResolutionX = int.Parse(arguments.ElementAt(3));
				parameters.ResolutionY = int.Parse(arguments.ElementAt(4));
			}
			catch (FormatException)
			{
				parameters.ResolutionX = defaultResolutionX;
				parameters.ResolutionY = defaultResolutionY;
			}
			try
			{
				parameters.FrameRate = int.Parse(arguments.ElementAt(5));
			}
			catch (FormatException)
			{
				parameters.FrameRate = 25;
			}
			return parameters;
		}
	}
}