
using LAMA.Auth;
using LAMA.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace LAMA.Core.Messages;


public partial class MPInbox : ContentPage
{
	
	public MPInbox()
	{
		InitializeComponent();
		BindingContext = new MPInboxViewModel(new FirebaseService());
	}

	private async void OnSessionSelected(object sender, SelectionChangedEventArgs e)
	{
		//if (e.CurrentSelection.FirstOrDefault() is Conversation selectedSession)
		//{
		//	((CollectionView)sender).SelectedItems = null;

		//	await Navigation.PushAsync(new MessagePage(selectedSession));
		//}
	}
}

public class MPInboxViewModel : BindableObject
{
	private readonly FirebaseService _firebaseService;

	public ObservableCollection<Conversation> Conversations { get; set; } = new();

	public ICommand ClaimedConversationsCommand { get; }

	public MPInboxViewModel(FirebaseService fService)
	{
		_firebaseService = fService;

		ClaimedConversationsCommand = new Command<Conversation>(async (session) => await ClaimConversationAsync(session));

        Task.Run(async () => await LoadConversationsAsync());
	}

	private async Task LoadConversationsAsync()
	{
		//var sessions = await _firebaseService.GetUnassignedAsync();
		//MainThread.BeginInvokeOnMainThread(() =>
		//{
		//	Conversations.Clear();
		//	foreach (var conversation in sessions)
		//	{
		//		Conversations.Add(conversation);
		//	}
		//});
	}

	private async Task ClaimConversationAsync(Conversation conversation)
	{
		//conversation.ProviderId = UserSession.Credential.User.Uid;
		//await _firebaseService.AssignProviderAsync(conversation);
		//Conversations.Remove(conversation);

		// Navigate to messages page

	}
}