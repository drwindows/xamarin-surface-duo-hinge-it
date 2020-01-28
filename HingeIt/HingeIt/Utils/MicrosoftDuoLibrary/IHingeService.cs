﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace HingeIt.Utils.MicrosoftDuoLibrary
{
	public interface IHingeService : INotifyPropertyChanged, IDisposable
	{
		event EventHandler<HingeEventArgs> OnHingeUpdated;

		bool IsSpanned { get; }

		bool IsLandscape { get; }

		Rectangle GetHinge();

		#region Custom changes

		bool IsDuo { get; }

		#endregion
	}

	public class HingeEventArgs : EventArgs
	{
		public HingeEventArgs(int angle)
			: base()
		{
			Angle = angle;
		}

		public int Angle { get; private set; }
	}
}
