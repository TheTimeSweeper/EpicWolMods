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
    /// An example custom mouse input source that wraps UnityEngine.Input.
    /// </summary>
    public class MyPlatformUnifiedMouseSource : Rewired.Platforms.Custom.CustomPlatformUnifiedMouseSource {

        /// <summary>
        /// Mouse screen position in pixels.
        /// </summary>
        public override UnityEngine.Vector2 mousePosition {
            get {
                return UnityEngine.Input.mousePosition;
            }
        }

        /// <summary>
        /// Called once per enabled update loop frame.
        /// </summary>
        protected override void Update() {

            // Update input values

            // Set axis values
            // Mouse axis count is fixed. Can be obtained from this.axisCount.
            SetAxisValue(0, UnityEngine.Input.GetAxis("MouseAxis1"));
            SetAxisValue(1, UnityEngine.Input.GetAxis("MouseAxis2"));
            SetAxisValue(2, UnityEngine.Input.GetAxis("MouseAxis3"));

            // Set button values
            // Mouse button count is fixed. Can be obtained from this.buttonCount.
            SetButtonValue(0, UnityEngine.Input.GetButton("MouseButton0"));
            SetButtonValue(1, UnityEngine.Input.GetButton("MouseButton1"));
            SetButtonValue(2, UnityEngine.Input.GetButton("MouseButton2"));
            SetButtonValue(3, UnityEngine.Input.GetButton("MouseButton3"));
            SetButtonValue(4, UnityEngine.Input.GetButton("MouseButton4"));
            SetButtonValue(5, UnityEngine.Input.GetButton("MouseButton5"));
            SetButtonValue(6, UnityEngine.Input.GetButton("MouseButton6"));
        }
    }
}

#endif
