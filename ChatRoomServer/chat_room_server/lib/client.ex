defmodule Client do
  require Logger
  use GenServer

  def start_link( opts ) do
    GenServer.start_link(__MODULE__, opts )
  end

  def handle_info({ :error, :closed }, state = [ socket: client_socket, process: pid, server: server ]) do
    send(server, {:disconnect, self(), client_socket })
    :gen_tcp.close(client_socket)
    Process.exit(pid, :kill)
    {:noreply, state}
  end

  def handle_info({ :error,_ }, state = [ socket: client_socket, process: pid, server: server ]) do
    send(server, {:disconnect, self(), client_socket })
    :gen_tcp.shutdown(client_socket, :read_write)
    Process.exit(pid, :kill)
    exit(0)
    {:noreply, state}
  end

  def init( [socket: client_socket, server: server] ) do
    clientPid = self()
    pid = spawn_link(fn -> client_loop(client_socket, clientPid, server) end)
    :gen_tcp.controlling_process(client_socket , self())
    Logger.info("client init")
    send(server, {:new_client, self(), client_socket})
    {:ok , [ socket: client_socket, process: pid, server: server ] }
  end

  defp client_loop( socket, clientPid , server ) do
    case :gen_tcp.recv( socket , 0 ) do
      {:ok, packet } ->
        handle_packet( socket, packet, server )
      {tag, reason} ->
        send(clientPid, {tag, reason})
    end
    client_loop(socket, clientPid , server)
  end

  defp handle_packet( socket, packet, server ) do
    Task.start( fn -> CommandParse.handle({socket, packet, server}) end )
  end

end
