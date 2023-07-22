using UnityEngine;

internal class TestValueManager : MonoBehaviour
{

    //how do doing attributes
    //[debugfloat("valuename", KeyCode.U, KeyCode.J, 5)] on any static value elsewhere
    //would be neat
    
    private float _tim;
    private float _holdTime = 0.4f;

    //compiler flags when
    public static bool testingEnabled = false;
    
    public static float value1 = 0.05f;
    public static float value2 = 0.2f;

    public static float value3 = 0.5f;
    public static float value4 = 0.1f;

    public static float value5 = 0.25f;
    public static float value6 = 0.15f;

    void Update()
    {
        if (!testingEnabled)
            return;

        if (!Input.GetKey(KeyCode.LeftAlt))
            return;

        manageTestValue(ref value1, "shorten", KeyCode.Keypad7, KeyCode.Keypad4, 0.01f);
        manageTestValue(ref value2, "slkide", KeyCode.Keypad8, KeyCode.Keypad5, 0.01f);
        manageTestValue(ref value3, "upgraded", KeyCode.Keypad9, KeyCode.Keypad6, 1);

        //manageTestValue(ref value5, "time1", KeyCode.P, KeyCode.Semicolon, 0.01f);
        //manageTestValue(ref value6, "timeempowered", KeyCode.LeftBracket, KeyCode.Quote, 0.01f);
    }

    private void manageTestValue(ref float value, string valueName, KeyCode upKey, KeyCode downKey, float incrementAmount)
    {

        if (Input.GetKeyDown(upKey))
        {

            value = setTestValue(value + incrementAmount, valueName);
        }

        if (Input.GetKeyDown(downKey))
        {

            value = setTestValue(value - incrementAmount, valueName);
        }


        if (Input.GetKey(upKey) || Input.GetKey(downKey))
        {

            float amount = incrementAmount * (Input.GetKey(upKey) ? 1 : -1);

            _tim += Time.deltaTime;

            if (_tim > _holdTime)
            {
                _tim = _holdTime - 0.02f;
                value = setTestValue(value + amount, valueName);
            }
        }


        if (Input.GetKeyUp(upKey) || Input.GetKeyUp(downKey))
        {
            _tim = 0;
        }

    }

    private float setTestValue(float value, string print)
    {
        Log.Warning($"{print}: {value.ToString("0.000")}");
        return value;
    }
}
