// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.11.3
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Avro.Serdes.Models
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	/// <summary>
	/// A simple message type as used by this sample.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.11.3")]
	public partial class User : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse("{\"type\":\"record\",\"name\":\"User\",\"doc\":\"A simple message type as used by this sampl" +
				"e.\",\"namespace\":\"Avro.Serdes.Models\",\"fields\":[{\"name\":\"Name\",\"type\":\"string\"},{" +
				"\"name\":\"Item\",\"type\":\"string\"}]}");
		private string _Name;
		private string _Item;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return User._SCHEMA;
			}
		}
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}
		public string Item
		{
			get
			{
				return this._Item;
			}
			set
			{
				this._Item = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.Name;
			case 1: return this.Item;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.Name = (System.String)fieldValue; break;
			case 1: this.Item = (System.String)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
