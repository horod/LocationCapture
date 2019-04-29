using System.Threading.Tasks;

namespace LocationCapture.Client.MVVM.Services
{
    public enum ConfirmationAnswer
    {
        OK,
        Cancel
    }

    public interface IDialogService
    {
        Task ShowAsync(string message);
        Task<ConfirmationAnswer> ShowConfirmationAsync(string question);
    }
}
