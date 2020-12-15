defmodule ClientTask do
  require Logger



  def start_link(state = [socket: client_socket, server: server]) do
    clientPid = self();
    spawn_link(fn -> client_loop(client_socket, clientPid, state) end)
    :gen_tcp.controlling_process(client_socket , self())
    Logger.info("client init")
    send(server, {:newclient ,self(), client_socket})
  end


  defp client_loop( socket, clientPid, state = [socket: client_socket, server: server] ) do
    case :gen_tcp.recv(socket , 0, 1_000) do
      {:ok, packet } ->
        #handle_packet( socket, packet )
        Task.start(CommandParse, :handle , [{socket, packet, server}])
        client_loop(socket, clientPid, state )
      {_tag, :timeout} ->
        client_loop(socket, clientPid, state )
      {_tag, :closed} ->
        send(server, {:disconnect, clientPid, client_socket })
        :gen_tcp.close(client_socket)
    end
  end
end
