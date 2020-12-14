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

  def handle_info({:new_client, pid, socket}, state) do
    IO.puts("New Client #{inspect pid}  #{inspect socket}" )
    :ets.insert(:clientList, { pid, socket })
    {:noreply, state}
  end

  def handle_info({:disconnect, pid , socket}, state) do
    IO.puts("Disconnect #{inspect pid}")
    :ets.delete(:clientList, pid)
    {:ok, roomPid} = RoomRegistry.lookup(RoomRegistry, :room_list)
    RoomAgent.delete_room(roomPid, socket)
    Process.exit(pid, :normal)
    {:noreply, state}
  end

  def handle_info({:new_room, room}, state) do
    %Data.Message.Room{ name: name, owner: _owner, participants: _participants } = room
    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)
    RoomAgent.set_room(pid, name, room)
    {:noreply, state}
  end


  def handle_info(msg, state) do
    IO.puts "Server"
    msg |> inspect() |> IO.puts()
    {:noreply, state}
  end

  def init(socket) do
    :gen_tcp.controlling_process(socket , self())
    server = self()
    pid = spawn_link(fn -> accept(socket, server ) end)
    :ets.new(:clientList , [:protected, :named_table, read_concurrency: true]);
    RoomRegistry.create( RoomRegistry, :room_list )
    {:ok, [socket: socket, process: pid] }
  end

  defp accept(socket ,server) do
    case :gen_tcp.accept(socket) do
      {:ok, client_socket} ->
        #DynamicSupervisor.start_child(Server.DynamicSupervisor, {Client, [socket: client_socket, server: server]})
        #GenServer.start_link(Client, [socket: client_socket, server: server])
        Task.Supervisor.start_child( Server.TaskSupervisor, ClientTask, :start_link, [[socket: client_socket, server: server]])
      err -> Logger.info( "Linstening #{inspect err}" )
    end
    accept(socket, server)
  end

end
