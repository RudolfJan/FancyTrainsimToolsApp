using Assets.Library.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FancyTrainsimToolsDesktop.ViewModels
	{
	public class InstructionDetailsViewModel : Screen
		{
		public InstructionModel Instruction { get; set; }

		protected override async void OnViewLoaded(object view)
			{
			base.OnViewLoaded(view);
			}

		public async Task Exit()
			{
			await TryCloseAsync();
			}

		}
	}
