﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FancyTrainsimTools.Desktop.Helpers
	{
	public static class LinqMethods
		{

		// https://extensionmethod.net/csharp/ienumerable-t/toobservablecollection-t
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
			{
			var c = new ObservableCollection<T>();
			foreach (var e in coll)
				c.Add(e);
			return c;
			}

		//TODO get this working somehow
		public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> coll)
			{
			var c = new BindableCollection<T>();
			foreach (var e in coll)
				c.Add(e);
			return c;
			}

	
		}
	}
