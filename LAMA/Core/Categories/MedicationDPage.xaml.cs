using LAMA.Core.Messages;

namespace LAMA.Core.Categories
{
    public partial class MedicationDPage : ContentPage
    {
        public MedicationDPage()
        {
            InitializeComponent();
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {
            Entry questionEntry = (Entry)FindByName("QuestionEntry");

            if (questionEntry != null && !string.IsNullOrWhiteSpace(questionEntry.Text))
            {
                string questionText = questionEntry.Text;
                questionEntry.Text = ""; // clear input field

                await Shell.Current.GoToAsync($"{nameof(InboxPage)}?Question={Uri.EscapeDataString(questionText)}");
            }
        }
    }
}