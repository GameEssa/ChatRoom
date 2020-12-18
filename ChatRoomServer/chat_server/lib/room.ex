defmodule Room do
  require Logger
  use GenServer

  def init({name, owner}) do
    {:ok , %{name: name, owner: owner, participants: []}}
  end

  def create({name, owner}) do
    pid = RoomRegistry.create(RoomRegistry, {name, owner})
    GenServer.cast(pid, {:new_room, true})
  end

  def get_state(room) do
    GenServer.call(room, {:get_state})
  end

  def add_user(roomName, userName) do
    case RoomRegistry.lookup(RoomRegistry, roomName) do
      {:ok, pid} ->
        GenServer.cast(pid, {:join, userName})
      :error ->
        Logger.info("no room")
    end
  end

  def remove_user(roomName, userName) do
    case RoomRegistry.lookup(RoomRegistry, roomName) do
      {:ok, pid} ->
        GenServer.cast(pid, {:exit, userName})
      :error ->
        Logger.info("no room")
    end
  end

  def remove_user_logout(roomName, userName) do
    case RoomRegistry.lookup(RoomRegistry, roomName) do
      {:ok, pid} ->
        GenServer.cast(pid, {:exit_logout, userName})
      :error ->
        Logger.info("no room")
    end
  end

  def receive_message(roomName, sender, message) do
    case RoomRegistry.lookup(RoomRegistry, roomName) do
      {:ok, pid} ->
        GenServer.cast(pid, {:receive_message, sender, message})
      :error ->
        Logger.info("no room")
    end
  end

  def start_link(state) do
    GenServer.start_link(__MODULE__, state)
  end

  def handle_cast({:new_room, success}, %{name: name, owner: owner, participants: participants}) do
    respond = Data.Message.MessageRespond.new(respondState: success)
    bytes = PacketPack.pack(MessageTag.Tag.CreateRoomTag.value, Data.Message.MessageRespond.encode(respond))
    #:gen_tcp.send(socket, bytes)
    User.send_message(owner , bytes)

    {:noreply, %{name: name, owner: owner, participants: participants}}
  end

  def handle_cast({:join, userName}, state = %{name: name, owner: owner, participants: participants}) do
    newParticipants = List.insert_at(participants, 0, userName)
    User.join_room(userName, name)

    respond = Data.Message.Room.new(name: name, owner: owner, participants: newParticipants)
    bytes = PacketPack.pack(MessageTag.Tag.EnterRoomTag.value,  Data.Message.Room.encode(respond))
    User.send_message(userName, bytes)

    newState = Map.update(state, :participants, participants, fn _old -> newParticipants end)
    {:noreply, newState}
  end

  def handle_cast({:exit, userName}, state = %{name: _name, owner: _owner, participants: participants}) do
    newParticipants = List.delete(participants, userName)
    User.dropout_room(userName)

    respond = Data.Message.MessageRespond.new(respondState: true)
    bytes = PacketPack.pack(MessageTag.Tag.ExitRoomTag.value, Data.Message.MessageRespond.encode(respond))
    User.send_message(userName , bytes)

    newState = Map.update(state, :participants, participants, fn _old -> newParticipants end)
    {:noreply, newState}
  end

  def handle_cast({:receive_message, sender, message}, state = %{name: name, owner: _owner, participants: participants}) do
    #Send Success
    respond = Data.Message.MessageRespond.new(respondState: true)
    bytes = PacketPack.pack(MessageTag.Tag.SendDialogueTag.value, Data.Message.MessageRespond.encode(respond))
    User.send_message(sender , bytes)

    #broadcast
    dialogue = Data.Message.Dialogue.new(roomName: name, sender: sender, message: message)
    broadcast = PacketPack.pack(MessageTag.Tag.ReceiveDialogueTag.value, Data.Message.Dialogue.encode(dialogue))

    Enum.map(participants, fn participant ->
      User.send_message(participant , broadcast)
    end)

    {:noreply, state}
  end

  def handle_cast({:exit_logout, userName}, state = %{name: _name, owner: _owner, participants: participants}) do
    newParticipants = List.delete(participants, userName)
    newState = Map.update(state, :participants, participants, fn _old -> newParticipants end)
    {:noreply, newState}
  end


  def handle_call({:get_state}, _form,  %{name: name, owner: owner, participants: participants}) do
    {:reply, {name, owner, participants}, %{name: name, owner: owner, participants: participants}}
  end
end
