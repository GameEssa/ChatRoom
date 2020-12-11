defmodule Data.Message.Dialogue do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          tag: String.t(),
          sender: String.t(),
          message: String.t()
        }

  defstruct [:tag, :sender, :message]

  field :tag, 1, type: :string
  field :sender, 2, type: :string
  field :message, 4, type: :string
end
