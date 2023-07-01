using UnityEngine;

public class TestValueManager : MonoBehaviour
{

    //how do doing attributes
    //[debugfloat("valuename", KeyCode.U, KeyCode.J, 5)] on any static value elsewhere
    //would be neat

    private float _tim;
    private float _holdTime = 0.4f;

    //compiler flags when
    private bool _testingEnabled => false;

    public static float value1 = 0.5f;
    public static float value2 = 0.6f;

    public static float value3 = 0.7f;
    public static float value4 = 0.1f;

    public static float value5 = 0.25f;
    public static float value6 = 0.15f;

    void Update()
    {
        if (!_testingEnabled)
            return;

        if (!Input.GetKey(KeyCode.LeftAlt))
            return;

        manageTestValue(ref value4, "exec", KeyCode.Keypad2, KeyCode.Keypad3, 0.01f);

        manageTestValue(ref value1, "cancel", KeyCode.Keypad7, KeyCode.Keypad4, 0.01f);
        manageTestValue(ref value2, "run", KeyCode.Keypad8, KeyCode.Keypad5, 0.01f);
        manageTestValue(ref value3, "exit", KeyCode.Keypad9, KeyCode.Keypad6, 0.01f);

        manageTestValue(ref value5, "time1", KeyCode.P, KeyCode.Semicolon, 0.01f);
        manageTestValue(ref value6, "timeempowered", KeyCode.LeftBracket, KeyCode.Quote, 0.01f);
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
        SkillsButEpic.Log.Warning($"{print}: {value.ToString("0.000")}");
        return value;
    }
}
