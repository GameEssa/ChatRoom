defmodule ClientAgent do
  use Agent

  def start_link(_opts) do
    Agent.start_link(fn -> %{} end, name: ClientAgent)
  end

  def set_client(agent, socket, id) do
    Agent.update(agent, fn map ->
        Map.put(map, socket, id)
    end)
  end

  def get_client(agent, socket) do
    Agent.get(agent, fn map ->
      Map.get(map, socket)
    end)
  end

  def delete_client(agent, socket) do
    Agent.get_and_update(agent, fn map ->
      Map.pop(map, socket)
    end)
  end

  def map(agent, function) do
    Agent.update(agent, fn map ->
      function.(map)
    end)
  end
end
