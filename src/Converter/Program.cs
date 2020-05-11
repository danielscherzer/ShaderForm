﻿using ShaderForm.Demo;
using ShaderForm.DemoData;
using ShaderForm.DemoData2;
using System;
using System.IO;
using System.Linq;
using Zenseless.Patterns;

namespace Converter
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (2 > args.Count()) return;
			try
			{
				var fileName = args[0];
				var fileNameNew = args[1];
				var dataOld = Serialization.FromXMLFile(fileName, typeof(DemoData)) as DemoData;
				string oldDir = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName)));
				var soundFileName = PathTools.GetFullPath(dataOld.SoundFileName);
				Directory.SetCurrentDirectory(oldDir);

				//calculating demo length
				float length = 100.0f;
				if (!string.IsNullOrWhiteSpace(soundFileName))
				{
					var sound = DemoTimeSource.FromMediaFile(soundFileName);
					if (sound is null)
					{
						Console.WriteLine("Could not load sound file '" + soundFileName + "'");
						return;
					}
					length = sound.Length;
				}
				var ratios = dataOld.ShaderRatios.Select((item) => new Tuple<float, string>(item.Ratio, item.ShaderPath));
				//convert ratios to absolute times
				var keyframes = ShaderKeyframes.CalculatePosFromRatios(ratios, length);
				//save to new format
				DemoData2 data = new DemoData2
				{
					SoundFileName = dataOld.SoundFileName,
					Textures = dataOld.Textures.ToList()
				};
				var track = new Track
				{
					Name = "sum"
				};
				data.Tracks.Add(track);
				foreach (var element in keyframes)
				{
					track.ShaderKeyframes.Add(new ShaderKeyframe(element.Item1, element.Item2));
				}
				foreach (var uniformOld in dataOld.Uniforms)
				{
					var uniform = new Uniform(uniformOld.UniformName);
					foreach (var keyframeOld in uniformOld.Keyframes)
					{
						uniform.Keyframes.Add(new ShaderForm.DemoData2.Keyframe(keyframeOld.Time, keyframeOld.Value));
					}
					data.Uniforms.Add(uniform);
				}
				Serialization.ToXMLFile(data, fileNameNew);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error exception '" + e.Message + "'");
			}
		}
	}
}
