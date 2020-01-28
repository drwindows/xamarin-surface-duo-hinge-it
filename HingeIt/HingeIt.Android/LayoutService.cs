﻿using HingeIt.Droid;
using HingeIt.Utils.MicrosoftDuoLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(LayoutService))]
namespace HingeIt.Droid
{
    public class LayoutService : LayoutServiceBase, ILayoutService
    {
        public override Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.GetRenderer(visualElement);

            if (view?.View == null)
                return null;

            int[] location = new int[2];
            view.View.GetLocationOnScreen(location);
            return new Point(view.View.Context.FromPixels(location[0]), view.View.Context.FromPixels(location[1]));
        }
    }
}