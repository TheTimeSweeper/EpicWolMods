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

#pragma warning disable 0649 // disable warnings about unused variables

namespace Rewired.Demos.CustomPlatform {

    /// <summary>
    /// Example custom platform manager.
    /// Add this component to the Rewired Input Manager GameObject or a child GameObject and Rewired
    /// will set the custom platform during initialization.
    /// The first active and enabled component that implements <see cref="Rewired.Platforms.Custom.ICustomPlatformInitializer"/>
    /// found that returns a non-null <see cref="Rewired.Platforms.Custom.CustomPlatformInitOptions"/> will be used by
    /// Rewired during initialization to set the custom platform.
    /// </summary>
    public sealed class CustomPlatformManager : UnityEngine.MonoBehaviour, Rewired.Platforms.Custom.ICustomPlatformInitializer {

        /// <summary>
        /// Provides custom platform joystick definition maps.
        /// </summary>
        public CustomPlatformHardwareJoystickMapProvider mapProvider;

        /// <summary>
        /// Gets the custom platform options to initialize a custom platform.
        /// Called during Rewired initialization.
        /// Return null to not use a custom platform.
        /// </summary>
        /// <returns>Custom platform init options</returns>
        public Rewired.Platforms.Custom.CustomPlatformInitOptions GetCustomPlatformInitOptions() {
        
#if SUPPORTS_UNITY_INPUT_MANAGER

            // You can use #if conditionals or other means to determine which custom platform to initialize, if any.

            // Create platform options
            var options = new Rewired.Platforms.Custom.CustomPlatformInitOptions();

            // Set the platform id
            options.platformId = (int)CustomPlatformType.MyPlatform;

            // Set the platform identifier string
            options.platformIdentifierString = "MyPlatform";

            // Set the map provider
            options.hardwareJoystickMapCustomPlatformMapProvider = mapProvider;

            // Create platform configuration values
            var configVars = new Rewired.Platforms.Custom.CustomPlatformConfigVars() {
                ignoreInputWhenAppNotInFocus = true,
                useNativeKeyboard = true,
                useNativeMouse = true
            };

            // Create and set the input source
            options.inputSource = new MyPlatformInputSource(configVars);
            
            return options;
            
#else
            UnityEngine.Debug.LogError("Rewired: The Custom Platform example requires Unity Input Manager to be enabled. This can be changed at Unity Player Settings -> Active Input Handling.");
            return null;
#endif
        }
    }

}
