using System;
using System.Windows.Forms;

namespace ShaderForm
{
	public static class Dialogs
	{
		public static void OpenFile(string filter, Action<string> action)
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				CheckPathExists = false,
				Filter = filter
			};
			if (DialogResult.OK == dlg.ShowDialog() && !(action is null))
			{
				action.Invoke(dlg.FileName);
			}
		}

		public static void SaveFile(string filter, Action<string> action)
		{
			SaveFileDialog dlg = new SaveFileDialog
			{
				Filter = filter,
				AddExtension = true
			};
			if (DialogResult.OK == dlg.ShowDialog() && !(action is null))
			{
				action.Invoke(dlg.FileName);
			}
		}

		public static void Help()
		{
			MessageBox.Show("Active main form\n"
				+ "F1 \t\t help\n"
				+ "F11 \t\t toggle fullscreen\n"
				+ "F12 \t\t toggle compact view\n"
				+ "right mouse \t show pixel information\n"
				+ "Ctrl+T \t\t Toggle on top\n"
				+ "Ctrl+O \t\t load demo\n"
				+ "Ctrl+S \t\t save demo\n"
				+ "Ctrl+H \t\t add shader\n"
				+ "Ctrl+Shift+S \t save screenshot\n"

				+ "\nActive shader that reads camera uniforms\n"
				+ "w|a|s|d \t\t camera pan xz-plane\n"
				+ "q|e \t\t camera up|down y-plane\n"
				+ "f|h \t\t camera heading\n"
				+ "t|g \t\t camera tilt\n"
				+ "left mouse drag \t camera direction\n"
				+ "+|- \t\t camera speed\n"
				+ "Ctrl+R \t\t reset camera\n"

				+ "\nActive graph form\n"
				+ "Ctrl+K \t\t add keyframe to this graph\n"
				+ "Ctrl+C \t\t copy Keyframes\n"
				+ "Ctrl+P \t\t paste Keyframes\n"

				+ "\nAll windows\n"
				+ "Escape \t\t close\n"
				+ "K \t\t add keyframe to all open graphs\n"
				+ "C \t\t add camera position and rotation keyframes\n"
				+ "Space \t\t play/pause\n"
				+ "Ctrl+Left \t time step backward 0.5 / 0.1 [+Alt]\n"
				+ "Ctrl+Right \t time step forward 0.5 / 0.1 [+Alt]\n"
				+ "Alt+Left \t\t time step backward 0.1\n"
				+ "Alt+Right \t time step forward 0.1\n"
				+ "PageUp \t\t time step backward 5 / 1 [+Alt]\n"
				+ "PageDown \t time step forward 5 / 1 [+Alt]\n"
				+ "Home \t\t time 0\n"
				+ "End \t\t time length\n"
				, "ShaderForm " + typeof(FormMain).Assembly.GetName().Version.ToString()
				);
		}
	}
}
