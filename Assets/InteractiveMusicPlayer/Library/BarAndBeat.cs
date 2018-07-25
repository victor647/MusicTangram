using System;
using UnityEngine;

namespace InteractiveMusicPlayer
{
	[Serializable]
	public struct BarAndBeat
	{

		public int Bar, Beat;

		public BarAndBeat(int bar, int beat) //constructor
		{
			if (bar >= 0) //make sure that the bars and beats cannot be nagative
			{
				Bar = bar;
			}
			else
			{
				Debug.LogWarning("Bar number cannot be negative!");
				Bar = 0;
			}

			if (beat >= 0)
			{
				Beat = beat;
			}
			else
			{
				Debug.LogWarning("Beat number cannot be negative!");
				Beat = 0;
			}
		}

		//operator for comparing
		#region OPERATORS 
		public static bool operator ==(BarAndBeat x, BarAndBeat y)
		{
			return x.Bar == y.Bar && x.Beat == y.Beat;
		}

		public static bool operator !=(BarAndBeat x, BarAndBeat y)
		{
			return x.Bar != y.Bar || x.Beat != y.Beat;
		}

		public static bool operator >(BarAndBeat x, BarAndBeat y)
		{
			if (x.Bar > y.Bar)
			{
				return true;
			}

			if (x.Bar == y.Bar)
			{
				return x.Beat > y.Beat;
			}

			return false;
		}

		public static bool operator <(BarAndBeat x, BarAndBeat y)
		{
			if (x.Bar < y.Bar)
			{
				return true;
			}

			if (x.Bar == y.Bar)
			{
				return x.Beat < y.Beat;
			}

			return false;
		}

		public override bool Equals(System.Object y)
		{
			return this == (BarAndBeat) y;
		}

		public static bool operator >=(BarAndBeat x, BarAndBeat y)
		{
			return x > y || x == y;
		}

		public static bool operator <=(BarAndBeat x, BarAndBeat y)
		{
			return x < y || x == y;
		}

		public override int GetHashCode() //just for Unity to not throw warnings
		{
			return Bar ^ Beat;
		}
		#endregion

		#region VALUES		
		public static BarAndBeat Zero
		{
			get { return new BarAndBeat(0, 0); }
		}

		public static BarAndBeat OneBar
		{
			get { return new BarAndBeat(1, 0); }
		}

		public static BarAndBeat OneBeat
		{
			get { return new BarAndBeat(0, 1); }
		}
		#endregion
	}
}
