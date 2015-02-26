using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BulkSender.Common
{
	public static class UtilManager
	{
		public static string ConvertToBase64(string text)
		{
			byte[] encodedByte = Encoding.ASCII.GetBytes(text);
			string base64Encoded = Convert.ToBase64String(encodedByte);
			return base64Encoded;
		}

		public static string ConvertFromBase64(string base64Text)
		{
			try
			{
				var base64EncodedBytes = Convert.FromBase64String(base64Text);
				return Encoding.UTF8.GetString(base64EncodedBytes);
			}
			catch
			{
				return string.Empty;
			}
		}

		public static bool EmailIsValid(string email)
		{
			string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			if (Regex.IsMatch(email, expresion))
			{
				if (Regex.Replace(email, expresion, string.Empty).Length == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public static string PostXmlData(string url, string postData, int timeoutSeconds = 100)
		{
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = SSLCertNonVerificationHandler;
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "POST";
				request.ContentLength = bytes.Length;
				request.ContentType = "text/xml";
				request.Timeout = 1000 * timeoutSeconds;
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					using (var responseStream = response.GetResponseStream())
					{
						if (responseStream != null)
							using (var responseReader = new StreamReader(responseStream))
							{
								var responseText = responseReader.ReadToEnd();
								return responseText;
							}
					}
				}
			}
			catch (WebException ex)
			{
				throw new WebException("PostXmlData Error(ex)", ex);

			}
			catch (Exception ex2)
			{
				throw new Exception("PostXmlData Error(ex2)", ex2);
			}
			return string.Empty;
		}

		public static bool SSLCertNonVerificationHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
		{
			return true;
		}

		/// <summary>
		/// ToType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		public static object ToType<T>(this object obj, T type)
		{
			var tmp = Activator.CreateInstance(Type.GetType(type.ToString()));
			foreach (var pi in obj.GetType().GetProperties())
			{
				tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
			}
			return tmp;
		}

		/// <summary>
		/// ToNonAnonymousList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="t"></param>
		public static object ToNonAnonymousList<T>(this List<T> list, Type t)
		{
			var genericType = typeof(List<>).MakeGenericType(t);
			var l = Activator.CreateInstance(genericType);
			var addMethod = l.GetType().GetMethod("Add");
			foreach (var item in list)
			{
				addMethod.Invoke(l, new[] { item.ToType(t) });
			}
			return l;
		}

		/// <summary>
		/// TypeName
		/// </summary>
		/// <param name="value"></param>
		public static string TypeName(object value)
		{
			return value.GetType().ToString();
		}

	}
}