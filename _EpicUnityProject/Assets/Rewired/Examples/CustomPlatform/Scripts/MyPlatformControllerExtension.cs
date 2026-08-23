// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_6000_PLUS
#endif

#if UNITY_2023 || UNITY_6000_PLUS
#define UNITY_2023_PLUS
#endif

#if UNITY_2022 || UNITY_2023_PLUS
#define UNITY_2022_PLUS
#endif

#if UNITY_2021 || UNITY_2022_PLUS
#define UNITY_2021_PLUS
#endif

#if UNITY_2020 || UNITY_2021_PLUS
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if !UNITY_2019_PLUS || ENABLE_LEGACY_INPUT_MANAGER
#define SUPPORTS_UNITY_INPUT_MANAGER
#endif

#if SUPPORTS_UNITY_INPUT_MANAGER

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// A controller extension for this custom platform.
    /// This allows supporting special features such as vibration and other custom functionality.
    /// Implementing Rewired.Interfaces.IControllerVibrator allows Rewired's Controller and Player vibration function calls to work.
    /// </summary>
    public sealed class MyPlatformControllerExtension : ControllerExtensions.CustomControllerExtension, Rewired.Interfaces.IControllerVibrator {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceJoystick">Source joystick.</param>
        public MyPlatformControllerExtension(MyPlatformInputSource.Joystick sourceJoystick) : base(new Source(sourceJoystick)) {
        }
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Object to copy</param>
        private MyPlatformControllerExtension(MyPlatformControllerExtension other) : base(other) {
        }

        /// <summary>
        /// Makes a shallow copy of the object.
        /// </summary>
        /// <returns>Shallow copy of the object.</returns>
        public override Controller.Extension ShallowCopy() {
            return new MyPlatformControllerExtension(this);
        }

        // Implement IControllerVibrator interface so Rewired vibration works

        public int vibrationMotorCount {
            get {
                return 2;
            }
        }

        public void SetVibration(int motorIndex, float motorLevel) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel);
        }

        public void SetVibration(int motorIndex, float motorLevel, float duration) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, duration);
        }

        public void SetVibration(int motorIndex, float motorLevel, bool stopOtherMotors) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, stopOtherMotors);
        }

        public void SetVibration(int motorIndex, float motorLevel, float duration, bool stopOtherMotors) {
            ((Source)GetSource()).sourceJoystick.SetVibration(motorIndex, motorLevel, duration, stopOtherMotors);
        }

        public float GetVibration(int motorIndex) {
            return ((Source)GetSource()).sourceJoystick.GetVibration(motorIndex);
        }

        public void StopVibration() {
            ((Source)GetSource()).sourceJoystick.StopVibration();
        }

        class Source : Rewired.Interfaces.IControllerExtensionSource {

            public readonly MyPlatformInputSource.Joystick sourceJoystick;

            public Source(MyPlatformInputSource.Joystick sourceJoystick) {
                this.sourceJoystick = sourceJoystick;
            }
        }
    }
}

#endif
