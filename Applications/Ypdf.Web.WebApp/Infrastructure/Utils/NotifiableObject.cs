using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ypdf.Web.WebApp.Infrastructure.Utils;

public class NotifiableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? property = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    public bool UpdateField<T>(
        ref T backingField,
        T newValue,
        [CallerMemberName] string? propertyName = null)
    {
        if (Equals(backingField, newValue))
            return false;

        backingField = newValue;
        OnPropertyChanged(propertyName);

        return true;
    }
}
