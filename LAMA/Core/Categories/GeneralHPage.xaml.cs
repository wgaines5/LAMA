using LAMA.Core.Messages;

namespace LAMA.Core.Categories
{
    public partial class GeneralHPage : ContentPage
    {
        public GeneralHPage()
        {
            InitializeComponent();
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {
            string question = QuestionEntry.Text;
            if (string.IsNullOrWhiteSpace(question))
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(InboxPage), true,
                new Dictionary<string, object> { { "Question", question } });

            QuestionEntry.Text = ""; // clear input
        }
    }
}