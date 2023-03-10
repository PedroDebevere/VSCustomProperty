using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Skyline.VSX.ProtocolEditor.PropertyExtenders;

using Task = System.Threading.Tasks.Task;

namespace CustomProperty
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(CustomPropertyPackage.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	public sealed class CustomPropertyPackage : AsyncPackage
	{
		/// <summary>
		/// CustomPropertyPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "e244d2f6-8acc-4af7-9f94-1499734eb890";

		private readonly Dictionary<int, IExtenderProvider> _extenderProviders = new Dictionary<int, IExtenderProvider>();

		public EnvDTE.DTE DTE { get; private set; }

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
		    await Command1.InitializeAsync(this);

			DTE = await GetServiceAsync(typeof(SDTE)) as EnvDTE.DTE;

			// register property extenders
			var provider = new PropertyExtenderProvider();
			RegisterExtenderProvider(VSLangProj.PrjBrowseObjectCATID.prjCATIDCSharpReferenceBrowseObject, ReferencesExtender.ExtenderName, provider);

		}

		private void RegisterExtenderProvider(string extenderCatId, string name, IExtenderProvider extenderProvider)
		{
			int cookie = DTE.ObjectExtenders.RegisterExtenderProvider(extenderCatId, name, extenderProvider);
			_extenderProviders.Add(cookie, extenderProvider);
		}
		#endregion

		protected override void Dispose(bool disposing)
		{
			try
			{
				foreach (var ep in _extenderProviders)
				{
					DTE.ObjectExtenders.UnregisterExtenderProvider(ep.Key);
				}

				_extenderProviders.Clear();
			}
			catch(Exception ex) { }


			base.Dispose(disposing);
		}
	}


}
