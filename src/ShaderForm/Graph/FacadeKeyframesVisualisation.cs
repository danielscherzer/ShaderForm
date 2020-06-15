﻿using ShaderForm.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShaderForm.Graph
{
	public class FacadeKeyframesVisualisation
	{
		public delegate void ChangedPositionHandler(float position);
		public event ChangedPositionHandler ChangedPosition;
		public event KeyEventHandler KeyDown;
		public event KeyEventHandler KeyUp;

		public FacadeKeyframesVisualisation(string uniformName, IKeyFrames kfs)
		{
			formGraph = new FormGraph(uniformName);
			formGraph.ChangedPoints += FormGraph_OnChangePoints;
			formGraph.ChangedPosition += (pos) => { ChangedPosition?.Invoke((float)pos); };
			formGraph.KeyDown += (sender, args) => { KeyDown?.Invoke(sender, args); };
			formGraph.KeyUp += (sender, args) => { KeyUp?.Invoke(sender, args); };
			formGraph.CopyCommand += (sender, args) => { if (!(kfs is null)) kfs.CopyKeyframesToClipboard(); };
			formGraph.PasteCommand += (sender, args) => { if (!(kfs is null)) kfs.PasteKeyframesFromClipboard(); };
			this.kfs = kfs;
			Update();
		}

		public bool IsYourForm(object sender)
		{
			return sender == formGraph;
		}

		public void AddInterpolatedKeyframe(float position)
		{
			if (kfs is null) return;
			var value = kfs.Interpolate(position);
			kfs.AddUpdate(position, value);
		}

		internal void AddKeyframe(float position, float value)
		{
			if (kfs is null) return;
			kfs.AddUpdate(position, value);
		}

		public void Close()
		{
			SaveData();
			formGraph.Close();
		}

		public void Update()
		{
			if (!updating)
			{
				formGraph.SetPoints(kfs);
			}
		}

		public void SaveData()
		{
			formGraph.SaveLayout();
		}

		public void Show()
		{
			formGraph.Visible = true;
			Update();
		}

		public bool IsVisible { get { return formGraph.Visible; } }

		public void UpdatePosition(float position)
		{
			formGraph.UpdatePosition(position);
		}

		private readonly FormGraph formGraph;
		private readonly IKeyFrames kfs;
		private bool updating = false;

		private void FormGraph_OnChangePoints(IEnumerable<KeyValuePair<float, float>> points)
		{
			if (kfs is null) return;
			updating = true;
			kfs.Clear();
			foreach (var p in points)
			{
				kfs.AddUpdate(p.Key, p.Value);
			}
			updating = false;
		}
	}
}
