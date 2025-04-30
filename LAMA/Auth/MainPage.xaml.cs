using System.Collections.ObjectModel;


namespace LAMA.Auth
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> Categories { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Categories = new ObservableCollection<string>
            {
                "General Health",
                "Mental Health",
                "Sexual Health",
                "Chronic Conditions & Autoimmune",
                "Medication & Drug Interactions",
                "Alternative & Holistic Medicine"
            };
            BindingContext = this;
        }


        private async void OnMPSignupClicked(object sender, EventArgs e) 
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//SignUpPage");
        }

        private async void OnUserSignupClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//UsrSignUp");
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {
            string questionText = QuestionEntry.Text;
            string selectedCategory = CategoryPicker.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(questionText) || string.IsNullOrWhiteSpace(selectedCategory))
            {
                await DisplayAlert("Error", "Please enter a question and select a category.", "OK");
                return;
            }
            QuestionEntry.Text = string.Empty;
            CategoryPicker.SelectedIndex = -1;

            await Shell.Current.GoToAsync($"MessagePage?Question={Uri.EscapeDataString(questionText)}&Category={Uri.EscapeDataString(selectedCategory)}");
        }
    }

}
