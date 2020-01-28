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
    /// <summary>
    /// The MainPage contains functionallity for the master and
    /// the detail page.
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainPage : DuoPage
    {
        /// <summary>
        /// Determines the state of the page.
        /// </summary>
        enum PageState
        {
            /// <summary>
            /// App runs on a non supported device.
            /// </summary>
            UnsupportedDevice,

            /// <summary>
            /// App runs on a non supported orienation.
            /// </summary>
            UnsupportedOrientation,

            /// <summary>
            /// App runs capable of running the game.
            /// </summary>
            Game,

            /// <summary>
            /// User has succeeded.
            /// </summary>
            UserSucceeded
        }

        #region Private member

        /// <summary>
        /// Time span between each angle timer tick.
        /// </summary>
        readonly TimeSpan ANGLE_CHANGE_TIMEPAN = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Minimum possible angle of the hinge.
        /// TODO: Find correct value.
        /// </summary>
        readonly int ANGLE_MIN_VALUE = 0;

        /// <summary>
        /// Maximum possible angle of the hinge.
        /// TODO: Find correct value.
        /// </summary>
        readonly int ANGLE_MAX_VALUE = 360;

        /// <summary>
        /// Angle target which the player has to find.
        /// </summary>
        int angleTarget;

        /// <summary>
        /// Determines if the timer should stop.
        /// </summary>
        bool stopTimer = false;

        /// <summary>
        /// Underlying random engine.
        /// </summary>
        readonly Random random = new Random();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// 
        /// Lesson learned:
        ///     Will be called each time the layout changes. (?!)
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Update Page to current state.
            UpdatePageToState(GetPageState());

            // Listen on hinge value changes.
            FormsWindow.PropertyChanged += FormsWindow_PropertyChanged;

            // Start timer
            Device.StartTimer(ANGLE_CHANGE_TIMEPAN, Timer_Ticked);
        }

        #endregion

        #region Event handler

        /// <summary>
        /// Called on each timer tick. 
        /// Will randomize the target angle value.
        /// </summary>
        /// <returns>True if timer should tick further.</returns>
        private bool Timer_Ticked()
        {
            // Ensure ticker should tick (abort of stop timer is already set)
            if (stopTimer) return true;

            // Get random angle and store.
            angleTarget = random.Next(ANGLE_MIN_VALUE, ANGLE_MAX_VALUE);

            // Update label with three 000 to 360.
            AngleTargetLabel.Text = $"{angleTarget.ToString("D3")}°";

            // Check if timer should be stopped.
            return stopTimer == false;
        }

        /// <summary>
        /// Start button  tapped.
        /// 
        /// Stops the timer and starts the actual game.
        /// </summary>
        /// <param name="sender">Button as sender</param>
        /// <param name="e">Event args.</param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            stopTimer = true;
        }

        private void FormsWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("foo");

            // TODO: Implement "win" logic
            // TODO: Check how to emulate hinge values.
        }

        #endregion

        #region Private helper

        /// <summary>
        /// Gets the page state according to the layout and device type.
        /// </summary>
        /// <returns>Current page state.</returns>
        private PageState GetPageState()
        {
            // It has to be a surface duo.
            if (FormsWindow.IsDuo == false) return PageState.UnsupportedDevice;

            // The app has to be in portrait mode and spanned across both screens.
            if (!FormsWindow.IsPortrait || !FormsWindow.IsSpanned) return PageState.UnsupportedOrientation;

            // If everything is correct, the game is playable.
            return PageState.Game;
        }

        /// <summary>
        /// Updates the page to given state.
        /// </summary>
        /// <param name="state">State to update UI for.</param>
        private void UpdatePageToState(PageState state)
        {
            ResultSuccessTextLabel.IsVisible = state == PageState.UserSucceeded;
            GameStackLayout.IsVisible = state == PageState.Game;
            ErrorStackLayout.IsVisible = state != PageState.Game;
            ErrorLabel.Text = GetErrorMessageForState(state);
        }

        /// <summary>
        /// Gets the error title for given state.
        /// </summary>
        /// <param name="state">Underlying state.</param>
        /// <returns>Error title for state.</returns>
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

        /// <summary>
        /// Gets the error message for given state.
        /// </summary>
        /// <param name="state">Underlying state.</param>
        /// <returns>Error message for state.</returns>
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
