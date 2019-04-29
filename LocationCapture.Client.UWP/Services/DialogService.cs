using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using LocationCapture.Client.MVVM.Services;

namespace LocationCapture.Client.UWP.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowAsync(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public async Task<ConfirmationAnswer> ShowConfirmationAsync(string question)
        {
            var dialog = new MessageDialog(question);
            dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
            var answer = (await dialog.ShowAsync()).Label;
            return (answer == "OK") ? ConfirmationAnswer.OK : ConfirmationAnswer.Cancel;
        }
    }
}
