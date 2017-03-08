using System;
using Xamarin.Forms;

namespace VASSALChatter
{
	public interface IMessage
	{
	}

	public interface IMessenger
	{
		void Send<TMessage>(TMessage message, object sender = null)
			where TMessage : IMessage;

		void Subscribe<TMessage>(object subscriber, Action<object, TMessage> callback)
			where TMessage : IMessage;

		void Unsubscribe<TMessage>(object subscriber)
			where TMessage : IMessage;
	}

	public class FormsMessenger : IMessenger
	{
		private static volatile FormsMessenger instance;
		private static readonly object syncRoot = new Object();

		public static FormsMessenger Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new FormsMessenger();
					}
				}

				return instance;
			}
		}

		public void Send<TMessage>(TMessage message, object sender = null) where TMessage : IMessage
		{
			if (sender == null)
				sender = new object();

			MessagingCenter.Send<object, TMessage>(sender, typeof(TMessage).FullName, message);
		}

		public void Subscribe<TMessage>(object subscriber, Action<object, TMessage> callback) where TMessage : IMessage
		{
			MessagingCenter.Subscribe<object, TMessage>(subscriber, typeof(TMessage).FullName, callback, null);
		}

		public void Unsubscribe<TMessage>(object subscriber) where TMessage : IMessage
		{
			MessagingCenter.Unsubscribe<object, TMessage>(subscriber, typeof(TMessage).FullName);
		}
	}
}
