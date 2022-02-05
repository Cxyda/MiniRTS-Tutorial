using System;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Simulation.Data
{
	/// <summary>
	/// A integer representation of Unity's Vector3. It multiplies the incoming float X,Y,Z values with a precision and
	/// rounds the result to the next whole number. This clamping is done because sending floating point values  over
	/// the network and working with these across multiple systems is not deterministic.
	///
	/// A Vector3 representation is available within the Unity engine context and is constructed on the fly if not yet present.
	///
	/// This is done because this struct could also be used on a dedicated server where no Unity data types are available.
	/// </summary>
	public struct SimulationVector3
	{
		public static SimulationVector3 Zero => new SimulationVector3(0, 0, 0);

		private const int Precision = 10;

		private short _xShort;
		private short _yShort;
		private short _zShort;

		public int X => _xShort;
		public int Y => _yShort;
		public int Z => _zShort;

#if UNITY_STANDALONE || UNITY_EDITOR
		private UnityEngine.Vector3? _vector3;
		public UnityEngine.Vector3 Vector3
		{
			get
			{
				_vector3 ??= new UnityEngine.Vector3(
					_xShort / (float)Precision,
					_yShort / (float)Precision,
					_zShort / (float)Precision);

				return _vector3.Value;
			}
		}
#endif

		public SimulationVector3(float x, float y, float z)
		{
			_xShort = (short)Math.Round(x * Precision);
			_yShort = (short)Math.Round(y * Precision);
			_zShort = (short)Math.Round(z * Precision);
#if UNITY_STANDALONE || UNITY_EDITOR
			_vector3 = null;
#endif
		}

		public bool Equals(SimulationVector3 other)
		{
			return _xShort == other._xShort && _yShort == other._yShort && _zShort == other._zShort;
		}

		public override bool Equals(object obj)
		{
			return obj is SimulationVector3 other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _xShort.GetHashCode();
				hashCode = (hashCode * 397) ^ _yShort.GetHashCode();
				hashCode = (hashCode * 397) ^ _zShort.GetHashCode();
				return hashCode;
			}
		}
		public static bool operator ==(SimulationVector3 a, SimulationVector3 b)
		{
			return a._xShort == b._xShort && a._yShort == b._yShort && a._zShort == b._zShort;
		}

		public static bool operator !=(SimulationVector3 a, SimulationVector3 b)
		{
			return !(a == b);
		}


	}
}