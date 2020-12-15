defmodule RoomAgent do
  use Agent

  def start_link(_opts) do
    Agent.start_link(fn -> %{} end)
  end

  def get_room(agent, key) do
    Agent.get(agent, fn map -> Map.get(map, key) end)
  end

  def set_room(agent, key, room) do
    Agent.update(agent, fn map ->
        Map.put(map, key, room)
    end)
  end

  def delete_room(agent, key) do
    Agent.get_and_update(agent, fn map -> Map.pop(map, key) end)
  end

  def get_all(agent) do
    Agent.get(agent, fn map-> Map.values(map) end)
  end

  def lookup(agent, key) do
    Agent.get(agent, fn map -> Map.has_key?(map, key) end)
  end
end
