// GENERATED AUTOMATICALLY FROM 'Assets/Input/CameraControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Elektronik.Input
{
    public class @CameraControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CameraControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraControls"",
    ""maps"": [
        {
            ""name"": ""Controls"",
            ""id"": ""3296de75-f043-48f4-b597-b0057bb2af6b"",
            ""actions"": [
                {
                    ""name"": ""Move Forward"",
                    ""type"": ""Value"",
                    ""id"": ""e24ac579-c14e-4cc3-8d8c-2050856141c0"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Sides"",
                    ""type"": ""Value"",
                    ""id"": ""6fde4b62-094a-4702-beb7-2a3c4c811732"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""ScaleVector2(x=5,y=5)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""c6e8d5a2-225f-44ab-bfc8-926cb03ce48d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Value"",
                    ""id"": ""82748281-e420-40ac-b2a5-ada86d74f579"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""e56fad53-e5b3-4ab9-bf9d-41b347ab0cda"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""f801220f-f4a9-468f-a19a-329159d9de18"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""c1dcd14f-1171-4a22-b6e0-f2ed1a4bf346"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""cb6d741f-e8b8-4696-8e83-b92f9a7073f5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""d4f9aa70-0909-4bdd-a467-edf74852d87f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e21b39f6-208b-4b06-a406-247ba1d88059"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4ad80931-4371-4a50-977a-c960823ec543"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8d35484f-01be-4c01-8c10-4e67651e1527"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""Move Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""518a5f76-4951-4875-9ce9-b082c52e44b5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""208219a5-5e3f-4f39-b312-68b84540a685"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cf8cbcac-bee6-4127-b905-525ea1d97636"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6ac52c8d-53e4-41a1-869b-5180f77ddf38"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f77d55e2-f6e5-40ef-a1e9-10d349c64470"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Sides"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b6370aae-aa65-4980-946e-89eaef76305f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=50,y=50)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2fb5a899-cbd5-47c8-ac13-88ef09e9d72b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""05e58344-c5eb-4090-8782-c9854558708c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""116a429f-defc-4221-bda4-27f0e867e96e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""40b5d6a8-6ec0-4781-b3f1-ab676df01263"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse Drag"",
                    ""id"": ""1e48f88a-cc4c-4deb-b202-8e44ef84a4e7"",
                    ""path"": ""MouseDrag"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button"",
                    ""id"": ""177df200-e88d-4360-bb85-912de5b11265"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis1"",
                    ""id"": ""263baa66-2769-4ee9-ab61-963948cc978f"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis2"",
                    ""id"": ""79dc4f4d-c856-4eaa-a155-ceaf410559fa"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5),Invert"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a1759788-8537-468d-af90-d6bd991c4dda"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""055c0d1d-9c00-43d1-8be3-cf3d0a22b8b3"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Default"",
            ""id"": ""43f7d51d-be1e-4a53-a233-f4123404cb47"",
            ""actions"": [
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""ada2bf4e-483f-4dea-bf0a-75d09fdfce1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""a792ad56-9862-4821-bc67-3d01cac4345e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""54ca85f0-f733-41aa-8596-4003d74acbd3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59635337-d9ef-4526-a93d-50051d6cf12a"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""242158ca-c282-4009-b032-603d9b1aa2d0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Controls
            m_Controls = asset.FindActionMap("Controls", throwIfNotFound: true);
            m_Controls_MoveForward = m_Controls.FindAction("Move Forward", throwIfNotFound: true);
            m_Controls_MoveSides = m_Controls.FindAction("Move Sides", throwIfNotFound: true);
            m_Controls_Rotate = m_Controls.FindAction("Rotate", throwIfNotFound: true);
            m_Controls_Boost = m_Controls.FindAction("Boost", throwIfNotFound: true);
            m_Controls_Reset = m_Controls.FindAction("Reset", throwIfNotFound: true);
            // Default
            m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
            m_Default_Cancel = m_Default.FindAction("Cancel", throwIfNotFound: true);
            m_Default_Click = m_Default.FindAction("Click", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Controls
        private readonly InputActionMap m_Controls;
        private IControlsActions m_ControlsActionsCallbackInterface;
        private readonly InputAction m_Controls_MoveForward;
        private readonly InputAction m_Controls_MoveSides;
        private readonly InputAction m_Controls_Rotate;
        private readonly InputAction m_Controls_Boost;
        private readonly InputAction m_Controls_Reset;
        public struct ControlsActions
        {
            private @CameraControls m_Wrapper;
            public ControlsActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveForward => m_Wrapper.m_Controls_MoveForward;
            public InputAction @MoveSides => m_Wrapper.m_Controls_MoveSides;
            public InputAction @Rotate => m_Wrapper.m_Controls_Rotate;
            public InputAction @Boost => m_Wrapper.m_Controls_Boost;
            public InputAction @Reset => m_Wrapper.m_Controls_Reset;
            public InputActionMap Get() { return m_Wrapper.m_Controls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ControlsActions set) { return set.Get(); }
            public void SetCallbacks(IControlsActions instance)
            {
                if (m_Wrapper.m_ControlsActionsCallbackInterface != null)
                {
                    @MoveForward.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveForward.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveForward.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveForward;
                    @MoveSides.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @MoveSides.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @MoveSides.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMoveSides;
                    @Rotate.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Rotate.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Rotate.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnRotate;
                    @Boost.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Boost.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Boost.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnBoost;
                    @Reset.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                    @Reset.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                    @Reset.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnReset;
                }
                m_Wrapper.m_ControlsActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveForward.started += instance.OnMoveForward;
                    @MoveForward.performed += instance.OnMoveForward;
                    @MoveForward.canceled += instance.OnMoveForward;
                    @MoveSides.started += instance.OnMoveSides;
                    @MoveSides.performed += instance.OnMoveSides;
                    @MoveSides.canceled += instance.OnMoveSides;
                    @Rotate.started += instance.OnRotate;
                    @Rotate.performed += instance.OnRotate;
                    @Rotate.canceled += instance.OnRotate;
                    @Boost.started += instance.OnBoost;
                    @Boost.performed += instance.OnBoost;
                    @Boost.canceled += instance.OnBoost;
                    @Reset.started += instance.OnReset;
                    @Reset.performed += instance.OnReset;
                    @Reset.canceled += instance.OnReset;
                }
            }
        }
        public ControlsActions @Controls => new ControlsActions(this);

        // Default
        private readonly InputActionMap m_Default;
        private IDefaultActions m_DefaultActionsCallbackInterface;
        private readonly InputAction m_Default_Cancel;
        private readonly InputAction m_Default_Click;
        public struct DefaultActions
        {
            private @CameraControls m_Wrapper;
            public DefaultActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Cancel => m_Wrapper.m_Default_Cancel;
            public InputAction @Click => m_Wrapper.m_Default_Click;
            public InputActionMap Get() { return m_Wrapper.m_Default; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
            public void SetCallbacks(IDefaultActions instance)
            {
                if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
                {
                    @Cancel.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCancel;
                    @Click.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                }
                m_Wrapper.m_DefaultActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                }
            }
        }
        public DefaultActions @Default => new DefaultActions(this);
        public interface IControlsActions
        {
            void OnMoveForward(InputAction.CallbackContext context);
            void OnMoveSides(InputAction.CallbackContext context);
            void OnRotate(InputAction.CallbackContext context);
            void OnBoost(InputAction.CallbackContext context);
            void OnReset(InputAction.CallbackContext context);
        }
        public interface IDefaultActions
        {
            void OnCancel(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
        }
    }
}
