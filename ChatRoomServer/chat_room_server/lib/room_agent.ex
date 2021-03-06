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


  def delete_user(agent, id) do
    Agent.update(agent, fn map ->
      for {k , room} <- map , into: %{} do
        %Data.Message.Room{name: _name, owner: _owner, participants: part }  = room
        newPart = List.delete(part, id)
        newroom = Map.update(room, :participants, part, fn _oldvalue -> newPart end)
        {k, newroom}
      end
      end)
  end
end
