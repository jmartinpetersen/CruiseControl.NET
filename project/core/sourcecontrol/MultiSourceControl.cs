using System;
using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("multi")]
	public class MultiSourceControl : ITemporaryLabeller, ISourceControl
	{
		private IList _sourceControls;

		[ReflectorCollection("sourceControls", InstanceType=typeof(ArrayList), Required=true)]
		public IList SourceControls 
		{
			get 
			{
				if (_sourceControls == null)
					_sourceControls = new ArrayList();

				return _sourceControls;
			}

			set { _sourceControls = value; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			ArrayList modifications = new ArrayList();
			foreach (ISourceControl sourceControl in SourceControls)
			{
				Modification[] mods = sourceControl.GetModifications(from, to);
				if (mods != null)
				{
					modifications.AddRange(mods);
				}
			}

			return (Modification[]) modifications.ToArray(typeof(Modification));
		}

		public void LabelSourceControl(string label, DateTime timeStamp)
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.LabelSourceControl(label, timeStamp);
			}
		}

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return true;
		}

		public void Run(IntegrationResult result, IProject project)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public void CreateTemporaryLabel()
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				if ( sourceControl is ITemporaryLabeller )
				{
					( (ITemporaryLabeller) sourceControl ).CreateTemporaryLabel();
				}
			}
		}

		public void DeleteTemporaryLabel()
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				if ( sourceControl is ITemporaryLabeller )
				{
					( (ITemporaryLabeller) sourceControl ).DeleteTemporaryLabel();
				}
			}
		}

		public void GetSource(IntegrationResult result) 
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.GetSource(result);
			}
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}
