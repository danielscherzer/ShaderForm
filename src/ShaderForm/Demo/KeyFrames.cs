﻿using ShaderForm.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Zenseless.Geometry;

namespace ShaderForm.Demo
{
	public class KeyFrames : IKeyFrames
	{
		public event EventHandler<EventArgs> Changed;

		public void AddUpdate(float time, float value)
		{
			var clippedTime = Math.Max(0.0f, time);
			keyframes.AddUpdate(clippedTime, value);
			CallOnChange();
		}

		public void Clear()
		{
			keyframes.Clear();
			CallOnChange();
		}

		public float Interpolate(float currentTime)
		{
			//float cubic(float t)
			//{
			//	return t * t * (3f - 2f * t);
			//}
			if (0 == keyframes.Count) return 0.0f;
			var pair = keyframes.FindPair(currentTime);
			//interpolation
			float valueDelta = pair.Item2 - pair.Item1;
			return pair.Item1 + pair.Item3 * valueDelta;
		}

		public IEnumerator<KeyValuePair<float, float>> GetEnumerator()
		{
			return keyframes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return keyframes.GetEnumerator();
		}

		protected void CallOnChange()
		{
			Changed?.Invoke(this, EventArgs.Empty);
		}

		private readonly ControlPoints<float> keyframes = new ControlPoints<float>();
	}
}
