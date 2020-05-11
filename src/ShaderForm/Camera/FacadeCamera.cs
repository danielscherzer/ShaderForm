using ShaderForm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShaderForm.Camera
{
	public class FacadeCamera
	{
		public FacadeCamera()
		{
			adapter = new AdapterCamera(camera);
			formCamera.Set(adapter);
		}

		public void AddKeyFrames(float time, IUniforms uniforms)
		{
			//todo: event recursions handle all with mediator pattern or similar
			var position = camera.Position;
			var rotation = camera.Rotation;
			for (int i = 0; i < 3; ++i)
			{
				uniforms.Add(posUniformNames[i]);
				var kfsPos = uniforms.GetKeyFrames(posUniformNames[i]);
				kfsPos.AddUpdate(time, position[i]);

				uniforms.Add(rotUniformNames[i]);
				var kfsRot = uniforms.GetKeyFrames(rotUniformNames[i]);
				kfsRot.AddUpdate(time, rotation[i]);
			}
		}

		public void SaveLayout()
		{
			formCamera.SaveData();
		}

		public void KeyChange(Keys keyCode, bool pressed)
		{
			camera.KeyChange(keyCode, pressed);
			if (IsActive)
			{
				formCamera.Set(new AdapterCamera(camera));
			}
			Redraw?.Invoke(this);
		}

		public bool IsActive { get { return camera.IsActive; } }

		public delegate void ChangedHandler(FacadeCamera camera);
		public event ChangedHandler Redraw;

		public void Reset()
		{
			camera = new FlyCamera();
		}

		public void Show()
		{
			formCamera.Visible = true;
		}

		public void Update(float mouseX, float mouseY, bool mouseDown)
		{
			camera.Update(mouseX, mouseY, mouseDown);
		}

		public void CopyKeyFrames(IUniforms uniforms)
		{
			var camKF = new List<IKeyFrames>();
			for (int i = 0; i < 3; ++i)
			{
				camKF.Add(uniforms.GetKeyFrames(posUniformNames[i]));
			}
			for (int i = 0; i < 3; ++i)
			{
				camKF.Add(uniforms.GetKeyFrames(rotUniformNames[i]));
			}
			camKF.RemoveAll((kf) => kf is null);
			if (camKF.Count != 6) return;
			var table = from posX in camKF[0]
						join posY in camKF[1] on posX.Key equals posY.Key
						join posZ in camKF[2] on posX.Key equals posZ.Key
						join rotX in camKF[3] on posX.Key equals rotX.Key
						join rotY in camKF[4] on posX.Key equals rotY.Key
						join rotZ in camKF[5] on posX.Key equals rotZ.Key
						select $"{posX.Key}\t{posX.Value}\t{posY.Value}\t{posZ.Value}\t{rotX.Value}\t{rotY.Value}\t{rotZ.Value}";

			var sb = new StringBuilder();
			foreach (var entry in table)
			{
				sb.AppendLine(entry);
			}
			Clipboard.SetText(sb.ToString());
		}


		public void PasteKeyFrames(IUniforms uniforms)
		{
			var camKF = new List<IKeyFrames>();
			for (int i = 0; i < 3; ++i)
			{
				camKF.Add(uniforms.GetKeyFrames(posUniformNames[i]));
			}
			for (int i = 0; i < 3; ++i)
			{
				camKF.Add(uniforms.GetKeyFrames(rotUniformNames[i]));
			}
			camKF.RemoveAll((kf) => kf is null);
			camKF.ForEach((kf) => kf.Clear());
			if (camKF.Count != 6) return;

			string data = Clipboard.GetText();
			var lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				try
				{
					var values = Array.ConvertAll(line.Split('\t'), float.Parse);
					if (values.Length != 7) return;
					for (int i = 0; i < 6; ++i) camKF[i].AddUpdate(values[0], values[i + 1]);
				}
				catch { }
			}
		}

		public bool UpdateFromUniforms(IUniforms uniforms, float time)
		{
			for (int i = 0; i < 3; ++i)
			{
				var kfsPos = uniforms.GetKeyFrames(posUniformNames[i]);
				if (kfsPos is null) return false;
				var value = kfsPos.Interpolate(time);
				camera.Position[i] = value;

				var kfsRot = uniforms.GetKeyFrames(rotUniformNames[i]);
				if (kfsRot is null) return false;
				var valueRot = kfsRot.Interpolate(time);
				camera.Rotation[i] = valueRot;
			}
			return true;
		}

		public void SetUniforms(ISetUniform visualContext)
		{
			for (int i = 0; i < 3; ++i)
			{
				visualContext.SetUniform(posUniformNames[i], camera.Position[i]);
				visualContext.SetUniform(rotUniformNames[i], camera.Rotation[i]);
			}
		}

		private FlyCamera camera = new FlyCamera();
		private readonly AdapterCamera adapter;
		private readonly FormCamera formCamera = new FormCamera();
		private readonly string[] posUniformNames = { "iCamPosX", "iCamPosY", "iCamPosZ" };
		private readonly string[] rotUniformNames = { "iCamRotX", "iCamRotY", "iCamRotZ" };
	}
}
