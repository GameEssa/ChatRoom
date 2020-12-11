defmodule Server do
  require Logger
  use GenServer
  @moduledoc """
    Chat Room Server Open tcp listen and accept client message
  """

  def start_link( opts ) do
    with  {:ok, port} <- Keyword.fetch( opts, :port ) do
      IO.puts port
      IO.puts( inspect( self() ) )
      {:ok, socket} = :gen_tcp.listen( port,  [:binary, active: false, reuseaddr: true] )
      Logger.info( "Linstening #{port}" )
      GenServer.start_link( __MODULE__, socket )
    end
  end

  def handle_info( msg, state ) do
    msg |> inspect() |> IO.puts()
    {:noreply, state}
  end

  def init( socket ) do
    :gen_tcp.controlling_process( socket , self() )
    pid = spawn_link( fn -> accept(socket) end)
    IO.puts( inspect( pid ) )
    {:ok, [socket: socket, process: pid] }
  end

  defp accept ( socket ) do
    case :gen_tcp.accept(socket) do
      {:ok, client_socket} ->
        #GenServer.start_link(Client, socket: client_socket)
        DynamicSupervisor.start_child(Server.DynamicSupervisor, {Client, [socket: client_socket]})
      err -> Logger.info( "Linstening #{inspect err}" )
    end
    accept(socket)
  end

end
