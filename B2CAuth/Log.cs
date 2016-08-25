using System;
namespace B2CAuth
{
	public static class Log
	{
		public static void Info(string format, params object[] objects)
		{
			string message = string.Format(format, objects);
			PrintInfo(message);
		}

		public static void Info(object message)
		{
			PrintInfo(message != null ? message.ToString() : "NULL");
		}

		private static void PrintInfo(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		public static void Error(string format, params object[] objects)
		{
			string message = string.Format(format, objects);
			LogError(message);
		}

		public static void Error(object message)
		{
			LogError(message != null ? message.ToString() : "NULL");
		}

		public static void Error(Exception exception)
		{
			string message = exception.Message;

			var innerException = exception;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}

			message += ("\n" + innerException.Message);
			LogError(message);
		}

		private static void LogError(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
	}
}

