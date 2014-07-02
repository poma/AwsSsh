using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace AwsSsh
{
	public class Settings : SettingsBase, INotifyPropertyChanged
	{
		[DefaultValue("C:\\putty.exe")]
		public string PuttyPath { get; set; }

		[DefaultValue("")]
		public string PuttySession { get; set; }

		[DefaultValue("")]
		public string CommandLineArgs { get; set; }

		[DefaultValue(true)]
		public bool CloseOnConnect { get; set; }

		[DefaultValue("C:\\certificate.ppk")]
		public string KeyPath { get; set; }

		[DefaultValue(10)]
		public int UpdateInterval { get; set; }

		[DefaultValue(true)]
		public bool IsFirstTimeConfiguration { get; set; }

		[DefaultValue(250)]
		public int WindowWidth { get; set; }

		[DefaultValue(600)]
		public int WindowHeight { get; set; }

		[DefaultValue(false)]
		public bool ShowPuttyButton
		{
			get
			{
				return showPuttyButton;
			}
			set
			{
				if (showPuttyButton == value) return;
				showPuttyButton = value;
				OnPropertyChanged("ShowPuttyButton");
			}
		}

		private bool showPuttyButton;
        private bool includePuttySessionsInList;
		[DefaultValue(true)]
		public bool IncludePuttySessionsInList
		{
			get
			{
				return includePuttySessionsInList;
			}
			set
			{
				if (includePuttySessionsInList == value) return;
				includePuttySessionsInList = value;
				OnPropertyChanged("IncludePuttySessionsInList");
			}
		}

		private List<SettingsBase> _instanceSourcesSettings = new List<SettingsBase>();
		public List<SettingsBase> InstanceSourcesSettings
		{
			get { return _instanceSourcesSettings; }
			set { _instanceSourcesSettings = value; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}
}
