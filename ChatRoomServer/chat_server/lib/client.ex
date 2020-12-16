defmodule Client do
  require Logger
  use GenServer, restart: :temporary

  def start_link( opts ) do
    GenServer.start_link(__MODULE__, opts )
  end

  def handle_info({:quit, reason}, state) do
    {:stop, reason, state}
  end

  def handle_cast({:quit, reason}, state) do
    {:stop, reason, state}
  end

  def handle_cast({"Net.Message.Login", packet}, state = [socket: client_socket, process: _pid, server: _server]) do
    packet |> inspect() |> IO.puts()
    %Data.Message.User{userId: userId} = Data.Message.User.decode(packet)
    UserRegistry.create(UserRegistry, {userId, client_socket, self()} )
    {:noreply, state}
  end

  def init([socket: client_socket, server: server]) do
    clientPid = self()
    pid = spawn_link(fn -> client_loop(client_socket, clientPid, server) end)
    Logger.info("client init")
    {:ok , [socket: client_socket, process: pid, server: server]}
  end

  def terminate(reason, state) do
    state |> inspect() |> IO.puts()
    reason |> inspect() |> IO.puts()
  end

  defp client_loop(socket, clientPid , server) do
    case :gen_tcp.recv(socket, 0) do
      {:ok, bytes} ->
        {:ok, tag, packet} = CommandParse.parse(bytes)
        GenServer.cast(clientPid, {tag, packet})
        client_loop(socket, clientPid , server)
      {error, reason} ->
        Client.client_exit({error, reason}, clientPid)
        #{tag, reason} |> inspect() |> IO.puts()
    end
  end

  def client_exit({_error, reason}, clientPid) do
    send(clientPid, {:quit, reason})
  end

end
