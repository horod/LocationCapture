using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowAsync(string message)
        {
            await Shell.Current.DisplayAlert("", message, "OK");
        }

        public async Task<ConfirmationAnswer> ShowConfirmationAsync(string question)
        {
            bool answer = await Shell.Current.DisplayAlert("", question, "OK", "Cancel");

            return answer ? ConfirmationAnswer.OK : ConfirmationAnswer.Cancel;
        }
    }
}
