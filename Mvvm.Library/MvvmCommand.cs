using System;
using System.Windows.Input;

// https://www.tutorialspoint.com/mvvm/mvvm_view_viewmodel_communication.htm

// Example wiring an button in the view:

  //<Button Content = "Delete" 
  //Command = "{Binding DeleteCommand}" 
  //HorizontalAlignment = "Left" 
  //VerticalAlignment = "Top" 
  //Width = "75" />

// In the ViewModel add a property and create the command:

//public MyICommand DeleteCommand { get; set;} 

//public StudentViewModel() { 
//LoadStudents(); 
//DeleteCommand = new MyICommand(OnDelete, CanDelete); 


// Implement CanDelete and OnDelete in the ViewModel

//private void OnDelete() { 
//Students.Remove(SelectedStudent); 
//}

//private bool CanDelete() { 
//return SelectedStudent != null; 
//}

// In the example a new property is added to the viewmodel, which raises a CanExecute flag:

//private Student _selectedStudent;
 
//public Student SelectedStudent { 
//get { 
//  return _selectedStudent; 
//  } 
	
//set { 
//  _selectedStudent = value;
//  DeleteCommand.RaiseCanExecuteChanged(); 
//  } 
//}

namespace Mvvm.Library

  {
  public class MvvmCommand : ICommand
    {
    Action _TargetExecuteMethod;
    Func<bool> _TargetCanExecuteMethod;

    public MvvmCommand(Action executeMethod)
      {
      _TargetExecuteMethod = executeMethod;
      }

    public MvvmCommand(Action executeMethod, Func<bool> canExecuteMethod)
      {
      _TargetExecuteMethod = executeMethod;
      _TargetCanExecuteMethod = canExecuteMethod;
      }

    public void RaiseCanExecuteChanged()
      {
      CanExecuteChanged(this, EventArgs.Empty);
      }

    bool ICommand.CanExecute(object parameter)
      {

      if (_TargetCanExecuteMethod != null)
        {
        return _TargetCanExecuteMethod();
        }

      if (_TargetExecuteMethod != null)
        {
        return true;
        }

      return false;
      }

    // Beware - should use weak references if command instance lifetime 
    // is longer than lifetime of UI objects that get hooked up to command 

    // Prism commands solve this in their implementation 
    public event EventHandler CanExecuteChanged = delegate { };

    void ICommand.Execute(object parameter)
      {
      _TargetExecuteMethod?.Invoke();
      }
    }
  }
