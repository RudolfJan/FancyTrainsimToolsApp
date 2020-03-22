using System.ComponentModel;
using System.Runtime.CompilerServices;

//	Let's create a ViewModel for this MainWindow called MainWindowViewModel.
//We can just create an instance of our ViewModel from XAML and use that to
//set the DataContext property of the window. For this, we need to create a
//base class to encapsulate the implementation of INotifyPropertyChanged for
//our ViewModels.

//The main idea behind this class is to encapsulate the INotifyPropertyChanged
//implementation and provide helper methods to the derived class so that they
//can easily trigger the appropriate notifications. Following is the
//implementation of BindableBase class.

// Property creation:
//public BindableBase CurrentViewModel { 
//get { return _CurrentViewModel; } 
//set { SetProperty(ref _CurrentViewModel, value); } 

// https://www.tutorialspoint.com/mvvm/mvvm_hierarchies_and_navigation.htm

namespace Mvvm.Library
  {
  public class BindableBase : INotifyPropertyChanged { 
	
    protected virtual void SetProperty<T>(ref T member, T val,
      [CallerMemberName] string propertyName = null) { 
      if (object.Equals(member, val)) return;
				
      member = val;
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
      }
			
    protected virtual void OnPropertyChanged(string propertyName) { 
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
      } 
		
    public event PropertyChangedEventHandler PropertyChanged = delegate { }; 
    } 
  }
