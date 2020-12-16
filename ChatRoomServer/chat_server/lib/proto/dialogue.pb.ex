defmodule Data.Message.Dialogue do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          roomName: String.t(),
          sender: String.t(),
          message: String.t()
        }

  defstruct [:roomName, :sender, :message]

  field :roomName, 1, type: :string
  field :sender, 2, type: :string
  field :message, 4, type: :string
end
