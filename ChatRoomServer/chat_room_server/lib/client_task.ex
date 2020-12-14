defmodule ClientTask do
  require Logger

  def start_link( state = [socket: client_socket, server: server] ) do
    clientPid = self();
    spawn_link(fn -> client_loop(client_socket, clientPid, state) end)
    :gen_tcp.controlling_process(client_socket , self())
    Logger.info("client init")
    send(server, {:newclient, self(), client_socket})
  end

  def handle([ socket: client_socket, process: _pid, server: server ]) do
    send(server, {:disconnect, self(), client_socket })
    :gen_tcp.close(client_socket)
  end


 # def handle_info({ :error,_ }, state = [ socket: client_socket, process: pid, server: server ]) do
 #   send(server, {:disconnect, self(), client_socket })
 #   :gen_tcp.shutdown(client_socket, :read_write)
 #   Process.exit(pid, :kill)
 #   exit(0)
 #   {:noreply, state}
 # end

  defp client_loop( socket, clientPid, state = [socket: client_socket, server: server] ) do
    case :gen_tcp.recv( socket , 0 ) do
      {:ok, packet } ->
        #handle_packet( socket, packet )
        Task.start(CommandParse, :handle , [{socket, packet, server}])
        client_loop(socket, clientPid, state )
      {_tag, _reason} ->
        send(server, {:disconnect, clientPid, client_socket })
        :gen_tcp.close(client_socket)
    end
  end
end
