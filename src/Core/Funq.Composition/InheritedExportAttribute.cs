
namespace System.ComponentModel.Composition
{
	/// <include file='Composition.xdoc' path='docs/doc[@for="InheritedExportAttribute"]/*'/>
	// NOTE: we do not support AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method for now.
	// NOTE: we do not support AllowMultiple = true for now.
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class InheritedExportAttribute : ExportAttribute
	{
		/// <include file='Composition.xdoc' path='docs/doc[@for="ExportAttribute.ctor"]/*'/>
		public InheritedExportAttribute()
		{
		}

		/// <include file='Composition.xdoc' path='docs/doc[@for="ExportAttribute.ctor(string)"]/*'/>
		public InheritedExportAttribute(string contractName)
			: base(contractName, null)
		{
		}

		/// <include file='Composition.xdoc' path='docs/doc[@for="ExportAttribute.ctor(Type)"]/*'/>
		public InheritedExportAttribute(Type contractType)
			: base(null, contractType)
		{
		}

		/// <include file='Composition.xdoc' path='docs/doc[@for="ExportAttribute.ctor(string, Type)"]/*'/>
		public InheritedExportAttribute(string contractName, Type contractType)
			: base(contractName, contractType)
		{
		}
	}
}
