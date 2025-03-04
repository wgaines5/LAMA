namespace LAMA.Core
{
    public partial class ChronicAPage : ContentPage
    {
        public ChronicAPage()
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

                await Shell.Current.GoToAsync($"{nameof(MessagesPage)}?Question={Uri.EscapeDataString(questionText)}");
            }
        }
    }
}