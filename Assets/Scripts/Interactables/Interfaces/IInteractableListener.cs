namespace GGJ.Interactables
{
    public interface IInteractableListener
    {
        void OnEnterInteractRange(IInteractable interactable);
        void OnExitInteractRange(IInteractable interactable);
    }
}