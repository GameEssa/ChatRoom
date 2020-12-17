defmodule Server do
  require Logger
  use GenServer

  def start_link(opts) do
    with  {:ok, port} <- Keyword.fetch(opts, :port) do
      {:ok, socket} = :gen_tcp.listen(port, [:binary, active: false, reuseaddr: true])
      Logger.info("Linstening #{port}")
      GenServer.start_link(__MODULE__, socket)
    end
  end

  def init(socket) do
    :gen_tcp.controlling_process(socket , self())
    server = self()
    pid = spawn_link(fn -> accept(socket, server) end)
    {:ok, [socket: socket, process: pid]}
  end

  defp accept(socket ,server) do
    case :gen_tcp.accept(socket) do
      {:ok, clientSocket} ->
        case DynamicSupervisor.start_child(Server.DynamicSupervisor, {Client, [socket: clientSocket, server: server]})  do
          {:ok, _} -> Logger.info("client build success")
          {:error, _} -> Logger.info("client build error")
        end
      err -> Logger.info("Linstening #{inspect err}")
    end
    accept(socket, server)
  end

  def say() do
    :read
  end
end
