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
	internal class MyApplication
	{
		[STAThread]
		private static void Main()
		{
			var window = new MyApplication();
			window.Run();
		}

		private readonly GameWindow gameWindow = new GameWindow();
		private readonly VisualContext visualContext;
		private readonly DemoModel demo;
		private readonly int bufferWidth;
		private readonly int bufferHeight;
		private readonly string saveDirectory;
		private int fileNumber;
		private readonly int frameRate;

		private MyApplication()
		{
			gameWindow.KeyDown += GameWindow_KeyDown;
			gameWindow.RenderFrame += Game_RenderFrame;
			visualContext = new VisualContext();
			var textures = new Textures(visualContext);
			var shaders = new Shaders(NewShaderFile);
			demo = new DemoModel(visualContext, shaders, textures, false);
			demo.TimeSource.TimeFinished += () => gameWindow.Close();

			var arguments = Environment.GetCommandLineArgs();
			if (3 > arguments.Length)
			{
				MessageBox.Show("DemoRecorder <shaderform demo configfile> <saveDirectory> [<resolutionX> <resolutionY> <frameRate>]"
					+ Environment.NewLine
					+ " Please give the demo config file name as application parameter followed by the resolution.");
				gameWindow.Close();
				return;
			}
			bufferWidth = gameWindow.Width;
			bufferHeight = gameWindow.Height;
			try
			{
				bufferWidth = int.Parse(arguments.ElementAt(3));
				bufferHeight = int.Parse(arguments.ElementAt(4));
			}
			catch (FormatException)
			{
				bufferWidth = gameWindow.Width;
				bufferHeight = gameWindow.Height;
			}
			try
			{
				frameRate = int.Parse(arguments.ElementAt(5));
			}
			catch (FormatException)
			{
				frameRate = 25;
			}
			try
			{
				DemoLoader.LoadFromFile(demo, arguments.ElementAt(1));
				saveDirectory = Directory.CreateDirectory(arguments.ElementAt(2)).FullName;
				saveDirectory += Path.DirectorySeparatorChar;
				saveDirectory += DateTime.Now.ToString("yyyyMMdd HHmmss");
				saveDirectory += Path.DirectorySeparatorChar;
				Directory.CreateDirectory(saveDirectory);
				fileNumber = 0;
			}
			catch (Exception e)
			{
				MessageBox.Show("Error loading demo '" + arguments.ElementAt(1) + '"'
					+ Environment.NewLine + e.Message);
				gameWindow.Close();
			}
		}

		private void Run()
		{
			gameWindow.VSync = VSyncMode.Off;
			gameWindow.Run(200.0);
		}

		private void GameWindow_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (Key.Escape == e.Key)
			{
				gameWindow.Close();
			}
		}

		private IShaderFile NewShaderFile()
		{
			return new ShaderFile(visualContext);
		}

		private void Game_RenderFrame(object sender, FrameEventArgs e)
		{
			demo.UpdateBuffer(0, 0, 0, bufferWidth, bufferHeight);
			demo.Draw(gameWindow.Width, gameWindow.Height, true);
			demo.GetScreenshot().Save(saveDirectory + fileNumber.ToString("00000") + ".png");
			gameWindow.SwapBuffers();
			++fileNumber;
			demo.TimeSource.Position += 1.0f / frameRate; //step 1/25 of a second
		}
	}
}