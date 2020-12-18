defmodule User do
  require Logger
  use GenServer, restart: :temporary

  def init({id, socket, process}) do
    {:ok, %{id: id, socket: socket, client: process, room: nil}}
  end

  def terminate(_reason, %{id: id, socket: _socket, client: _process, room: room}) do
    Logger.info("User Destory")
    if(room != nil) do
      Room.remove_user_logout(room, id)
    end
  end

  def create({id, socket, process}) do
    pid = UserRegistry.create(UserRegistry, {id, socket, process})
    GenServer.cast(pid, {:login, true})
  end

  def send_message(id, bytes) do
    case UserRegistry.lookup(UserRegistry, id) do
      {:ok, user} ->
        GenServer.cast(user, {:send, bytes})
      :error ->
        Logger.info("user not registered")
    end
  end

  def join_room(id, roomName) do
    case UserRegistry.lookup(UserRegistry, id) do
      {:ok, user} ->
        GenServer.cast(user, {:join_room, roomName})
      :error ->
        Logger.info("user not registered")
    end
  end

  def dropout_room(id) do
    case UserRegistry.lookup(UserRegistry, id) do
      {:ok, user} ->
        GenServer.cast(user, {:dropout_room})
      :error ->
        Logger.info("user not registered")
    end
  end

  def logout(id) do
    case UserRegistry.lookup(UserRegistry, id) do
      {:ok, user} ->
        GenServer.cast(user, {:quit, :normal})
      :error ->
        Logger.info("user not registered")
    end
  end

  def start_link({id, socket, process}) do
    GenServer.start_link(__MODULE__, {id, socket, process})
  end

  def handle_cast({:login, success}, state) do
    respond = Data.Message.MessageRespond.new(respondState: success)
    bytes = PacketPack.pack(MessageTag.Tag.LoginTag.value, Data.Message.MessageRespond.encode(respond))

    GenServer.cast(self(), {:send, bytes})
    {:noreply, state}
  end

  def handle_cast({:send, bytes}, state = %{id: _id, socket: socket, client: _process, room: _room}) do
    :gen_tcp.send(socket, bytes)
    {:noreply, state}
  end

  def handle_cast({:join_room, roomName}, state) do
    newState = Map.replace(state, :room, roomName)
    {:noreply, newState}
  end

  def handle_cast({:dropout_room}, state) do
    newState = Map.replace(state, :room, nil)
    {:noreply, newState}
  end

  def handle_cast({:quit, :normal}, state) do
    {:stop, :normal, state}
  end
end
