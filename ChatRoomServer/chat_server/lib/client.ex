defmodule Client do
  require Logger
  use GenServer, restart: :temporary

  def init([socket: client_socket, server: server]) do
    clientPid = self()
    pid = spawn_link(fn -> client_loop(client_socket, clientPid, server) end)
    Logger.info("client init")
    {:ok , %{socket: client_socket, process: pid, server: server, user: nil}}
  end

  def terminate(_reason, %{socket: _client_socket, process: _pid, server: _server, user: user}) do
    if(user != nil) do
      User.logout(user)
    end
  end

  def send_message(client, packet) do
    GenServer.cast(client, {:send, packet})
  end

  def start_link( opts ) do
    GenServer.start_link(__MODULE__, opts )
  end

  def handle_info({:quit, reason}, state) do
    {:stop, reason, state}
  end

  def handle_cast({:quit, _error, _reason}, state = %{socket: _client_socket, process: pid, server: _server, user: _user}) do
    Process.exit(pid, :normal)
    {:stop, :normal, state}
  end

  def handle_cast({:send, packet}, state = %{socket: client_socket, process: _pid, server: _server, user: _user}) do
    :gen_tcp.send(client_socket, packet)
    {:noreply, state}
  end

  def handle_cast({"Net.Message.Login", packet}, state = %{socket: client_socket, process: _pid, server: _server,  user: _user}) do
    packet |> inspect() |> IO.puts()
    %Data.Message.User{userId: userId} = Data.Message.User.decode(packet)
    User.create({userId, client_socket, self()})
    newState = Map.replace(state, :user, userId)

    {:noreply, newState}
  end

  def handle_cast({"Net.Message.CreateRoom", packet}, state) do
    packet |> inspect() |> IO.puts()
    %Data.Message.Room{name: name, owner: owner, participants: _} = Data.Message.Room.decode(packet)
    Room.create({name, owner})
    {:noreply, state}
  end

  def handle_cast({"Net.Message.GetAllRoom", _packet}, state) do
    RoomRegistry.get_all(self())
    {:noreply, state}
  end

  def handle_cast({"Net.Message.EnterRoom", packet}, state) do
    packet |> inspect() |> IO.puts()
    %Data.Message.Room{name: name, owner: join, participants: _} = Data.Message.Room.decode(packet)
    Room.add_user(name, join)
    {:noreply, state}
  end

  def handle_cast({"Net.Message.ExitRoom", packet}, state) do
    packet |> inspect() |> IO.puts()
    %Data.Message.Room{name: name, owner: dropout, participants: _} = Data.Message.Room.decode(packet)
    Room.remove_user(name, dropout)
    {:noreply, state}
  end

  def handle_cast({"Net.Message.SendDialogue", packet}, state) do
    packet |> inspect() |> IO.puts()
    %Data.Message.Dialogue{roomName: roomName, sender: sender, message: message} = Data.Message.Dialogue.decode(packet)
    Room.receive_message(roomName, sender, message)
    {:noreply, state}
  end


  def handle_cast({tag, pack}, state) do
    pack |> inspect() |> IO.puts()
    "Unhandled Pack #{tag}" |> IO.puts()
    {:noreply, state}
  end

  defp client_loop(socket, clientPid , server) do
    case :gen_tcp.recv(socket, 0) do
      {:ok, bytes} ->
        {:ok, tag, packet} = CommandParse.parse(bytes)
        GenServer.cast(clientPid, {tag, packet})
        client_loop(socket, clientPid , server)
      {error, reason} ->
        GenServer.cast(clientPid, {:quit, error, reason})
    end
  end

end
