using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VowScriptHelper.Core;

namespace VowScriptHelper.MVVM.ViewModel
{
    internal class MainViewModel :ObservableObject
    {

		public RelayCommand HomeViewCommand { get; set; }
		public RelayCommand ScriptCleanerViewCommand { get; set; }
		public RelayCommand CodeGeneratorViewCommand { get; set; }

		public HomeViewModel HomeVM { get; set; }
		public ScriptCleanerViewModel ScriptCleanerVM { get; set; }
		public CodeGeneratorViewModel CodeGeneratorVM { get; set; }

		private object _currentView;

		public object CurrentView
		{
			get { return _currentView; }
			set {
				_currentView = value;
				onPropertyChanged();
			}
		}

		public MainViewModel()
		{
			HomeVM = new HomeViewModel();
			ScriptCleanerVM = new ScriptCleanerViewModel(); 
			CodeGeneratorVM = new CodeGeneratorViewModel();
			CurrentView = HomeVM;

			HomeViewCommand = new RelayCommand(o =>
			{
				CurrentView = HomeVM;
			});

			ScriptCleanerViewCommand = new RelayCommand(o =>
			{
				CurrentView = ScriptCleanerVM;
			});

			CodeGeneratorViewCommand = new RelayCommand (o =>
			{
				CurrentView = CodeGeneratorVM;
			});
		}



    }
}
