defmodule Data.Message.MessageRespond do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          respondState: boolean
        }

  defstruct [:respondState]

  field :respondState, 1, type: :bool
end
