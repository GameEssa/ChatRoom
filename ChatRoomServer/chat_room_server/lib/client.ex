defmodule Client do
  use GenServer

  def start_link( opts ) do
    GenServer.start_link(__MODULE__, opts )
  end

  def handle_info( msg, state ) do
    msg |> inspect() |> IO.puts()
    {:noreply, state}
  end

  def init( init_args ) do
    socket = Keyword.fetch!(init_args, :socket)
    pid = spawn_link( fn -> client_loop(socket) end)
    :gen_tcp.controlling_process( socket , self() )
    {:ok , [ socket: socket , process: pid] }
  end

  defp client_loop( socket ) do
    case :gen_tcp.recv( socket , 0 ) do
      {:ok, packet } ->
        handle_packet( socket, packet )
      {tag, reason} ->
        {tag, reason} |> inspect() |> IO.puts()
        exit(0)
    end
    client_loop( socket )
  end

  defp handle_packet( socket, packet ) do
    Task.start( fn -> CommandParse.handle({socket, packet}) end )
  end
end
