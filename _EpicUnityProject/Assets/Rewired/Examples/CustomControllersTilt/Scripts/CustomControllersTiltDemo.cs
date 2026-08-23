// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

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

#if UNITY_2022_PLUS || (UNITY_2021_PLUS && (UNITY_2021_2 || UNITY_2021_3 || UNITY_2021_2_OR_NEWER || UNITY_2021_3_OR_NEWER))
#define UNITY_2021_2_PLUS
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

#if UNITY_2021_2_PLUS && ENABLE_INPUT_SYSTEM
#define SUPPORTS_UNITY_INPUT_SYSTEM
#endif

namespace Rewired.Demos {

    using UnityEngine;
    using System.Collections;
    using Rewired;
    
    /* IMPORTANT NOTE: Basic tilt control is now available through using the Tilt Control component. Please see
     * the documentation on Touch Controls for more information: http://guavaman.com/rewired/docs/TouchControls.html
     * 
     * This is a simple demo to show how to use a Custom Controller to handle tilt input on a mobile device
     */

    [AddComponentMenu("")]
    public class CustomControllersTiltDemo : MonoBehaviour {

        public Transform target; // the object that will be moving -- the cube in this case
        public float speed = 10.0F;
        private CustomController controller;
        private Player player;

        void Awake() {
#if UNITY_2021_2_OR_NEWER
            Screen.orientation = ScreenOrientation.LandscapeLeft;
#else
            Screen.orientation = ScreenOrientation.Landscape;
#endif
            player = ReInput.players.GetPlayer(0); // get the Rewired Player
            ReInput.InputSourceUpdateEvent += OnInputUpdate; // subscribe to input update event
            controller = (CustomController)player.controllers.GetControllerWithTag(ControllerType.Custom, "TiltController"); // get the Custom Controller from the player by the Tag set in the editor
        }

        void Update() {
            if(target == null) return;

            Vector3 dir = Vector3.zero;

            // Get the tilt vectors using Action names
            dir.y = player.GetAxis("Tilt Vertical");
            dir.x = player.GetAxis("Tilt Horizontal");

            if(dir.sqrMagnitude > 1) dir.Normalize();

            dir *= Time.deltaTime;
            target.Translate(dir * speed);
        }


        /// <summary>
        /// This will be called each time input updates. Use this to push values into the Custom Controller axes.
        /// </summary>
        private void OnInputUpdate() {
            Vector3 acceleration;
            
#if SUPPORTS_UNITY_INPUT_MANAGER

            // Get the acceleration values from UnityEngine.Input and push into the controller
            acceleration = UnityEngine.Input.acceleration;

#elif SUPPORTS_UNITY_INPUT_SYSTEM

            // Get the acceleration values from UnityEngine.Input and push into the controller
            UnityEngine.InputSystem.Accelerometer accelerometer = UnityEngine.InputSystem.Accelerometer.current;
            acceleration = accelerometer != null ? accelerometer.acceleration.value : new Vector3();
            
#else
            acceleration = new Vector3();
#endif
            controller.SetAxisValue(0, acceleration.x);
            controller.SetAxisValue(1, acceleration.y);
            controller.SetAxisValue(2, acceleration.z);
        }
    }
}