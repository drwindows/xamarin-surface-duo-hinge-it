using HingeIt.Utils.MicrosoftDuoLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HingeIt
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : DuoPage
    {
        enum PageState
        {
            UnsupportedDevice,
            UnsupportedOrientation,
            Game
        }

        public MainPage()
        {
            InitializeComponent();

            // Update Page to current state.
            UpdatePageToState(GetPageState());
        }

        #region Private helper

        private PageState GetPageState()
        {
            // It has to be a surface duo.
            if (FormsWindow.IsDuo == false) return PageState.UnsupportedDevice;

            // The app has to be in portrait mode and spanned across both screens.
            if (!FormsWindow.IsPortrait || !FormsWindow.IsSpanned) return PageState.UnsupportedOrientation;

            // If everything is correct, the game is playable.
            return PageState.Game;
        }

        private void UpdatePageToState(PageState state)
        {
            GameStackLayout.IsVisible = state == PageState.Game;
            ErrorStackLayout.IsVisible = state != PageState.Game;
            ErrorLabel.Text = GetErrorMessageForState(state);
        }

        private string GetErrorTitleForState(PageState state)
        {
            switch(state)
            {
                case PageState.UnsupportedDevice:
                    return "Unsupported device";

                case PageState.UnsupportedOrientation:
                    return "Unsupported orientation";

                default:
                    return "";
            }
        }

        private string GetErrorMessageForState(PageState state)
        {
            switch(state)
            {
                case PageState.UnsupportedDevice:
                    return "HingeIt! is only supported on a Microsoft Surface Duo.";

                case PageState.UnsupportedOrientation:
                    return "Please use portrait mode and span the app across both displays.";

                default:
                    return "";
            }
        }

        #endregion
    }
}
