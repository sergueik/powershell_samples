﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
	public class Entity : IEntity
	{
		public string Id { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime UpdatedDate { get; set; }

		public long Version { get; set; }

		public IDictionary<string, object> Metadata { get; set; }

		public Entity()
		{
			this.Metadata = new Dictionary<string, object>();
		}
	}
}
