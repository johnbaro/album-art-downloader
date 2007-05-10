//Code taken from http://johnmelville.spaces.live.com/blog/cns!79D76793F7B6D5AD!122.entry
//Ensures only a single instance of this application runs, and passes command line arguments
//to the existing instance if a second instance is run.
using System;
using System.ServiceModel;

namespace AlbumArtDownloader
{

	[ServiceContract]
	public interface IPriorInstance
	{
		[OperationContract]
		void Signal(string[] parameters);
		int Run();
	}

	public static class InstanceMutex
	{
		public static void RunAppAsServiceHost(IPriorInstance instance, string channelUri)
		{
			using (ServiceHost service = new ServiceHost(instance, new Uri(channelUri)))
			{
				service.AddServiceEndpoint(typeof(IPriorInstance), new NetNamedPipeBinding(), new Uri(channelUri));
				service.Open();
				instance.Run();
			}
		}
		public static bool QueryPriorInstance(string[] args, string channelUri)
		{
			try
			{
				EndpointAddress address = new EndpointAddress(channelUri);
				IPriorInstance instance = ChannelFactory<IPriorInstance>.CreateChannel(new NetNamedPipeBinding(), address);
				
				//Exception on following line is normal behaviour (ugh) and should be ignored
				instance.Signal(args);
				((ICommunicationObject)instance).Close();

				return true;
			}
			catch (EndpointNotFoundException)
			{
				return false;
			}
		}
	}
}