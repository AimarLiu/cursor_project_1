using CommunityToolkit.Mvvm.ComponentModel;

namespace CursorTestApp.ViewModels;

public sealed partial class UserPasswordRowViewModel : ObservableObject
{
    public string Id { get; }
    public string Username { get; }

    [ObservableProperty]
    private string _password;

    public UserPasswordRowViewModel(string id, string username, string password)
    {
        Id = id;
        Username = username;
        _password = password;
    }
}
