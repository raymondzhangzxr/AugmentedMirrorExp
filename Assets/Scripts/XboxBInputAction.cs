using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;



public class XboxBInputAction : BaseInputHandler, IMixedRealityInputHandler<float>
{
    public MixedRealityInputAction XboxAction;
    public GameObject textOb;
    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);

    }
    protected override void UnregisterHandlers()
    {

        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);

    }

    public void OnInputChanged(InputEventData<float> eventData)
    {
        /*JoystickButton0 - X
          JoystickButton1 - A
          JoystickButton2 - B
          JoystickButton3 - Y*/
        
        TextMesh t = textOb.GetComponent<TextMesh>();

        if (eventData.MixedRealityInputAction == XboxAction && eventData.InputData > 0)
        {
            t.text = "XboxB is pressed";
        }

    }
}
