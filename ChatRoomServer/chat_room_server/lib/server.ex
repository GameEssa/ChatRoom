defmodule Server do
  require Logger
  use GenServer
  @moduledoc """
    Chat Room Server Open tcp listen and accept client message
  """

  def start_link(opts) do
    with  {:ok, port} <- Keyword.fetch(opts, :port) do
      {:ok, socket} = :gen_tcp.listen(port, [:binary, active: false, reuseaddr: true])
      Logger.info("Linstening #{port}")
      GenServer.start_link(__MODULE__, socket)
    end
  end

  def handle_info({:new_client, clientPid, socket}, state) do
    IO.puts("New Client #{inspect clientPid}  #{inspect socket}" )
    {:noreply, state}
  end

  def handle_info({:disconnect, clientPid , socket}, state) do
    IO.puts("Disconnect #{inspect clientPid}")

    {:ok, agent} = ClientRegistry.lookup(ClientRegistry, :client_map)
    id = ClientAgent.get_client(agent, socket)
    ClientAgent.delete_client(agent, socket)

    {:ok, roomAgent} = RoomRegistry.lookup(RoomRegistry, :room_list);
    RoomAgent.delete_user(roomAgent, id);
    :gen_tcp.close(socket)
    Process.exit(clientPid, :normal)
    {:noreply, state}
  end

  def handle_info(msg, state) do
    "Server Info" <> msg |> inspect() |> IO.puts()
    {:noreply, state}
  end

  def init(socket) do
    :gen_tcp.controlling_process(socket , self())

    server = self()
    pid = spawn_link(fn -> accept(socket, server) end)

    ClientRegistry.create(ClientRegistry, :client_map)
    RoomRegistry.create(RoomRegistry, :room_list)

    {:ok, [socket: socket, process: pid]}
  end

  defp accept(socket ,server) do
    case :gen_tcp.accept(socket) do
      {:ok, client_socket} ->
        #Task.Supervisor.start_child(Server.TaskSupervisor, ClientTask, :start_link, [[socket: client_socket, server: server]])
        DynamicSupervisor.start_child(Client, [socket: client_socket, server: server])
      err -> Logger.info("Linstening #{inspect err}")
    end

    accept(socket, server)
  end
end
