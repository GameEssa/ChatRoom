defmodule CommandParse do
  def handle( {socket, packet, server} ) do
    with  {:ok, command}  <- parse( packet ),
          {:ok, call } <- run( socket, command, server )
    do
      call |> inspect() |> IO.puts()
      :ok
    end
  end

  defp parse ( packet ) do
    <<_length::32-integer, tagLength::32-integer , tail::binary>> = packet
    <<tag::binary-size(tagLength) , content::binary>> = tail
    tag = to_string(tag)
    {:ok, [tag: tag, content: content, packet: packet] }
  end

  defp run(socket, [tag: "Net.Message.Login", content: content, packet: _packet], _server) do
    {:ok, agent} = ClientRegistry.lookup(ClientRegistry, :client_map)
    %Data.Message.User{userId: id} = Data.Message.User.decode(content)
    ClientAgent.set_client(agent, socket, id)
    respond = Data.Message.MessageRespond.new(respondState: true)
    id <> "Login:" |> IO.puts()
    send_message(socket, "Net.Message.Login", Data.Message.MessageRespond.encode(respond))
  end

  defp run(socket, [tag: "Net.Message.GetAllRoom", content: _content, packet: _packet], _server) do
    :io.setopts([encoding: :latin1])
    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)

    rooms = RoomAgent.get_all(pid);
    roomList = Data.Message.RoomList.new(rooms: rooms)

    send_message(socket, "Net.Message.GetAllRoom", Data.Message.RoomList.encode(roomList))
  end

  defp run(socket, [tag: "Net.Message.CreateRoom", content: content, packet: packet], _server) do
    :io.setopts([encoding: :latin1])

    room = Data.Message.Room.decode(content);
    %Data.Message.Room{ name: name, owner: owner, participants: _participants } = room
    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)

    if(RoomAgent.lookup(pid, name) == false) do
     RoomAgent.set_room(pid, name, room)
     owner <> "create room :" <> name |> IO.puts()
     :gen_tcp.send(socket, packet);
    else
      send_message(socket, "Net.Message.CreateRoom", nil)
    end

  end

  defp run(socket, [tag: "Net.Message.EnterRoom", content: content, packet: _packet], _server) do
    :io.setopts([encoding: :latin1])
    room = Data.Message.Room.decode(content);
    %Data.Message.Room{ name: name, owner: jion, participants: _participants } = room

    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)
    result = RoomAgent.get_room(pid, name)
    if( result != nil) do
      %Data.Message.Room{ name: _name, owner: owner, participants: oldparticipants } = result
      newParticipant = oldparticipants ++ [ jion ]
      newRoom = Data.Message.Room.new(name: name, owner: owner, participants: newParticipant)
      RoomAgent.set_room(pid, name, newRoom)

      newRoom |> inspect() |> IO.puts()
      send_message(socket, "Net.Message.EnterRoom", Data.Message.Room.encode(newRoom))
    else
      send_message(socket, "Net.Message.EnterRoom", nil)
    end
  end

  defp int32_bytes(number) do
    <<number::big-signed-32>>
  end

  defp send_message(socket, tag, nil) do
    tagSize = byte_size(tag)
    totalSize = tagSize + 2 * 4;

    packet = int32_bytes(totalSize) <> int32_bytes(tagSize) <> tag
    :gen_tcp.send(socket, packet);
  end

  defp send_message(socket, tag, content) do
    tagSize = byte_size(tag)
    dataSize = byte_size(content);
    totalSize = tagSize + dataSize + 2 * 4;

    packet = int32_bytes(totalSize) <> int32_bytes(tagSize) <> tag <> content
    :gen_tcp.send(socket, packet);
  end
end
