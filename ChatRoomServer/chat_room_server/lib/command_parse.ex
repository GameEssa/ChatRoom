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


  defp run( socket, [tag: "Net.Message.GetAllRoom", content: _content, packet: _packet], _server ) do
    :io.setopts([encoding: :latin1])
    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)

    #roomList = Data.Message.RoomList.new()
    rooms = RoomAgent.get_all(pid);

    roomList = Data.Message.RoomList.new(rooms: rooms)

    tagSize = byte_size("Net.Message.GetAllRoom")
    IO.puts(inspect(rooms))
    dataBytes = Data.Message.RoomList.encode(roomList);
    dataSize = byte_size(dataBytes);
    totalSize = tagSize + dataSize + 2 * 4;
    packet = int32_bytes(totalSize) <> int32_bytes(tagSize) <> "Net.Message.GetAllRoom" <> dataBytes
    IO.puts(inspect(packet))
    :gen_tcp.send(socket, packet);
  end

  defp run(socket, [tag: "Net.Message.CreateRoom", content: content, packet: packet ], _server ) do
    :io.setopts([encoding: :latin1])

    room = Data.Message.Room.decode(content);
    %Data.Message.Room{ name: name, owner: owner, participants: _participants } = room
    {:ok, pid} = RoomRegistry.lookup(RoomRegistry, :room_list)
    RoomAgent.set_room(pid, socket, room)

    name <> owner |> to_string() |> IO.puts()
    :gen_tcp.send(socket, packet);
  end


  defp int32_bytes(number) do
    <<number::big-signed-32>>
  end
end
