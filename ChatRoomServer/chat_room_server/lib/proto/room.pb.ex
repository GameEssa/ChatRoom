defmodule Data.Message.Room do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          owner: String.t(),
          name: String.t(),
          participants: [String.t()]
        }

  defstruct [:owner, :name, :participants]

  field :owner, 1, type: :string
  field :name, 2, type: :string
  field :participants, 3, repeated: true, type: :string
end

defmodule Data.Message.RoomList do
  @moduledoc false
  use Protobuf, syntax: :proto3

  @type t :: %__MODULE__{
          rooms: [Data.Message.Room.t()]
        }

  defstruct [:rooms]

  field :rooms, 1, repeated: true, type: Data.Message.Room
end
