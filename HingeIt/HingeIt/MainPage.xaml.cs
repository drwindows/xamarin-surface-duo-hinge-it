using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Essentials;

namespace HingeIt
{
    /// <summary>
    /// The MainPage contains functionallity for the master and
    /// the detail page.
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
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
        /// Determines if the app is spanned across both screens.
        /// </summary>
        static bool IsSpanned => DualScreenInfo.Current.SpanMode == TwoPaneViewMode.Wide;

        /// <summary>
        /// Determines if the app runs in potrait mode.
        /// </summary>
        static bool IsPortrait => DualScreenInfo.Current.IsLandscape == false;

        /// <summary>
        /// Name of the angle changed event.
        /// </summary>
        readonly string ANGLE_CHANGED_EVENT_NAME = "HingeSensorChanged";

        /// <summary>
        /// Time span between each angle timer tick.
        /// </summary>
        readonly TimeSpan ANGLE_CHANGE_TIMEPAN = TimeSpan.FromMilliseconds(500);

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
        /// Threshold for the target (+-).
        /// </summary>
        readonly int ANGLE_THRESHOLD = 5;

        /// <summary>
        /// Angle target which the player has to find.
        /// </summary>
        int targetAngle;

        /// <summary>
        /// Current angle of the device.
        /// </summary>
        int currentAngle;

        /// <summary>
        /// Determines if the timer should stop.
        /// </summary>
        bool stopTargetRandomizerTimer = false;

        /// <summary>
        /// Hinging start time
        /// </summary>
        DateTime hingingStartTime;

        /// <summary>
        /// Sensor events will be ignored.
        /// </summary>
        bool ingoreSensorEvents = true;

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
            UpdateUi();

            // Add event listener for changed properties.
            DualScreenInfo.Current.PropertyChanged += Current_PropertyChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, ANGLE_CHANGED_EVENT_NAME, (angle) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnHingeSensorChanged(int.Parse(angle));
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, ANGLE_CHANGED_EVENT_NAME);
        }

        #endregion

        #region Event handler

        private void OnHingeSensorChanged(int angle)
        {
            // Store current angle.
            currentAngle = angle;

            if (ingoreSensorEvents) return;

            // Update page.
            UpdateUi();
        }

        /// <summary>
        /// Called on any DualScreenInfo property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check if changed property is a layout related one.
            if (e.PropertyName == "SpanMode")
            {
                UpdateUi();
            }
        }

        /// <summary>
        /// Called on each timer tick. 
        /// Will randomize the target angle value.
        /// </summary>
        /// <returns>True if timer should tick further.</returns>
        private bool Timer_Ticked()
        {
            // Ensure ticker should tick (abort of stop timer is already set)
            if (stopTargetRandomizerTimer) return true;

            // Get random angle and store.
            targetAngle = random.Next(ANGLE_MIN_VALUE, ANGLE_MAX_VALUE);

            // Update label with three 000 to 360.
            AngleTargetLabel.Text = $"{targetAngle:D3}°";

            // Check if timer should be stopped.
            return stopTargetRandomizerTimer == false;
        }

        /// <summary>
        /// Start button tapped.
        /// 
        /// Stops the timer and starts the actual game.
        /// </summary>
        /// <param name="sender">Button as sender</param>
        /// <param name="e">Event args.</param>
        private void StartButton_Clicked(object sender, EventArgs e)
        {
            stopTargetRandomizerTimer = true;
            hingingStartTime = DateTime.Now;
            UpdateUi();
        }

        /// <summary>
        /// Share button tapped.
        /// </summary>
        /// <param name="sender">Button as sender</param>
        /// <param name="e">Event args.</param>
        private void ShareButton_Clicked(object sender, EventArgs e)
        {
            PresentShareSheet();
        }

        #endregion

        #region Private helper

        /// <summary>
        /// Gets the page state according to the layout and device type.
        /// </summary>
        /// <returns>Current page state.</returns>
        private PageState GetPageState()
        {
            // Ensure the app runs on a Surface Duo.
            if (DualScreenInfo.Current.HingeBounds == null) return PageState.UnsupportedDevice;

            // Ensure that the app is in portrait mode and spanned across both screens.
            if (!IsSpanned || !IsPortrait) return PageState.UnsupportedOrientation;

            // Check if current angle is within target threshold
            if (currentAngle != 0 && currentAngle >= targetAngle - ANGLE_THRESHOLD && currentAngle <= targetAngle + ANGLE_THRESHOLD) return PageState.UserSucceeded;

            // If everything is correct, the game is playable.
            return PageState.Game;
        }

        /// <summary>
        /// Updates the page.
        /// </summary>
        private void UpdateUi()
        {
            // Get current state.
            var state = GetPageState();

            // Update controls.
            ResultSuccessTextLabel.IsVisible = state == PageState.UserSucceeded;
            GameStackLayout.IsVisible = state == PageState.Game;
            WinStackLayout.IsVisible = state == PageState.UserSucceeded;
            ErrorStackLayout.IsVisible = state != PageState.Game;
            ErrorTitleLabel.Text = GetErrorTitleForState(state);
            ErrorMessageLabel.Text = GetErrorMessageForState(state);
            BackgroundView.BackgroundGradientStops = GetGradientForState(state);
            ingoreSensorEvents = true;

            // Check for diffrent state combinations.
            if (state == PageState.Game && !stopTargetRandomizerTimer)
            {
                // Start timer.
                Device.StartTimer(ANGLE_CHANGE_TIMEPAN, Timer_Ticked);
            }
            else if (state == PageState.Game && stopTargetRandomizerTimer)
            {
                ingoreSensorEvents = false;
                ResultAngleLabel.Text = $"{currentAngle:D3}°";
                BackgroundView.BackgroundGradientStops = GetGradientCurrentAngle();
            }
            else if (state == PageState.UserSucceeded)
            {

                ResultAngleLabel.Text = $"{targetAngle:D3}°";
                DurationLabel.Text = $"{(DateTime.Now - hingingStartTime).Seconds}s";
            }
            else
            {
                ResultAngleLabel.Text = "? °";
            }
        }

        /// <summary>
        /// Gets the background gradient stops according to the current angle.
        /// </summary>
        /// <returns>Gradient stops according to the current an angle.</returns>
        private Xamarin.Forms.PancakeView.GradientStopCollection GetGradientCurrentAngle()
        {
            if (Math.Abs(targetAngle - currentAngle) < 45)
            {
                return new Xamarin.Forms.PancakeView.GradientStopCollection
                        {
                            new Xamarin.Forms.PancakeView.GradientStop { Color = Color.DarkOrange, Offset = 0 },
                            new Xamarin.Forms.PancakeView.GradientStop { Color = Color.OrangeRed, Offset = 0.5f }
                        };
            }
            else
            {
                return new Xamarin.Forms.PancakeView.GradientStopCollection
                        {
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.IndianRed, Offset = 0 },
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.DarkRed, Offset = 0.5f }
                        };
            }
        }

        /// <summary>
        /// Gets the background gradient stops according to the current state.
        /// </summary>
        /// <returns>Gradient stops according to the current an state.</returns>
        private Xamarin.Forms.PancakeView.GradientStopCollection GetGradientForState(PageState state)
        {
            switch(state)
            {
                case PageState.UnsupportedDevice:
                case PageState.UnsupportedOrientation:
                    return new Xamarin.Forms.PancakeView.GradientStopCollection
                    {
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.Gray, Offset = 0 },
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.DarkGray, Offset = 0.5f }
                    };

                case PageState.Game:
                    return GetGradientCurrentAngle();

                case PageState.UserSucceeded:
                    return new Xamarin.Forms.PancakeView.GradientStopCollection
                    {
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.LightSeaGreen, Offset = 0 },
                        new Xamarin.Forms.PancakeView.GradientStop { Color = Color.DarkOliveGreen, Offset = 0.5f }
                    };
            }

            return new Xamarin.Forms.PancakeView.GradientStopCollection();
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

        /// <summary>
        /// Presents async a share sheet with a win message.
        /// </summary>
        private async void PresentShareSheet()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Share your score!",
                Text = $"It took only {DurationLabel.Text} to hinge my Surface Duo correctly! #HingeIt"
            });
        }

        #endregion
    }
}
