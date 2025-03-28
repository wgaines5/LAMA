using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LAMA.Core;

public partial class MedFactsPage : ContentPage
{
	public MedFactsPage()
	{
		InitializeComponent();
		BindingContext = new MedFactsViewModel();
	}
}
 
public class MedFactsViewModel : BindableObject
{
	//
	// This class is the binding context for the Collection View.
	//
	public ObservableCollection<MedFact> medFacts { get; set; }
	public Command<MedFact> ToggleBookmarkCommand { get; }

	public MedFactsViewModel()
	{
		medFacts = new ObservableCollection<MedFact>()
		{
			new MedFact { Text = "The human brain contains about 86 billion neurons." },
			new MedFact { Text = "Your heart beats around 100,000 times a day." },
			new MedFact { Text = "The average adult human body is made up of 60% water." },
			new MedFact { Text = "Bones are about five times stronger than steel of the same density." },
            new MedFact { Text = "The skin is the largest organ of the human body." },
            new MedFact { Text = "According to the World Health Organization, 5% of adults struggle with depression." },
            new MedFact { Text = "According to the World Health Organization an estimated 13% of people aged 15-49 have HSV-2." }
        };

		ToggleBookmarkCommand = new Command<MedFact>(ToggleBookmark);
	}
	public event PropertyChangedEventHandler PropertyChanged;

	private void ToggleBookmark(MedFact medFact)
	{
		if (medFact != null)
		{
			medFact.IsBookmarked = !medFact.IsBookmarked;
			OnPropertyChanged(nameof(medFacts));
		}
	}
}

public class MedFact : BindableObject
{
	private string _text;
	private bool _isBookmarked;

	public string Text
	{
		get { return _text; }
		set
		{
			_text = value;
			OnPropertyChanged(nameof(Text));
		}
	}

	public bool IsBookmarked
	{
		get { return _isBookmarked; }

		set
		{
			if (_isBookmarked != value)
			{
				_isBookmarked = value;
				OnPropertyChanged(nameof(IsBookmarked));
			}
		}
	}
}