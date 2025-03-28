using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LAMA.Core.Profile;

public partial class SavedFacts : ContentPage
{
	public SavedFacts()
	{
		InitializeComponent();
		BindingContext = new SavedFactsViewModel();
	}
}

public class SavedFactsViewModel : BindableObject
{
	private ObservableCollection<MedFact> _savedFacts = new();
	public ObservableCollection<MedFact> SavedFacts => _savedFacts;

	public Command<MedFact> RemoveFactCommand { get; }

    public SavedFactsViewModel()
	{
		_savedFacts = new ObservableCollection<MedFact>(LoadFacts());

		RemoveFactCommand = new Command<MedFact>(RemoveFact);
    }

	private void RemoveFact(MedFact medFact)
	{
		if (medFact != null)
		{
			SavedFacts.Remove(medFact);

            var tempFacts = new ObservableCollection<MedFact>(SavedFacts);
            SavedFacts.Clear();
            foreach (var fact in tempFacts)
            {
                SavedFacts.Add(fact);
            }

            OnPropertyChanged(nameof(SavedFacts));
			SaveFacts();
		} 
	}
	
	private void SaveFacts()
	{

	}

	private ObservableCollection<MedFact> LoadFacts()
	{
		return new ObservableCollection<MedFact>
		{
		new MedFact { Text = "The human brain contains about 86 billion neurons.", IsBookmarked = true },
		new MedFact { Text = "The average adult human body is made up of 60% water.", IsBookmarked = true}
		};
	}
}
