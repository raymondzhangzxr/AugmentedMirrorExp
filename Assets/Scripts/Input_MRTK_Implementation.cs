using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class Input_MRTK_Implementation :
  InputSystemGlobalHandlerListener, IMixedRealityInputHandler
{
    public GameObject textOb;
    public GameObject startButton;
    [Header("MRTK Input Actions")]
    [SerializeField] private MixedRealityInputAction Start_Button;
    [SerializeField] private MixedRealityInputAction View_Button;
    [SerializeField] private MixedRealityInputAction A_Button;
    [SerializeField] private MixedRealityInputAction B_Button;
    [SerializeField] private MixedRealityInputAction Y_Button;
    [SerializeField] private MixedRealityInputAction X_Button;
    [SerializeField] private MixedRealityInputAction Left_Bumper;
    [SerializeField] private MixedRealityInputAction Right_Bumper;

    [SerializeField] private bool _start;
    [SerializeField] private bool _view;
    [SerializeField] private bool _a;
    [SerializeField] private bool _b;
    [SerializeField] private bool _y;
    [SerializeField] private bool _x;
    [SerializeField] private bool _lb;
    [SerializeField] private bool _rb;

    #region On Input Up/Down
    public void OnInputUp(InputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == Start_Button) _start = false;
        if (eventData.MixedRealityInputAction == View_Button) _view = false;
        if (eventData.MixedRealityInputAction == A_Button) _a = false;
        if (eventData.MixedRealityInputAction == B_Button) _b = false;
        if (eventData.MixedRealityInputAction == Y_Button) _y = false;
        if (eventData.MixedRealityInputAction == X_Button) _x = false;
        if (eventData.MixedRealityInputAction == Left_Bumper) _lb = false;
        if (eventData.MixedRealityInputAction == Right_Bumper) _rb = false;
    }

    public void OnInputDown(InputEventData eventData)
    {
        TextMesh t = textOb.GetComponent<TextMesh>();


        if (eventData.MixedRealityInputAction == Start_Button) _start = true;
        if (eventData.MixedRealityInputAction == View_Button) _view = true;
        if (eventData.MixedRealityInputAction == A_Button)
        {
           
            _a = true;
        }
        if (eventData.MixedRealityInputAction == B_Button)
        {
            
            _b = true;
            
        }
        if (eventData.MixedRealityInputAction == Y_Button)
        {
            
            _y = true;
        }
        if (eventData.MixedRealityInputAction == X_Button) _x = true;
        if (eventData.MixedRealityInputAction == Left_Bumper) _lb = true;
        if (eventData.MixedRealityInputAction == Right_Bumper) _rb = true;
    }
    #endregion

    #region Handlers
    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
    }

    protected override void UnregisterHandlers()
    {

    }
    #endregion
}