using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Funq.Build
{
	internal class Export
	{
		public Export()
		{
			this.Parameters = Enumerable.Empty<Import>();
			this.Properties = Enumerable.Empty<PropertyImport>();
		}

		public Contract Contract { get; set; }
		public Type Implementation { get; set; }
		public IEnumerable<Import> Parameters { get; set; }
		public IEnumerable<PropertyImport> Properties { get; set; }
	}
}
