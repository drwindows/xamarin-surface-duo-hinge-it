using System;

namespace HingeIt.Utils.MicrosoftDuoLibrary
{
    public class LayoutGuideChangedEventArgs : EventArgs
	{
		public LayoutGuideChangedEventArgs(LayoutGuide layoutGuide)
		{
			LayoutGuide = layoutGuide;
		}

		public LayoutGuide LayoutGuide { get; }
	}
}
