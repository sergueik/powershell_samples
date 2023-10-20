using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel.Serialization
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private readonly Emitter emitter;
		private readonly IEnumerable<IYamlTypeConverter> typeConverters;

		public CustomSerializationObjectGraphVisitor(Emitter emitter, IObjectGraphVisitor nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters)
			: base(nextVisitor)
		{
			this.emitter = emitter;
			this.typeConverters = typeConverters != null
				? typeConverters.ToList()
				: Enumerable.Empty<IYamlTypeConverter>();
		}

		public override bool Enter(object value, Type type)
		{
			var typeConverter = typeConverters.FirstOrDefault(t => t.Accepts(type));
			if (typeConverter != null)
			{
				typeConverter.WriteYaml(emitter, value, type);
				return false;
			}

			var serializable = value as IYamlSerializable;
			if (serializable != null)
			{
				serializable.WriteYaml(emitter);
				return false;
			}

			return base.Enter(value, type);
		}
	}
}
