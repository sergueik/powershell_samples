//  This file is part of YamlDotNet - A .NET library for YAML.
//  Copyright (c) 2008, 2009, 2010, 2011, 2012, 2013 Antoine Aubry

//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel.Serialization.NodeDeserializers;
using YamlDotNet.RepresentationModel.Serialization.NodeTypeResolvers;

namespace YamlDotNet.RepresentationModel.Serialization
{
	/// <summary>
	/// A fa�ade for the YAML library with the standard configuration.
	/// </summary>
	public sealed class Deserializer
	{
		private static readonly Dictionary<string, Type> predefinedTagMappings = new Dictionary<string, Type>
		{
			{ "tag:yaml.org,2002:map", typeof(Dictionary<object, object>) },
			{ "tag:yaml.org,2002:bool", typeof(bool) },
			{ "tag:yaml.org,2002:float", typeof(double) },
			{ "tag:yaml.org,2002:int", typeof(int) },
			{ "tag:yaml.org,2002:str", typeof(string) },
			{ "tag:yaml.org,2002:timestamp", typeof(DateTime) },
		};

		private readonly Dictionary<string, Type> tagMappings;
		private readonly List<IYamlTypeConverter> converters;
		private TypeDescriptorProxy typeDescriptor = new TypeDescriptorProxy();
		private IValueDeserializer valueDeserializer;

		public IList<INodeDeserializer> NodeDeserializers { get; private set; }
		public IList<INodeTypeResolver> TypeResolvers { get; private set; }

		private class TypeDescriptorProxy : ITypeDescriptor
		{
			public ITypeDescriptor TypeDescriptor;
			
			public IEnumerable<IPropertyDescriptor> GetProperties(Type type)
			{
				return TypeDescriptor.GetProperties(type);
			}

			public IPropertyDescriptor GetProperty(Type type, string name)
			{
				return TypeDescriptor.GetProperty(type, name);
			}
		}
		
		public Deserializer(IObjectFactory objectFactory = null, INamingConvention namingConvention = null)
		{
			objectFactory = objectFactory ?? new DefaultObjectFactory();
			namingConvention = namingConvention ?? new NullNamingConvention();
			
			typeDescriptor.TypeDescriptor = 
				new YamlAttributesTypeDescriptor(
					new NamingConventionTypeDescriptor(
						new ReadableAndWritablePropertiesTypeDescriptor(),
						namingConvention
					)
				);

			converters = new List<IYamlTypeConverter>();
			NodeDeserializers = new List<INodeDeserializer>();
			NodeDeserializers.Add(new TypeConverterNodeDeserializer(converters));
			NodeDeserializers.Add(new NullNodeDeserializer());
			NodeDeserializers.Add(new ScalarNodeDeserializer());
			NodeDeserializers.Add(new ArrayNodeDeserializer());
			NodeDeserializers.Add(new GenericDictionaryNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new NonGenericDictionaryNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new GenericCollectionNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new NonGenericListNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new EnumerableNodeDeserializer());
			NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, typeDescriptor));

			tagMappings = new Dictionary<string, Type>(predefinedTagMappings);
			TypeResolvers = new List<INodeTypeResolver>();
			TypeResolvers.Add(new TagNodeTypeResolver(tagMappings));
			TypeResolvers.Add(new TypeNameInTagNodeTypeResolver());
			TypeResolvers.Add(new DefaultContainersNodeTypeResolver());
			
			valueDeserializer =
				new AliasValueDeserializer(
					new NodeValueDeserializer(
						NodeDeserializers,
						TypeResolvers
					)
				);
		}

		public void RegisterTagMapping(string tag, Type type)
		{
			tagMappings.Add(tag, type);
		}

		public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
		{
			converters.Add(typeConverter);
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)Deserialize(input, typeof(T));
		}

		public object Deserialize(TextReader input)
		{
			return Deserialize(input, typeof(object));
		}

		public object Deserialize(TextReader input, Type type)
		{
			return Deserialize(new EventReader(new Parser(input)), type);
		}

		public T Deserialize<T>(EventReader reader)
		{
			return (T)Deserialize(reader, typeof(T));
		}

		public object Deserialize(EventReader reader)
		{
			return Deserialize(reader, typeof(object));
		}

		/// <summary>
		/// Deserializes an object of the specified type.
		/// </summary>
		/// <param name="reader">The <see cref="EventReader" /> where to deserialize the object.</param>
		/// <param name="type">The static type of the object to deserialize.</param>
		/// <param name="options">Options that control how the deserialization is to be performed.</param>
		/// <returns>Returns the deserialized object.</returns>
		public object Deserialize(EventReader reader, Type type)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			var hasStreamStart = reader.Allow<StreamStart>() != null;

			var hasDocumentStart = reader.Allow<DocumentStart>() != null;

			object result = null;
			if (!reader.Accept<DocumentEnd>() && !reader.Accept<StreamEnd>())
				result = valueDeserializer.DeserializeValue(reader, type, new SerializerState(), valueDeserializer);

			if (hasDocumentStart)
			{
				reader.Expect<DocumentEnd>();
			}

			if (hasStreamStart)
			{
				reader.Expect<StreamEnd>();
			}

			return result;
		}
	}
}