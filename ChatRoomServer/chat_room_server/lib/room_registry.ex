defmodule RoomRegistry do
  use GenServer

  def init( server ) do
    refs = %{}
    names = :ets.new( server, [:named_table, {:read_concurrency , true}] )
    {:ok, {names, refs}}
  end

  def handle_call( {:create, name}, _form, {names, refs} ) do
    case lookup( names, name ) do
      {:ok, pid} -> {:reply, pid, {names, refs}}
      :error ->
        {:ok, pid} = DynamicSupervisor.start_child( RoomRegistry.Supervisor, RoomAgent )
        ref = Process.monitor( pid )
        refs = Map.put( refs, ref, name )
        :ets.insert( names, {name, pid} )
        {:reply, pid ,{names, refs}}
    end
  end

  def handle_info({:DOWN, ref, :process, _pid, _reason}, {names, refs}) do
    {name, refs} = Map.pop(refs, ref)
    :ets.delete(names, name)
    {:noreply, {names, refs}}
  end


  def handle_info(_msg, state) do
    IO.puts "Message Info"
    {:noreply, state}
  end

  def start_link(opts) do
    server_name = Keyword.fetch!( opts, :name )
    GenServer.start_link( __MODULE__, server_name, opts )
  end

  def lookup(server, name) do
    case :ets.lookup( server, name ) do
      [{^name, pid}] -> {:ok , pid}
      [] -> :error
    end
  end

  def create(server, name) do
    GenServer.call( server, {:create, name} )
  end
end
