using ShaderForm.Demo;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShaderForm
{
	public partial class FormSaveImages : Form
	{
		private class Settings
		{
			public string Directory { get; set; }
			public uint ResolutionX { get; set; } = 1920;
			public uint ResolutionY { get; set; } = 1080;
			public uint FramesPerSecond { get; set; } = 25;
		}

		public FormSaveImages()
		{
			InitializeComponent();
			settings = new Settings();
			Dialogs.SaveFile("", (fileName) => settings.Directory = Path.ChangeExtension(fileName, null));
			propertyGrid1.SelectedObject = settings;
		}

		public static DialogResult Show(DemoModel demo)
		{
			// rename old
			DefaultFiles.RenameAutoSaveDemoFile();
			// save
			DemoLoader.SaveToFile(demo, DefaultFiles.GetAutoSaveDemoFilePath());
			var form = new FormSaveImages();
			return (form.ShowDialog());
		}

		private void ButtonClickHandler(object sender, System.EventArgs e)
		{
			string quote(string input) => '"' + input + '"';
			var assembly = Assembly.GetExecutingAssembly();
			var demoRecorderPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "DemoRecorder.exe");
			Process.Start(demoRecorderPath, $"{quote(DefaultFiles.GetAutoSaveDemoFilePath())} {quote(settings.Directory)} {settings.ResolutionX} {settings.ResolutionY} {settings.FramesPerSecond}");
			Close();
		}
	}
}
