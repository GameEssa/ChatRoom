defmodule CommandParse do
  def handle( {socket, packet} ) do
    with  {:ok, command}  <- parse( packet ),
          {:ok, call } <- run( socket, command )
    do
      call |> inspect() |> IO.puts()
      :ok
    end
  end

  defp parse ( packet ) do
    <<_length::32-integer, content::binary >> = packet
    {:ok, [content: content] }
  end

  defp run( socket, [content: content] ) do
    :io.setopts([encoding: :latin1])
    proto = Data.Message.Dialogue.decode(content)
    %Data.Message.Dialogue{tag: _, sender: _, message: msg} = proto
    msg |> to_string() |> IO.puts()
    #{:ok, file} = File.open( "message", [:write] );
    #IO.write( file, msg );
    #File.close( file )
    #:gen_tcp.send( socket, content  )
    {:ok , {socket,  [content: content]}}
  end
end
