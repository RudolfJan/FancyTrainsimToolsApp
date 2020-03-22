// Created by:  Rudolf Heijink
// Copyright:  (C) 2020 Rudolf Heijink
// Created date: --
// File: IViewAware.cs
// Project: Mvvm.Library

using System.Windows;

namespace Mvvm.Library
  {
  public interface IViewAware
    {
    Window View { get; set; }
    }
  }
