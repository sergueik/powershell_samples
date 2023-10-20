using System;
using System.Configuration;
using System.Collections.Generic;

namespace PerfTap.Configuration {
	public class CounterNameConfigurationCollection : ConfigurationElementCollection {

		public CounterNameConfigurationCollection() { }

		public CounterNameConfigurationCollection(IEnumerable<string> names) {
			foreach (var name in names) {
				// NOTE: virtual member call in constructor
				this.BaseAdd(new CounterName() { Name = name });
			}			
		}

		protected override ConfigurationElement CreateNewElement() {
			return new CounterName();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((CounterName)element).Name;
		}
	}
}