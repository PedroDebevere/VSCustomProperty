using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using EnvDTE;

using Microsoft.VisualStudio.Shell.Interop;

using Skyline.VSX.ProtocolEditor.Tools.Extensions;

namespace Skyline.VSX.ProtocolEditor.PropertyExtenders
{
	[ComVisible(true)] // Important!
	public class ReferencesExtender
	{
		public const string ExtenderName = "Example_ReferencesExtender";

		private readonly VSLangProj.Reference _reference;
		private readonly IExtenderSite _extenderSite;
		private readonly int _cookie;

		private readonly IVsBuildPropertyStorage _propertyStorage;
		private readonly uint _itemId;

		private bool _disposed;

		public ReferencesExtender(VSLangProj.Reference reference, IExtenderSite extenderSite, int cookie)
		{
			_reference = reference;
			_extenderSite = extenderSite;
			_cookie = cookie;

			//Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

			IVsBrowseObject browseObject = reference as IVsBrowseObject;
			if (browseObject != null)
			{
				browseObject.GetProjectItem(out IVsHierarchy hierarchyItem, out uint itemId);
				_propertyStorage = (IVsBuildPropertyStorage)hierarchyItem;
				_itemId = itemId;
			}
		}

		// These attributes supply the property with some information
		[DisplayName("CustomDLLPath")]
		[Category("Custom")]
		[Description(@"Allows to specify a custom value.")]
		[Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomDllPath
		{
			get { return _propertyStorage.GetItemAttribute<string>(_itemId, nameof(CustomDllPath), null); }
			set { _propertyStorage.SetItemAttribute<string>(_itemId, nameof(CustomDllPath), value, null); }
		}

		~ReferencesExtender()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_disposed) return;

			try
			{


				_extenderSite.NotifyDelete(_cookie);
			}
			catch (Exception)
			{
				// ignore
			}

			GC.SuppressFinalize(this);
			_disposed = true;
		}
	}
}