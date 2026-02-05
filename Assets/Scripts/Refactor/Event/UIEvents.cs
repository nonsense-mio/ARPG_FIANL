using HT;

namespace ARPG
{
    public struct AimActionEvent
    {
        public bool IsAiming;
    }

    public struct EnableInputEvent
    {
        public bool Enabled;
    }

    public struct SelectWindowEvent
    {
        public bool IsOpen;
    }

    public struct InteractPromptEvent
    {
        public Interactable Target;
    }
}
