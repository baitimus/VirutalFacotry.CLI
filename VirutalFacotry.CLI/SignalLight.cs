// SignalLight.cs
using System;

public class SignalLight
{
    // Define the possible colors of the signal light
    public enum LightColor
    {
        Green,
        Yellow,
        Red
    }

    // Current color of the light
    private LightColor _currentColor;

    // Reference to the display
    private ConsoleDisplay _display;

    public SignalLight(ConsoleDisplay display)
    {
        _currentColor = LightColor.Yellow; // Default to yellow (Ready state)
        _display = display;
        _display.UpdateSignalLight(_currentColor);
    }

    // Property to get/set the current light color
    public LightColor CurrentColor
    {
        get { return _currentColor; }
        set
        {
            if (_currentColor != value)
            {
                _currentColor = value;
                _display.UpdateSignalLight(_currentColor);
            }
        }
    }

    // Updates the light based on machine state
    public void UpdateFromMachineState(Machine.State machineState)
    {
        switch (machineState)
        {
            case Machine.State.Ready:
                CurrentColor = LightColor.Yellow;
                break;
            case Machine.State.Running:
                CurrentColor = LightColor.Green;
                break;
            case Machine.State.Error:
                CurrentColor = LightColor.Red;
                break;
        }
    }
}