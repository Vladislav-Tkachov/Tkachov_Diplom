namespace Diplom.Services;

public class ToastService
{
    public event Action<string> OnShow;

    public void ShowSuccess(string message)
    {
        OnShow?.Invoke(message);
    }

    public void ShowError(string message)
    {
        OnShow?.Invoke(message);
    }
}