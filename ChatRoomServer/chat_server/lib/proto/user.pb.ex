defmodule Data.Message.User do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          userId: String.t()
        }

  defstruct [:userId]

  field :userId, 1, type: :string
end
