using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

// We will add validation support to our input view,
// and in large application you will probably need this a number of places in your application.
// Sometimes on Views, sometimes on ViewModels and sometimes on these helper objects there
// are wrappers around model objects.

//  It’s a good practice for putting the validation support in a common base class that you
// can then inherit from different scenarios.

//The base class will support INotifyDataErrorInfo so that that validation gets triggered
//when properties change.

//Create add a new class called ValidatableBindableBase. Since we already have a base class
//for a property change handling, let’s derive the base class from it and also implement
//the INotifyDataErrorInfo interface.


// https://www.tutorialspoint.com/mvvm/mvvm_validations.htm

namespace Mvvm.Library
  {

  public class ValidatingBindableBase : BindableBase, INotifyDataErrorInfo
    {
    private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    public event EventHandler<DataErrorsChangedEventArgs>
      ErrorsChanged = delegate { };

    public System.Collections.IEnumerable GetErrors(string propertyName)
      {

      if (_errors.ContainsKey(propertyName))
        return _errors[propertyName];
      else
        return null;
      }

    public bool HasErrors
      {
      get { return _errors.Count > 0; }
      }

    protected override void SetProperty<T>(ref T member, T val,
      [CallerMemberName] string propertyName = null)
      {

      base.SetProperty<T>(ref member, val, propertyName);
      ValidateProperty(propertyName, val);
      }

    private void ValidateProperty<T>(string propertyName, T value)
      {
      var results = new List<ValidationResult>();

      //ValidationContext context = new ValidationContext(this); 
      //context.MemberName = propertyName;
      //Validator.TryValidateProperty(value, context, results);

      if (results.Any())
        {
        //_errors[propertyName] = results.Select(c => c.ErrorMessage).ToList(); 
        }
      else
        {
        _errors.Remove(propertyName);
        }

      ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
      }
    }
  }
