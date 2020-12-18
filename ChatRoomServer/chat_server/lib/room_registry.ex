defmodule RoomRegistry do
  use GenServer

  def init(serverName) do
    refs = %{}
    rooms = :ets.new(serverName, [:named_table, {:read_concurrency , true}])
    {:ok, {rooms, refs}}
  end

  def lookup(server, roomName) do
    case :ets.lookup(server, roomName) do
      [{^roomName, room}] -> {:ok , room}
      [] -> :error
    end
  end

  def create(server, roomData) do
    GenServer.call(server, {:create, roomData})
  end

  def get_all(client) do
    roomList = GenServer.call(RoomRegistry, {:get_rooms})
    packet = PacketPack.pack(MessageTag.Tag.GetAllRoomTag.value, Data.Message.RoomList.encode(Data.Message.RoomList.new(rooms: roomList)))
    Client.send_message(client, packet)
  end

  def start_link(opts) do
    serverName = Keyword.fetch!(opts, :name)
    GenServer.start_link(__MODULE__, serverName, opts)
  end

  def handle_call({:create, {roomName, owner}}, _from, {rooms, refs}) do
    case lookup(rooms, roomName) do
      {:ok, room} -> {:reply, room, {rooms, refs}}
      :error ->
        {:ok, room} = DynamicSupervisor.start_child(RoomRegistry.DynamicSupervisor, {Room, {roomName, owner}})

        ref = Process.monitor(room)
        Map.put(refs, ref, roomName)
        :ets.insert(rooms, {roomName, room})

        {:reply, room, {rooms, refs}}
    end
  end

  def handle_call({:get_rooms}, _from, {rooms, refs}) do
    list = :ets.tab2list(rooms)

    {_, result} = Enum.map_reduce(list, [],  fn {name, room}, roomList ->
      {roomName, roomOwner, roomParticipants} = Room.get_state(room)
      protoRoom = Data.Message.Room.new(name: roomName, owner: roomOwner, participants: roomParticipants)
      {{name, room}, List.insert_at(roomList, 0, protoRoom)}
    end)

    {:reply, result, {rooms, refs}}
  end

  def handle_info({:DOWN, ref, :process, _pid, _reason}, {rooms, refs}) do
    {roomName, refs} = Map.pop(refs, ref)
    :ets.delete(rooms, roomName)
    {:noreply, {rooms, refs}}
  end

end
