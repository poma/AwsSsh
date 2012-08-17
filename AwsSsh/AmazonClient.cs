﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using AwsSsh.Properties;
using System.Windows;

namespace AwsSsh
{
	public static class AmazonClient
	{
		public static List<Instance> GetInstances()
		{
			try
			{
				var result = new List<Instance>();

				AmazonEC2 ec2 = AWSClientFactory.CreateAmazonEC2Client(
					Settings.Default.AWSAccessKey,
					Settings.Default.AWSSecretKey,
					new AmazonEC2Config { ServiceURL = Settings.Default.ServiceUrl }
					);

				var ec2Response = ec2.DescribeInstances(new DescribeInstancesRequest());

				foreach (var image in ec2Response.DescribeInstancesResult.Reservation.SelectMany(a => a.RunningInstance))
				{
					var instance = new Instance()
					{
						Id = image.InstanceId,
						Name = image.Tag[0].Value,
						StateName = image.InstanceState.Name,
						State = (InstatnceStates)image.InstanceState.Code,
						PublicIp = image.IpAddress,
						PrivateIp = image.PrivateIpAddress,
						InstanceType = image.InstanceType,
						PublicDnsName = image.PublicDnsName,
						PrivateDnsName = image.PrivateDnsName
					};
					result.Add(instance);
				}
				return result.OrderBy(a => a.Name).ToList();
			}
			catch (AmazonEC2Exception ex)
			{
				string message = ex.Message + "\r\n" +
				"Response Status Code: " + ex.StatusCode + "\r\n" +
				"Error Code: " + ex.ErrorCode + "\r\n" +
				"Error Type: " + ex.ErrorType + "\r\n" +
				"Request ID: " + ex.RequestId;
				throw new AmazonEC2Exception(message, ex);
			}
		}

		/// <summary>
		/// Used to merge new instance info but retain references
		/// </summary>
		public static void MergeInstanceList(ObservableCollection<Instance> existingInstances, List<Instance> newInstances)
		{
			var c = new InstanceComparer();
			var itemsToRemove = existingInstances.Except(newInstances, c).ToList();
			var itemsToAdd = newInstances.Except(existingInstances, c).ToList();
			var itemsToUpdate = existingInstances.Join(newInstances, a => a.Id, a => a.Id, (a, b) => new { Old = a, New = b }).ToList();
			itemsToAdd.ForEach(a => existingInstances.Add(a));
			itemsToRemove.ForEach(a => existingInstances.Remove(a));
			itemsToUpdate.ForEach(a => Instance.AssignInstance(a.Old, a.New));
		}

		public static bool CheckConnection()
		{

			try
			{
				AmazonEC2 ec2 = AWSClientFactory.CreateAmazonEC2Client(
						Settings.Default.AWSAccessKey,
						Settings.Default.AWSSecretKey,
						new AmazonEC2Config { ServiceURL = Settings.Default.ServiceUrl }
						);

				ec2.DescribeInstances(new DescribeInstancesRequest());

				return true;
			}
			catch (AmazonEC2Exception ex)
			{
				string message = ex.Message + "\r\n" +
				"Response Status Code: " + ex.StatusCode + "\r\n" +
				"Error Code: " + ex.ErrorCode + "\r\n" +
				"Error Type: " + ex.ErrorType + "\r\n" +
				"Request ID: " + ex.RequestId;
				MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
	}
}
