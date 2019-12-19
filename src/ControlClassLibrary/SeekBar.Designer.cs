namespace ControlClassLibrary
{
	partial class SeekBar
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.playing = new System.Windows.Forms.CheckBox();
			this.timerUpdateMarkerBar = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
			this.markerBarPosition = new ControlClassLibrary.FloatValueBar();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// playing
			// 
			this.playing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.playing.Appearance = System.Windows.Forms.Appearance.Button;
			this.playing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.playing.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.playing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.playing.Image = global::ControlClassLibrary.Properties.Resources.PlayHS;
			this.playing.Location = new System.Drawing.Point(0, 0);
			this.playing.Margin = new System.Windows.Forms.Padding(6);
			this.playing.Name = "playing";
			this.playing.Size = new System.Drawing.Size(54, 42);
			this.playing.TabIndex = 1;
			this.playing.UseVisualStyleBackColor = true;
			this.playing.CheckedChanged += new System.EventHandler(this.Playing_CheckedChanged);
			// 
			// timerUpdateMarkerBar
			// 
			this.timerUpdateMarkerBar.Interval = 10;
			this.timerUpdateMarkerBar.Tick += new System.EventHandler(this.TimerUpdateMarkerBar_Tick);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.ShowImageMargin = false;
			this.contextMenuStrip1.ShowItemToolTips = false;
			this.contextMenuStrip1.Size = new System.Drawing.Size(246, 84);
			// 
			// toolStripComboBox1
			// 
			this.toolStripComboBox1.Items.AddRange(new object[] {
            "100Hz",
            "60Hz",
            "10Hz",
            "1Hz"});
			this.toolStripComboBox1.Name = "toolStripComboBox1";
			this.toolStripComboBox1.Size = new System.Drawing.Size(121, 38);
			this.toolStripComboBox1.TextChanged += new System.EventHandler(this.ToolStripComboBox1_TextChanged);
			// 
			// markerBarPosition
			// 
			this.markerBarPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.markerBarPosition.BackColor = System.Drawing.Color.Gray;
			this.markerBarPosition.BarColor = System.Drawing.Color.GreenYellow;
			this.markerBarPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.markerBarPosition.Cursor = System.Windows.Forms.Cursors.VSplit;
			this.markerBarPosition.Decimals = ((byte)(2));
			this.markerBarPosition.ForeColor = System.Drawing.Color.White;
			this.markerBarPosition.Location = new System.Drawing.Point(51, 0);
			this.markerBarPosition.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
			this.markerBarPosition.Max = 10F;
			this.markerBarPosition.Min = 0F;
			this.markerBarPosition.Name = "markerBarPosition";
			this.markerBarPosition.ShowText = false;
			this.markerBarPosition.Size = new System.Drawing.Size(447, 40);
			this.markerBarPosition.TabIndex = 0;
			this.markerBarPosition.Text = "markerBar1";
			this.markerBarPosition.Value = 3F;
			this.markerBarPosition.ValueChanged += new System.EventHandler(this.MarkerBarPosition_ValueChanged);
			this.markerBarPosition.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MarkerBarPosition_MouseDown);
			// 
			// SeekBar
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.playing);
			this.Controls.Add(this.markerBarPosition);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "SeekBar";
			this.Size = new System.Drawing.Size(500, 42);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private FloatValueBar markerBarPosition;
		private System.Windows.Forms.CheckBox playing;
		private System.Windows.Forms.Timer timerUpdateMarkerBar;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
	}
}
