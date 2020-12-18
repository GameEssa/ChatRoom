defmodule UserRegistry do
  use GenServer

  def init( server ) do
    refs = %{}
    userIds = :ets.new( server, [:named_table, {:read_concurrency , true}] )
    {:ok, {userIds, refs}}
  end

  def lookup(server, userId) do
    case :ets.lookup(server, userId) do
      [{^userId, pid}] -> {:ok , pid}
      [] -> :error
    end
  end

  def create(server, {userIds, clientSocket, clientProcess}) do
    GenServer.call(server, {:create, userIds, clientSocket, clientProcess})
  end

  def start_link(opts) do
    server_name = Keyword.fetch!(opts, :name)
    GenServer.start_link( __MODULE__, server_name, opts)
  end

  def handle_call({:create, userId, clientSocket, clientProcess}, _form, {userIds, refs}) do
    case lookup(userIds, userId) do
      {:ok, pid} -> {:reply, pid, {userIds, refs}}
      :error ->
        {:ok, pid} = DynamicSupervisor.start_child(UserRegistry.DynamicSupervisor, {User, {userId, clientSocket, clientProcess}})
        ref = Process.monitor(pid)
        refs = Map.put(refs, ref, userId)
        :ets.insert(userIds, {userId, pid})
        {:reply, pid ,{userIds, refs}}
    end
  end

  def handle_info({:DOWN, ref, :process, _pid, _reason}, {userIds, refs}) do
    {userId, refs} = Map.pop(refs, ref)
    :ets.delete(userIds, userId)
    {:noreply, {userIds, refs}}
  end

  def handle_info(_msg, state) do
    IO.puts "Message Info"
    {:noreply, state}
  end
end
