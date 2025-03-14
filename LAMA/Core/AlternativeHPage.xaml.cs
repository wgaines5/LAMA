using LAMA.Core.Messages;

namespace LAMA.Core
{
    public partial class AlternativeHPage : ContentPage
    {
        public AlternativeHPage()
        {
            InitializeComponent();
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {
            Entry questionEntry = (Entry)FindByName("QuestionEntry");

            if (questionEntry != null && !string.IsNullOrWhiteSpace(questionEntry.Text))
            {
                string questionText = questionEntry.Text;
                questionEntry.Text = ""; // clear input after sending

                await Shell.Current.GoToAsync($"{nameof(InboxPage)}?Question={Uri.EscapeDataString(questionText)}");
            }
        }
    }
}