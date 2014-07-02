using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace AwsSsh
{
	[Serializable]
	public class AmazonInstance : Instance
	{
		[CopyProperty]
		public string Id { get; set; }

		#region Properties

		private AmazonInstatnceStates _state;
		[CopyProperty]
		public AmazonInstatnceStates State
		{
			get { return _state; }
			set
			{
				if (_state == value) return;

				if (Enum.IsDefined(typeof(AmazonInstatnceStates), value)) 
					_state = value;
				else
					_state = AmazonInstatnceStates.Unknown;
				
				UpdateStateColor();
				OnPropertyChanged("State");
			}
		}

		private string _publicIp;
		[CopyProperty]
		public string PublicIp
		{
			get { return _publicIp; }
			set
			{
				if (_publicIp == value) return;
				_publicIp = value;
				OnPropertyChanged("PublicIp");
				OnPropertyChanged("Tooltip");
			}
		}

		private string _privateIp;
		[CopyProperty]
		public string PrivateIp
		{
			get { return _privateIp; }
			set
			{
				if (_privateIp == value) return;
				_privateIp = value;
				OnPropertyChanged("PrivateIp");
				OnPropertyChanged("Tooltip");
			}
		}


		private string _instanceType;
		[CopyProperty]
		public string InstanceType
		{
			get { return _instanceType; }
			set
			{
				if (_instanceType == value) return;
				_instanceType = value;
				OnPropertyChanged("InstanceType");
				OnPropertyChanged("Tooltip");
			}
		}

		private string _publicDnsName;
		[CopyProperty]
		public string PublicDnsName
		{
			get { return _publicDnsName; }
			set
			{
				if (_publicDnsName == value) return;
				_publicDnsName = value;
				OnPropertyChanged("PublicDnsName");
				OnPropertyChanged("Tooltip");
			}
		}

		private string _privateDnsName;
		[CopyProperty]
		public string PrivateDnsName
		{
			get { return _privateDnsName; }
			set
			{
				if (_privateDnsName == value) return;
				_privateDnsName = value;
				OnPropertyChanged("PrivateDnsName");
				OnPropertyChanged("Tooltip");
			}
		}


		#endregion

		public override bool Run()
		{
			if (string.IsNullOrEmpty(PublicIp)) return false; // offline instancces
			var session = string.IsNullOrWhiteSpace(App.Settings.PuttySession) ? "" : String.Format("-load \"{0}\"", App.Settings.PuttySession);
			RunPutty(String.Format(@"{0} -ssh {1} -l {2} -i ""{3}"" {4}", session, PublicIp, App.Settings.DefaultUser, App.Settings.KeyPath, App.Settings.CommandLineArgs));
			return true;
		}

		public override string Tooltip
		{
			get
			{
				return String.Format("Instance Type: {0}\n\nAddresses:\nPublic IP: {1}\nPrivate IP: {2}\nPublic DNS: {3}\nPrivate DNS: {4}", InstanceType, PublicIp, PrivateIp, PublicDnsName, PrivateDnsName);
			}
		}

		public override string ClipboardText
		{
			get { return PublicIp; }
		}

		public override string GetId()
		{
			return Id;
		}

		private void UpdateStateColor()
		{
			switch (State)
			{
				case AmazonInstatnceStates.Unknown: StateColor = AwsSsh.StateColor.Gray; break;
				case AmazonInstatnceStates.Pending: StateColor = AwsSsh.StateColor.Yellow; break;
				case AmazonInstatnceStates.Running: StateColor = AwsSsh.StateColor.Green; break;
				case AmazonInstatnceStates.ShuttingDown: StateColor = AwsSsh.StateColor.Yellow; break;
				case AmazonInstatnceStates.Terminated: StateColor = AwsSsh.StateColor.Red; break;
				case AmazonInstatnceStates.Stopping: StateColor = AwsSsh.StateColor.Red; break;
				case AmazonInstatnceStates.Stopped: StateColor = AwsSsh.StateColor.Red; break;
				default: Trace.TraceWarning("Unknown instance state " + State); break;
			}
		}
	}
}
