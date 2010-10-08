using System;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Composition;
using Funq.Build.Properties;

namespace Funq.Build
{
	public class FunqletGenerator : AppDomainIsolatedTask
	{
		public string OutputPath { get; set; }
		public string TargetNamespace { get; set; }
		public ITaskItem[] References { get; set; }

		[Output]
		public ITaskItem Funqlet { get; private set; }

		public override bool Execute()
		{
			var exportedTypes = this.References
				.Where(item => item.GetMetadata("CopyLocal") == "true")
				.Select(item => Assembly.LoadFrom(item.GetMetadata("FullPath")))
				.SelectMany(asm => asm.GetTypes())
				.Where(type =>
					type.HasCustomAttribute<ExportAttribute>(false) ||
					type.HasCustomAttribute<InheritedExportAttribute>(true));

			var template = new FunqletTemplate
			{
				Exports = BuildExports(exportedTypes),
				TargetNamespace = TargetNamespace,
			};

			var fileName = Path.Combine(this.OutputPath, this.TargetNamespace + ".Funqlet.cs");

			File.WriteAllText(fileName, template.TransformText(), Encoding.UTF8);

			this.Funqlet = new TaskItem(fileName);

			this.Log.LogMessage(Resources.GenerateSuccessfull, fileName);

			return true;
		}

		private IEnumerable<Export> BuildExports(IEnumerable<Type> exportedTypes)
		{
			foreach (var type in exportedTypes)
			{
				ExportAttribute attr =
					type.GetCustomAttribute<InheritedExportAttribute>(true) ??
					type.GetCustomAttribute<ExportAttribute>(false);

				if (attr.ContractType != null && !attr.ContractType.IsAssignableFrom(type))
				{
					this.Log.LogError(Resources.IncompatibleExportImplementation, type, attr.ContractType);
					continue;
				}

				// Find an importing constructor, or log error.
				var export = new Export
				{
					Contract = new Contract { Type = attr.ContractType ?? type, Name = attr.ContractName },
					Implementation = type,
					Properties = BuildProperties(type),
				};

				if (HasNoDefaultConstructor(type))
				{
					// Find the constructor with the proper attribute
					var ctor = type
						.GetConstructors()
						.FirstOrDefault(c => c.HasCustomAttribute<ImportingConstructorAttribute>());
					// or log an error.
					if (ctor == null)
					{
						this.Log.LogError(Resources.ImportingConstructorMissing, type);
						continue;
					}

					export.Parameters = BuildParameters(ctor);
				}

				yield return export;
			}
		}

		private static bool HasNoDefaultConstructor(Type type)
		{
			return type.GetConstructor(new Type[0]) == null;
		}

		private IEnumerable<PropertyImport> BuildProperties(Type type)
		{
			return type.GetProperties()
				.Where(prop => prop.HasCustomAttribute<ImportAttribute>())
				.Select(prop => new { Property = prop, Import = prop.GetCustomAttribute<ImportAttribute>() })
				.Select(prop => new PropertyImport
				{
					Name = prop.Property.Name,
					Contract = new Contract
					{
						Name = prop.Import.ContractName,
						Type = prop.Import.ContractType ?? prop.Property.PropertyType,
					},
					AllowDefault = prop.Import.AllowDefault,
				});
		}

		private IEnumerable<Import> BuildParameters(ConstructorInfo ctor)
		{
			foreach (var parameter in ctor.GetParameters())
			{
				var import = parameter.GetCustomAttribute<ImportAttribute>();
				if (import == null)
				{
					yield return new Import
					{
						AllowDefault = false,
						Contract = new Contract
						{
							Type = parameter.ParameterType
						},
					};
				}
				else
				{
					yield return new Import
					{
						AllowDefault = import.AllowDefault,
						Contract = new Contract
						{
							Type = import.ContractType ?? parameter.ParameterType,
							Name = import.ContractName
						},
					};
				}
			}
		}
	}
}
