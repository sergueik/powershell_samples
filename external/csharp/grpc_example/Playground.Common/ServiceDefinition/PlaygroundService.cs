// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: playground_service.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Playground.Common.ServiceDefinition {

  namespace Proto {

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public static partial class PlaygroundService {

      #region Descriptor
      public static pbr::FileDescriptor Descriptor {
        get { return descriptor; }
      }
      private static pbr::FileDescriptor descriptor;

      static PlaygroundService() {
        byte[] descriptorData = global::System.Convert.FromBase64String(
            string.Concat(
              "ChhwbGF5Z3JvdW5kX3NlcnZpY2UucHJvdG8iIgoGUGVyc29uEgoKAmlkGAEg", 
              "ASgFEgwKBG5hbWUYAiABKAkiFgoIUGVyc29uSWQSCgoCaWQYASABKAUiEwoR", 
              "UGVyc29uTGlzdFJlcXVlc3QiLQoSUGVyc29uTGlzdFJlc3BvbnNlEhcKBnBl", 
              "b3BsZRgBIAMoCzIHLlBlcnNvbiIbChlMaXN0ZW5Gb3JOZXdQZW9wbGVSZXF1", 
              "ZXN0Mt0BChFQbGF5Z3JvdW5kU2VydmljZRIlCg1HZXRQZXJzb25CeUlkEgku", 
              "UGVyc29uSWQaBy5QZXJzb24iABIwCg1HZXRQZXJzb25MaXN0EhIuUGVyc29u", 
              "TGlzdFJlcXVlc3QaBy5QZXJzb24iADABEjAKDENyZWF0ZVBlb3BsZRIHLlBl", 
              "cnNvbhoTLlBlcnNvbkxpc3RSZXNwb25zZSIAKAESPQoSTGlzdGVuRm9yTmV3", 
              "UGVvcGxlEhouTGlzdGVuRm9yTmV3UGVvcGxlUmVxdWVzdBoHLlBlcnNvbiIA", 
              "MAFCJqoCI1BsYXlncm91bmQuQ29tbW9uLlNlcnZpY2VEZWZpbml0aW9uYgZw", 
              "cm90bzM="));
        descriptor = pbr::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
            new pbr::FileDescriptor[] { },
            new pbr::GeneratedCodeInfo(null, new pbr::GeneratedCodeInfo[] {
              new pbr::GeneratedCodeInfo(typeof(global::Playground.Common.ServiceDefinition.Person), new[]{ "Id", "Name" }, null, null, null),
              new pbr::GeneratedCodeInfo(typeof(global::Playground.Common.ServiceDefinition.PersonId), new[]{ "Id" }, null, null, null),
              new pbr::GeneratedCodeInfo(typeof(global::Playground.Common.ServiceDefinition.PersonListRequest), null, null, null, null),
              new pbr::GeneratedCodeInfo(typeof(global::Playground.Common.ServiceDefinition.PersonListResponse), new[]{ "People" }, null, null, null),
              new pbr::GeneratedCodeInfo(typeof(global::Playground.Common.ServiceDefinition.ListenForNewPeopleRequest), null, null, null, null)
            }));
      }
      #endregion

    }
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Person : pb::IMessage<Person> {
    private static readonly pb::MessageParser<Person> _parser = new pb::MessageParser<Person>(() => new Person());
    public static pb::MessageParser<Person> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Playground.Common.ServiceDefinition.Proto.PlaygroundService.Descriptor.MessageTypes[0]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public Person() {
      OnConstruction();
    }

    partial void OnConstruction();

    public Person(Person other) : this() {
      id_ = other.id_;
      name_ = other.name_;
    }

    public Person Clone() {
      return new Person(this);
    }

    public const int IdFieldNumber = 1;
    private int id_;
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    public const int NameFieldNumber = 2;
    private string name_ = "";
    public string Name {
      get { return name_; }
      set {
        name_ = pb::Preconditions.CheckNotNull(value, "value");
      }
    }

    public override bool Equals(object other) {
      return Equals(other as Person);
    }

    public bool Equals(Person other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Name != other.Name) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0) hash ^= Id.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.Default.Format(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      return size;
    }

    public void MergeFrom(Person other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadInt32();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PersonId : pb::IMessage<PersonId> {
    private static readonly pb::MessageParser<PersonId> _parser = new pb::MessageParser<PersonId>(() => new PersonId());
    public static pb::MessageParser<PersonId> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Playground.Common.ServiceDefinition.Proto.PlaygroundService.Descriptor.MessageTypes[1]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PersonId() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PersonId(PersonId other) : this() {
      id_ = other.id_;
    }

    public PersonId Clone() {
      return new PersonId(this);
    }

    public const int IdFieldNumber = 1;
    private int id_;
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as PersonId);
    }

    public bool Equals(PersonId other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0) hash ^= Id.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.Default.Format(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      return size;
    }

    public void MergeFrom(PersonId other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PersonListRequest : pb::IMessage<PersonListRequest> {
    private static readonly pb::MessageParser<PersonListRequest> _parser = new pb::MessageParser<PersonListRequest>(() => new PersonListRequest());
    public static pb::MessageParser<PersonListRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Playground.Common.ServiceDefinition.Proto.PlaygroundService.Descriptor.MessageTypes[2]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PersonListRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PersonListRequest(PersonListRequest other) : this() {
    }

    public PersonListRequest Clone() {
      return new PersonListRequest(this);
    }

    public override bool Equals(object other) {
      return Equals(other as PersonListRequest);
    }

    public bool Equals(PersonListRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.Default.Format(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
    }

    public int CalculateSize() {
      int size = 0;
      return size;
    }

    public void MergeFrom(PersonListRequest other) {
      if (other == null) {
        return;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PersonListResponse : pb::IMessage<PersonListResponse> {
    private static readonly pb::MessageParser<PersonListResponse> _parser = new pb::MessageParser<PersonListResponse>(() => new PersonListResponse());
    public static pb::MessageParser<PersonListResponse> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Playground.Common.ServiceDefinition.Proto.PlaygroundService.Descriptor.MessageTypes[3]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PersonListResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PersonListResponse(PersonListResponse other) : this() {
      people_ = other.people_.Clone();
    }

    public PersonListResponse Clone() {
      return new PersonListResponse(this);
    }

    public const int PeopleFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Playground.Common.ServiceDefinition.Person> _repeated_people_codec
        = pb::FieldCodec.ForMessage(10, global::Playground.Common.ServiceDefinition.Person.Parser);
    private readonly pbc::RepeatedField<global::Playground.Common.ServiceDefinition.Person> people_ = new pbc::RepeatedField<global::Playground.Common.ServiceDefinition.Person>();
    public pbc::RepeatedField<global::Playground.Common.ServiceDefinition.Person> People {
      get { return people_; }
    }

    public override bool Equals(object other) {
      return Equals(other as PersonListResponse);
    }

    public bool Equals(PersonListResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!people_.Equals(other.people_)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      hash ^= people_.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.Default.Format(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      people_.WriteTo(output, _repeated_people_codec);
    }

    public int CalculateSize() {
      int size = 0;
      size += people_.CalculateSize(_repeated_people_codec);
      return size;
    }

    public void MergeFrom(PersonListResponse other) {
      if (other == null) {
        return;
      }
      people_.Add(other.people_);
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            people_.AddEntriesFrom(input, _repeated_people_codec);
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class ListenForNewPeopleRequest : pb::IMessage<ListenForNewPeopleRequest> {
    private static readonly pb::MessageParser<ListenForNewPeopleRequest> _parser = new pb::MessageParser<ListenForNewPeopleRequest>(() => new ListenForNewPeopleRequest());
    public static pb::MessageParser<ListenForNewPeopleRequest> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::Playground.Common.ServiceDefinition.Proto.PlaygroundService.Descriptor.MessageTypes[4]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public ListenForNewPeopleRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    public ListenForNewPeopleRequest(ListenForNewPeopleRequest other) : this() {
    }

    public ListenForNewPeopleRequest Clone() {
      return new ListenForNewPeopleRequest(this);
    }

    public override bool Equals(object other) {
      return Equals(other as ListenForNewPeopleRequest);
    }

    public bool Equals(ListenForNewPeopleRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.Default.Format(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
    }

    public int CalculateSize() {
      int size = 0;
      return size;
    }

    public void MergeFrom(ListenForNewPeopleRequest other) {
      if (other == null) {
        return;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
