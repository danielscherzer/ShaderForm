﻿namespace ControlClassLibrary
{
	using System;
	using System.Windows.Forms;
	using Zenseless.Sound;

	public partial class SeekBar : UserControl
	{
		public delegate void PositionHandler(float position);
		public delegate void PlayingStateHandler(bool playing);
		public event FinishedHandler Finished;
		public event PlayingStateHandler PlayingStateChanged;
		public event PositionHandler PositionChanged;

		public ITimedMedia TimeSource
		{
			get { return timeSource; }
			set
			{
				if (value is null) throw new Exception("Property TimeSource is forbidden to become null!");
				timeSource.TimeFinished -= CallOnFinished;
				if (value != defaultTimeSource)
				{
					defaultTimeSource.IsRunning = false;
				}
				timeSource = value;
				timeSource.TimeFinished += CallOnFinished;
				markerBarPosition.Max = timeSource.Length;
				markerBarPosition.Value = 0.0f;
				Playing = Playing;
			}
		}

		public SeekBar()
		{
			InitializeComponent();
			defaultTimeSource = new LoopableStopWatch(100.0f);
			timeSource = defaultTimeSource;
			timeSource.IsLooping = true;
			timeSource.TimeFinished += CallOnFinished;
			markerBarPosition.Max = timeSource.Length;
			Playing = false;
			toolStripComboBox1.Text = "60Hz";
		}

		public bool Playing
		{
			get { return playing.Checked; }
			set
			{
				bool old = playing.Checked;
				playing.Checked = value;
				if (value)
				{
					playing.Image = Properties.Resources.PauseHS;
				}
				else
				{
					playing.Image = Properties.Resources.PlayHS;
				}
				timeSource.IsRunning = value;
				timerUpdateMarkerBar.Enabled = value;
				if (value != old) PlayingStateChanged?.Invoke(value);
			}
		}

		public float Position
		{
			get { return markerBarPosition.Value; }
			set { markerBarPosition.Value = value; }
		}

		public int UpdateIntervalMsec
		{
			get => timerUpdateMarkerBar.Interval;
			set => timerUpdateMarkerBar.Interval = value;
		}

		public float Length { get { return timeSource.Length; } }

		private ITimedMedia timeSource;
		private bool timerChange = false;
		private readonly LoopableStopWatch defaultTimeSource;

		private void CallOnFinished()
		{
			Finished?.Invoke();
		}

		private void Playing_CheckedChanged(object sender, EventArgs e)
		{
			Playing = playing.Checked;
		}

		private void TimerUpdateMarkerBar_Tick(object sender, EventArgs e)
		{
			timerChange = true;
			Position = timeSource.Position;
			timerChange = false;
			//todo: cleanup handling of value changed (data-binding?) update circles!
		}

		private void MarkerBarPosition_ValueChanged(object sender, EventArgs e)
		{
			PositionChanged?.Invoke(Position);
			if (timerChange) return;
			timeSource.Position = Position;
		}

		private void MarkerBarPosition_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) contextMenuStrip1.Show(MousePosition);
		}

		private void ToolStripComboBox1_TextChanged(object sender, EventArgs e)
		{
			var strValue = toolStripComboBox1.Text.ToLowerInvariant().Replace("hz", string.Empty);
			if (int.TryParse(strValue, out var value))
			{
				timerUpdateMarkerBar.Interval = (int)(1000.0 / value);
			}
		}
	}
}
