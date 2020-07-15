using OpenTK;
using System.ComponentModel;

namespace ShaderForm.Camera
{
	public class AdapterCamera : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public float PositionX { get { return cam.Position.X; } set => UpdatePos(0, value, nameof(PositionX)); }
		public float PositionY { get { return cam.Position.Y; } set => UpdatePos(1, value, nameof(PositionY)); }
		public float PositionZ { get { return cam.Position.Z; } set => UpdatePos(2, value, nameof(PositionZ)); }
		public float RotationX { get { return MathHelper.RadiansToDegrees(cam.Rotation.X); } set => UpdateRot(0, value, nameof(RotationX)); }
		public float RotationY { get { return MathHelper.RadiansToDegrees(cam.Rotation.Y); } set => UpdateRot(1, value, nameof(RotationY)); }
		public float RotationZ { get { return MathHelper.RadiansToDegrees(cam.Rotation.Z); } set => UpdateRot(2, value, nameof(RotationZ)); }
		public float Speed { get { return cam.Speed; } set { cam.Speed = value; NotifyPropertyChanged(nameof(Speed)); } }

		public AdapterCamera(FlyCamera cam)
		{
			this.cam = cam;
		}

		internal void Reset()
		{
			cam.Reset();
		}

		private readonly FlyCamera cam;

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdatePos(int index, float value, string propertyName)
		{
			var p = cam.Position; 
			p[index] = value;
			cam.Position = p;
			NotifyPropertyChanged(propertyName);
		}

		private void UpdateRot(int index, float value, string propertyName)
		{
			var r = cam.Rotation;
			r[index] = MathHelper.DegreesToRadians(value);
			cam.Rotation = r;
			NotifyPropertyChanged(propertyName);
		}
	}
}
