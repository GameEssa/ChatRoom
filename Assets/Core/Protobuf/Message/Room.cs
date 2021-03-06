// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: room.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Data.Message {

  /// <summary>Holder for reflection information generated from room.proto</summary>
  public static partial class RoomReflection {

    #region Descriptor
    /// <summary>File descriptor for room.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RoomReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cgpyb29tLnByb3RvEgxEYXRhLk1lc3NhZ2UiOQoEUm9vbRINCgVvd25lchgB",
            "IAEoCRIMCgRuYW1lGAIgASgJEhQKDHBhcnRpY2lwYW50cxgDIAMoCSItCghS",
            "b29tTGlzdBIhCgVyb29tcxgBIAMoCzISLkRhdGEuTWVzc2FnZS5Sb29tYgZw",
            "cm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Data.Message.Room), global::Data.Message.Room.Parser, new[]{ "Owner", "Name", "Participants" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Data.Message.RoomList), global::Data.Message.RoomList.Parser, new[]{ "Rooms" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Room : pb::IMessage<Room> {
    private static readonly pb::MessageParser<Room> _parser = new pb::MessageParser<Room>(() => new Room());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Room> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Data.Message.RoomReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Room() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Room(Room other) : this() {
      owner_ = other.owner_;
      name_ = other.name_;
      participants_ = other.participants_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Room Clone() {
      return new Room(this);
    }

    /// <summary>Field number for the "owner" field.</summary>
    public const int OwnerFieldNumber = 1;
    private string owner_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Owner {
      get { return owner_; }
      set {
        owner_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "participants" field.</summary>
    public const int ParticipantsFieldNumber = 3;
    private static readonly pb::FieldCodec<string> _repeated_participants_codec
        = pb::FieldCodec.ForString(26);
    private readonly pbc::RepeatedField<string> participants_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Participants {
      get { return participants_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Room);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Room other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Owner != other.Owner) return false;
      if (Name != other.Name) return false;
      if(!participants_.Equals(other.participants_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Owner.Length != 0) hash ^= Owner.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      hash ^= participants_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Owner.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Owner);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      participants_.WriteTo(output, _repeated_participants_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Owner.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Owner);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      size += participants_.CalculateSize(_repeated_participants_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Room other) {
      if (other == null) {
        return;
      }
      if (other.Owner.Length != 0) {
        Owner = other.Owner;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      participants_.Add(other.participants_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Owner = input.ReadString();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
          case 26: {
            participants_.AddEntriesFrom(input, _repeated_participants_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class RoomList : pb::IMessage<RoomList> {
    private static readonly pb::MessageParser<RoomList> _parser = new pb::MessageParser<RoomList>(() => new RoomList());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RoomList> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Data.Message.RoomReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RoomList() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RoomList(RoomList other) : this() {
      rooms_ = other.rooms_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RoomList Clone() {
      return new RoomList(this);
    }

    /// <summary>Field number for the "rooms" field.</summary>
    public const int RoomsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Data.Message.Room> _repeated_rooms_codec
        = pb::FieldCodec.ForMessage(10, global::Data.Message.Room.Parser);
    private readonly pbc::RepeatedField<global::Data.Message.Room> rooms_ = new pbc::RepeatedField<global::Data.Message.Room>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Data.Message.Room> Rooms {
      get { return rooms_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RoomList);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RoomList other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!rooms_.Equals(other.rooms_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= rooms_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      rooms_.WriteTo(output, _repeated_rooms_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += rooms_.CalculateSize(_repeated_rooms_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RoomList other) {
      if (other == null) {
        return;
      }
      rooms_.Add(other.rooms_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            rooms_.AddEntriesFrom(input, _repeated_rooms_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
