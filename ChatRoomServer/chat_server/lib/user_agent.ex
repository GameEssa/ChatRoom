defmodule UserAgent do
  #use Agent
  use GenServer

  def start_link({id, socket, process}) do
    #Agent.start(fn ->
    #  %{userId: id, socket: socket, process: process}
    #end)
    GenServer.start_link(__MODULE__, {id, socket, process} )
  end

  def init(state) do
    #start_link(opts)
    {:ok , state}
  end
end
