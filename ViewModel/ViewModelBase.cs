using System.ComponentModel;
namespace TMS_Project.ViewModel;

public class ViewModelBase : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;


    /*
    * METHOD NAME: OnPropertyChanged
    * DESCRIPTION: Dyanmically changes the value displaying
    * 
    * RETURN: void
    */
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}