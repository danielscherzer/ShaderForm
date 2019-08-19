using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShaderForm
{
	class RecentShaderFiles
	{
		public RecentShaderFiles(Action<string> onClick)
		{
			Menu = new ToolStripMenuItem("Recent shader files");
			this.onClick = onClick;
		}


		public void Add(string fileName)
		{
			if (!File.Exists(fileName)) return;
			if (recentShaderFiles.LastOrDefault() == fileName) return;
			recentShaderFiles.RemoveAll(name => name == fileName);
			recentShaderFiles.Add(fileName);
			var removeCount = Math.Max(0, recentShaderFiles.Count - 20);
			recentShaderFiles.RemoveRange(0, removeCount); // keep last 20 shader files
			var items = Menu.DropDownItems;
			items.Clear();
			foreach(var recentFileName in recentShaderFiles.Reverse<string>())
			{
				var newItem = new ToolStripMenuItem(recentFileName);
				if (onClick != null) newItem.Click += (s, a) =>
				 {
					 onClick(recentFileName);
				 };
				items.Add(newItem);
			}
		}

		private List<string> recentShaderFiles = new List<string>();
		private readonly Action<string> onClick;

		public ToolStripMenuItem Menu { get; }
		public IEnumerable<string> FileNames { get => recentShaderFiles; }
	}
}
