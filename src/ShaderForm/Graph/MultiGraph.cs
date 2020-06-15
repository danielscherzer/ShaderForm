using ShaderForm.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShaderForm.Graph
{
	public class MultiGraph
	{
		public delegate void ChangedPositionHandler(float position);
		public event ChangedPositionHandler ChangedPosition;
		public event KeyEventHandler KeyDown;
		public event KeyEventHandler KeyUp;

		public FacadeKeyframesVisualisation GetGraph(object sender)
		{
			foreach (var graph in graphs.Values)
			{
				if (graph.IsYourForm(sender))
				{
					return graph;
				}
			}
			return null;
		}

		public IEnumerable<FacadeKeyframesVisualisation> GetVisibleGraphs()
		{
			foreach (var graph in graphs.Values)
			{
				if (graph.IsVisible) yield return graph;
			}
		}

		internal void AddKeyframeTo(object sender, float position)
		{
			throw new NotImplementedException();
		}

		internal void AddKeyframeToVisible(float position)
		{
			throw new NotImplementedException();
		}

		public void SaveLayout()
		{
			foreach (var graph in graphs.Values)
			{
				graph.SaveData();
			}
		}

		public void Show(string uniformName)
		{
			if (!graphs.ContainsKey(uniformName)) return;
			graphs[uniformName].Show();
		}

		public void Uniforms_OnRemove(object _, string uniformName)
		{
			if (string.IsNullOrEmpty(uniformName)) return;
			if (graphs.ContainsKey(uniformName))
			{
				graphs[uniformName].Close();
				graphs.Remove(uniformName);
			}
		}

		public void Uniforms_OnAdd(object sender, string uniformName)
		{
			if (string.IsNullOrEmpty(uniformName)) return;
			if (!(sender is IUniforms uniforms)) return;
			var kfs = uniforms.GetKeyFrames(uniformName);
			var visualisation = new FacadeKeyframesVisualisation(uniformName, kfs);
			visualisation.ChangedPosition += (position) => ChangedPosition?.Invoke(position);
			visualisation.KeyDown += (s, a) => { KeyDown?.Invoke(s, a); };
			visualisation.KeyUp += (s, a) => { KeyUp?.Invoke(s, a); };
			graphs.Add(uniformName, visualisation);
		}

		public void Uniforms_OnChange(object _, string uniformName)
		{
			if (string.IsNullOrEmpty(uniformName)) return;
			if (graphs.ContainsKey(uniformName))
			{
				graphs[uniformName].Update();
			}
		}

		public void UpdatePosition(float position)
		{
			foreach (var graph in graphs.Values)
			{
				graph.UpdatePosition(position);
			}
		}

		private readonly Dictionary<string, FacadeKeyframesVisualisation> graphs = new Dictionary<string, FacadeKeyframesVisualisation>();
	}
}
