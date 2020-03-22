using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

// This solution is not fully correct
// https://stackoverflow.com/questions/4588359/implementing-collectionchanged

//  https://github.com/gsonnenf/ExtendedObservableCollection
// https://github.com/IgorBuchelnikov/ObservableComputations

namespace Mvvm.Library
  {
  class NotifyCollectionChanged : INotifyCollectionChanged
    {
    /// <summary>Occurs when the collection changes.</summary>
    /// <returns></returns>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
      throw (new NotImplementedException());
      }
    }
  }
