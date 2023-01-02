using EnvDTE;

namespace Skyline.VSX.ProtocolEditor.PropertyExtenders
{
	public class PropertyExtenderProvider : IExtenderProvider
	{
		public object GetExtender(string extenderCATID, string extenderName, object extendeeObject, IExtenderSite extenderSite, int cookie)
		{
			if (CanExtend(extenderCATID, extenderName, extendeeObject))
			{
				var reference = extendeeObject as VSLangProj.Reference;
				return new ReferencesExtender(reference, extenderSite, cookie);
			}

			return null;
		}

		public bool CanExtend(string extenderCATID, string extenderName, object extendeeObject)
		{
			if (extenderName == ReferencesExtender.ExtenderName)
				return true;

			return false;
		}

	}
}
