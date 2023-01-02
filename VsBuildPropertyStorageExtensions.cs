using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Skyline.VSX.ProtocolEditor.Tools.Extensions
{
	public static class VsBuildPropertyStorageExtensions
	{
		public static T GetItemAttribute<T>(this IVsBuildPropertyStorage propertyStorage, uint item, string name, T defaultValue)
		{
			if (ErrorHandler.Succeeded(propertyStorage.GetItemAttribute(item, name, out string value)))
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}

			return defaultValue;
		}

		public static void SetItemAttribute<T>(this IVsBuildPropertyStorage propertyStorage, uint item, string name, T value, T defaultValue)
		{
			string sValue = Convert.ToString(value);
			string sDefaultValue = Convert.ToString(defaultValue);

			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

			if (sValue != sDefaultValue)
			{
				propertyStorage.SetItemAttribute(item, name, sValue);
			}
			else
			{
				propertyStorage.SetItemAttribute(item, name, null);
			}
		}
	}
}
